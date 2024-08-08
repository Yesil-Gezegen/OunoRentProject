using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Warehouse.Request;
using Shared.DTO.Warehouse.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public WarehouseRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateWarehouse

    public async Task<WarehouseResponse> CreateWarehouse(CreateWarehouseRequest createWarehouseRequest)
    {
        await IsExistGeneric(x=> x.Name.Trim() == createWarehouseRequest.Name.Trim());
        
        var warehouse = new Warehouse();
        
        warehouse.Name = createWarehouseRequest.Name.Trim();
        warehouse.LogoWarehouseId = createWarehouseRequest.LogoWarehouseId;
        warehouse.IsActive = createWarehouseRequest.IsActive;
        
        _applicationDbContext.Warehouses.Add(warehouse);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<WarehouseResponse>(warehouse);
    }

 

    #endregion

    #region GetWarehouses

    public async Task<List<GetWarehousesResponse>> GetWarehouses(
        Expression<Func<GetWarehousesResponse, bool>>? predicate = null)
    {
        var warehouses = _applicationDbContext.Warehouses
            .AsNoTracking();

        if (predicate != null)
        {
            var warehousePredicate = _mapper.MapExpression<Expression<Func<Warehouse, bool>>>(predicate);
            warehouses = warehouses.Where(warehousePredicate);
        }
        
        var warehouseList = await warehouses
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();
        
        var warehouseResponse = _mapper.Map<List<GetWarehousesResponse>>(warehouseList);

        return warehouseResponse;
    }
    
    #endregion

    #region GetWarehouse

    public async Task<GetWarehouseResponse> GetWarehouse(Guid warehouseId)
    {
        var warehouse = await _applicationDbContext.Warehouses
                            .FirstOrDefaultAsync(x => x.WarehouseId == warehouseId)
                        ?? throw new NotFoundException(WarehouseExceptionMessages.NotFound);
        
        return _mapper.Map<GetWarehouseResponse>(warehouse);
    }

    #endregion

    #region UpdateWarehouse

    public async Task<WarehouseResponse> UpdateWarehouse(UpdateWarehouseRequest updateWarehouseRequest)
    {
        await IsExistGeneric(x=> x.WarehouseId != updateWarehouseRequest.WarehouseId &&
                                 x.Name.Trim() == updateWarehouseRequest.Name.Trim());
        
        var warehouse = await _applicationDbContext.Warehouses
                            .FirstOrDefaultAsync(x => x.WarehouseId == updateWarehouseRequest.WarehouseId)
                        ?? throw new NotFoundException(WarehouseExceptionMessages.NotFound);
        
        warehouse.Name = updateWarehouseRequest.Name.Trim();
        warehouse.LogoWarehouseId = updateWarehouseRequest.LogoWarehouseId;
        warehouse.IsActive = updateWarehouseRequest.IsActive;
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<WarehouseResponse>(warehouse);
    }

    #endregion

    #region DeleteWarehouse

    public async Task<Guid> DeleteWarehouse(Guid warehouseId)
    {
        var warehouse = await _applicationDbContext.Warehouses
                            .FirstOrDefaultAsync(x => x.WarehouseId == warehouseId)
                        ?? throw new NotFoundException(WarehouseExceptionMessages.NotFound);
        
        _applicationDbContext.Warehouses.Remove(warehouse);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return warehouseId;
    }


    #endregion

    #region IsExist
    
    private async Task<bool> IsExistGeneric(Expression<Func<Warehouse, bool>> filter)
    {
        var result = await _applicationDbContext.Warehouses.AnyAsync(filter);

        if (result)
            throw new ConflictException(WarehouseExceptionMessages.Conflict);

        return result;
    }

    
    #endregion
}