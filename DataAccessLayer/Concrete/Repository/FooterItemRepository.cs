using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Shared.DTO.Channel.Response;
using Shared.DTO.FooterItem.Request;
using Shared.DTO.FooterItem.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class FooterItemRepository : IFooterItemRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    private readonly IMapper _mapper;
    
    public FooterItemRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateFooterItem

    public async Task<FooterItemResponse> CreateFooterItem(CreateFooterItemRequest footerItemRequest)
    {
        await IsExistGeneric(x => x.Label.Trim() == footerItemRequest.Label.Trim());

        await IsExistOrderNumber(footerItemRequest.OrderNumber);
        
        var footerItem = new FooterItem();
        
        footerItem.Label = footerItemRequest.Label.Trim();
        footerItem.Column = footerItemRequest.Column;
        footerItem.OrderNumber = footerItemRequest.OrderNumber;
        footerItem.TargetUrl = footerItemRequest.TargetUrl.Trim();
        footerItem.IsActive = footerItemRequest.IsActive;
        
        _applicationDbContext.FooterItems.Add(footerItem);
        
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<FooterItemResponse>(footerItem);
    }

    #endregion

    #region UpdateFooterItem

    public async Task<FooterItemResponse> UpdateFooterItem(UpdateFooterItemRequest updateFooterItemRequest)
    {
        var footerItem = await _applicationDbContext.FooterItems
                             .FirstOrDefaultAsync(x => x.FooterItemId == updateFooterItemRequest.FooterItemId)
                         ?? throw new NotFoundException(FooterItemExceptionMessages.NotFound);

        await IsExistWhenUpdate(updateFooterItemRequest.FooterItemId, updateFooterItemRequest.OrderNumber,
            updateFooterItemRequest.Label);

        footerItem.Label = updateFooterItemRequest.Label.Trim();
        footerItem.Column = updateFooterItemRequest.Column;
        footerItem.OrderNumber = updateFooterItemRequest.OrderNumber;
        footerItem.TargetUrl = updateFooterItemRequest.TargetUrl.Trim();
        footerItem.IsActive = updateFooterItemRequest.IsActive;

        _applicationDbContext.FooterItems.Update(footerItem);

        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<FooterItemResponse>(footerItem);
    }

    #endregion

    #region DeleteFooterItem

    public async Task<Guid> DeleteFooterItem(Guid footerItemId)
    {
        var footerItem = await _applicationDbContext.FooterItems
                             .FirstOrDefaultAsync(x => x.FooterItemId == footerItemId)
                         ?? throw new NotFoundException(FooterItemExceptionMessages.NotFound);

        _applicationDbContext.FooterItems.Remove(footerItem);

        await _applicationDbContext.SaveChangesAsync();

        return footerItem.FooterItemId;
    }

    #endregion

    #region GetFooterItem
    
    public async Task<GetFooterItemResponse> GetFooterItem(Guid footerItemId)
    {
        var footerItem = await _applicationDbContext.FooterItems
                             .FirstOrDefaultAsync(x=> x.FooterItemId == footerItemId)
                         ?? throw new NotFoundException(FooterItemExceptionMessages.NotFound);
        
        return _mapper.Map<GetFooterItemResponse>(footerItem);
        
    }

    #endregion

    #region GetFooterItems

    public async Task<List<GetFooterItemsResponse>> GetFooterItems(Expression<Func<GetFooterItemsResponse, bool>>? predicate = null)
    {
        var footerItemList = _applicationDbContext.FooterItems
            .AsNoTracking();
        
        if (predicate != null)
        {
            var footerItemPredicate = _mapper.MapExpression<Expression<Func<FooterItem, bool>>>(predicate);
            footerItemList = footerItemList.Where(footerItemPredicate);
        }
        
        var footerItems = await footerItemList
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();
        
        var footerItemResponse = _mapper.Map<List<GetFooterItemsResponse>>(footerItems);

        return footerItemResponse;
    }

    #endregion
    
    #region IsExist

    private async Task IsExistOrderNumber(int orderNumber)
    {
        var footerItemOrderNumber = await _applicationDbContext.FooterItems
            .AnyAsync(x => x.OrderNumber == orderNumber);

        if (footerItemOrderNumber)
            throw new ConflictException(FooterItemExceptionMessages.OrderNumberConflict);
    }

    private async Task<bool> IsExistGeneric(Expression<Func<FooterItem, bool>> filter)
    {
        var result = await _applicationDbContext.FooterItems.AnyAsync(filter);

        if (result)
            throw new ConflictException(FooterItemExceptionMessages.Conflict);

        return result;
    }

    private async Task IsExistWhenUpdate(Guid footerItemId, int orderNumber, string label)
    {
        var isExistOrderNumber = await _applicationDbContext.FooterItems
            .AnyAsync(x => x.FooterItemId != footerItemId && x.OrderNumber == orderNumber);

        var isExistFooterItem = await _applicationDbContext.FooterItems
            .AnyAsync(x => x.FooterItemId != footerItemId &&
                           x.Label.Trim() == label.Trim());

        if (isExistOrderNumber)
        {
            throw new ConflictException(FooterItemExceptionMessages.OrderNumberConflict);
        }

        if (isExistFooterItem)
        {
            throw new ConflictException(FooterItemExceptionMessages.Conflict);
        }
    }

    #endregion

    #region ImportFooterItemsFromExcel

    public async Task<List<GetFooterItemsResponse>> ImportFooterItemsFromExcel(Stream fileStream)
    {
        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0]; // İlk sayfayı seçiyoruz
            var rowCount = worksheet.Dimension.Rows;

            var footerItems = new List<FooterItem>(); // Verileri kaydetmek için bir liste

            for (int row = 2; row <= rowCount; row++) // 1. satırda başlıklar olduğu varsayımıyla 2. satırdan başlıyoruz
            {
                await IsExistGeneric(x => x.Label.Trim() == worksheet.Cells[row, 1].Text);

                await IsExistOrderNumber(int.Parse(worksheet.Cells[row, 3].Text));
                
                var footerItem = new FooterItem()
                {
                    Label = worksheet.Cells[row, 1].Text, // 1. sütundaki değer
                    Column = int.Parse(worksheet.Cells[row, 2].Text), // 2. sütundaki değer
                    OrderNumber = int.Parse(worksheet.Cells[row, 3].Text), // 3. sütundaki değer
                    TargetUrl = worksheet.Cells[row, 4].Text, // 4. sütundaki değer
                    IsActive = true
                };
                
                footerItems.Add(footerItem);
            }

            _applicationDbContext.FooterItems.AddRange(footerItems);

            await _applicationDbContext.SaveChangesAsync();
            
            return _mapper.Map<List<GetFooterItemsResponse>>(footerItems);
        }
    }


    #endregion
}