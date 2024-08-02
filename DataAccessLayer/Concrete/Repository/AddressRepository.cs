using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Address.Request;
using Shared.DTO.Address.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public AddressRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateAddress

    public async Task<AddressResponse> CreateAddressAsync(CreateAddressRequest createAddressRequest)
    {
        await HasConflict(createAddressRequest);

        var address = new Address();

        address.UserId = createAddressRequest.UserId;
        address.Type = createAddressRequest.Type;
        address.Title = createAddressRequest.Title.Trim();
        address.City = createAddressRequest.City.Trim();
        address.District = createAddressRequest.District.Trim();
        address.Neighborhood = createAddressRequest.Neighborhood.Trim();
        address.AddressDetail = createAddressRequest.AddressDetail.Trim();

        if (createAddressRequest.TaxNo.HasValue &&
            !string.IsNullOrEmpty(createAddressRequest.TaxOffice) &&
            !string.IsNullOrEmpty(createAddressRequest.CompanyName))
        {
            address.TaxNo = createAddressRequest.TaxNo.Value;
            address.TaxOffice = createAddressRequest.TaxOffice.Trim();
            address.CompanyName = createAddressRequest.CompanyName.Trim();
        }

        await _applicationDbContext.Addresses.AddAsync(address);
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<AddressResponse>(address);
    }

    #endregion

    #region UpdateAddress

    public async Task<AddressResponse> UpdateAddressAsync(UpdateAddressRequest updateAddressRequest)
    {
        var address =
            await _applicationDbContext.Addresses.FirstOrDefaultAsync(
                a => a.AddressId == updateAddressRequest.AddressId) ??
            throw new NotFoundException(AddressExceptionMessages.NotFound);

        await HasConflict(updateAddressRequest);

        address.UserId = updateAddressRequest.UserId;
        address.Type = updateAddressRequest.Type;
        address.Title = updateAddressRequest.Title.Trim();
        address.City = updateAddressRequest.City.Trim();
        address.District = updateAddressRequest.District.Trim();
        address.Neighborhood = updateAddressRequest.Neighborhood.Trim();
        address.AddressDetail = updateAddressRequest.AddressDetail.Trim();

        if (updateAddressRequest.TaxNo.HasValue &&
            !string.IsNullOrEmpty(updateAddressRequest.TaxOffice) &&
            !string.IsNullOrEmpty(updateAddressRequest.CompanyName))
        {
            address.TaxNo = updateAddressRequest.TaxNo.Value;
            address.TaxOffice = updateAddressRequest.TaxOffice.Trim();
            address.CompanyName = updateAddressRequest.CompanyName.Trim();
        }

        _applicationDbContext.Addresses.Update(address);
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<AddressResponse>(address);
    }

    #endregion

    #region DeleteAddress

    public async Task<AddressResponse> DeleteAddressAsync(Guid addressId)
    {
        var address =
            await _applicationDbContext.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId) ??
            throw new NotFoundException(AddressExceptionMessages.NotFound);

        _applicationDbContext.Addresses.Remove(address);
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<AddressResponse>(address);
    }

    #endregion

    #region GetAddresses

    public async Task<List<GetAddressesResponse>> GetAddressesAsync(
        Expression<Func<GetAddressResponse, bool>>? predicate = null)
    {
        var addresses = _applicationDbContext.Addresses
            .Include(a => a.User)
            .AsNoTracking();

        if (predicate != null)
        {
            var addressPredicate = _mapper.MapExpression<Expression<Func<Address, bool>>>(predicate);
            addresses = addresses.Where(addressPredicate);
        }

        var addressesList = await addresses
            .AsNoTracking()
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        return _mapper.Map<List<GetAddressesResponse>>(addressesList);
    }

    #endregion

    #region GetAddress

    public async Task<GetAddressResponse> GetAddressAsync(Guid addressId)
    {
        var address =
            await _applicationDbContext.Addresses
                .Include(a => a.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                a => a.AddressId == addressId) ??
            throw new NotFoundException(AddressExceptionMessages.NotFound);

        return _mapper.Map<GetAddressResponse>(address);
    }

    #endregion

    #region IsExist

    public async Task<bool> IsExist(Expression<Func<Address, bool>> expression)
    {
        return await _applicationDbContext.Addresses.AnyAsync(expression);
    }

    #endregion

    #region HasConflict

    private async Task HasConflict(CreateAddressRequest createAddressRequest)
    {
        var isUserExist = await _applicationDbContext.Users.AnyAsync(u => u.Id == createAddressRequest.UserId);

        var isTitleExistForUser = await _applicationDbContext.Addresses.AnyAsync(a =>
            a.Title == createAddressRequest.Title && a.UserId == createAddressRequest.UserId);

        if (!isUserExist)
            throw new NotFoundException(UserExceptionMessage.NotFound);

        if (isTitleExistForUser)
            throw new ConflictException(AddressExceptionMessages.TitleConflict);
    }

    private async Task HasConflict(UpdateAddressRequest updateAddressRequest)
    {
        var isUserExist = await _applicationDbContext.Users.AnyAsync(u => u.Id == updateAddressRequest.UserId);

        var isTitleExistForUser = await _applicationDbContext.Addresses.AnyAsync(a =>
            a.Title == updateAddressRequest.Title &&
            a.UserId == updateAddressRequest.UserId &&
            a.AddressId != updateAddressRequest.AddressId);

        if (!isUserExist)
            throw new NotFoundException(UserExceptionMessage.NotFound);

        if (isTitleExistForUser)
            throw new ConflictException(AddressExceptionMessages.TitleConflict);
    }

    #endregion
}