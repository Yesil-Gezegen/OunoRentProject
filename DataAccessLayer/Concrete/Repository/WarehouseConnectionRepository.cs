using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.WarehouseConnection.Request;
using Shared.DTO.WarehouseConnection.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class WarehouseConnectionRepository : IWarehouseConnectionRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public WarehouseConnectionRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateWarehouseConnection

    public async Task<WarehouseConnectionResponse> CreateWarehouseConnection(
        CreateWarehouseConnectionRequest createWarehouseConnectionRequest)
    {
        await IsExistGeneric(x=> x.WarehouseId == createWarehouseConnectionRequest.WarehouseId &&
                                 x.ChannelId == createWarehouseConnectionRequest.ChannelId);
        
        var warehouseConnection = new WarehouseConnection();

        warehouseConnection.WarehouseId = createWarehouseConnectionRequest.WarehouseId;
        warehouseConnection.ChannelId = createWarehouseConnectionRequest.ChannelId;
        warehouseConnection.IsActive = createWarehouseConnectionRequest.IsActive;

        _applicationDbContext.WarehouseConnections.Add(warehouseConnection);

        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<WarehouseConnectionResponse>(warehouseConnection);
    }



    #endregion

    #region GetWarehouseConnections
    
    public async Task<List<GetWarehouseConnectionsResponse>> GetWarehouseConnections(
        Expression<Func<GetWarehouseConnectionsResponse, bool>>? predicate = null)
    {
        var warehouseConnections = _applicationDbContext.WarehouseConnections
            .Include(x => x.Warehouse)
            .Include(x => x.Channel)
            .AsNoTracking();

        if (predicate != null)
        {
            var warehouseConnectionPredicate = _mapper.MapExpression<Expression<Func<WarehouseConnection, bool>>>(predicate);
            warehouseConnections = warehouseConnections.Where(warehouseConnectionPredicate);
        }

        var warehouseConnectionList = await warehouseConnections
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .Select(x => new GetWarehouseConnectionsResponse
            {
                WarehouseConnectionId = x.WarehouseConnectionId,
                WarehouseName = x.Warehouse.Name,
                ChannelName = x.Channel.Name,
                IsActive = x.IsActive
            }).ToListAsync();

        return warehouseConnectionList;
    }

    #endregion

    #region GetWarehouseConnection

    public async Task<GetWarehouseConnectionResponse> GetWarehouseConnection(Guid warehouseConnectionId)
    {
        var warehouseConnection = await _applicationDbContext.WarehouseConnections
                                      .Include(x => x.Warehouse)
                                      .Include(x => x.Channel)
                                      .AsNoTracking()
                                      .Where(x => x.WarehouseConnectionId == warehouseConnectionId)
                                      .Select(x => new GetWarehouseConnectionResponse
                                      {
                                          WarehouseConnectionId = x.WarehouseConnectionId,
                                          WarehouseName = x.Warehouse.Name,
                                          ChannelName = x.Channel.Name,
                                          IsActive = x.IsActive
                                      }).FirstOrDefaultAsync()
                                  ?? throw new NotFoundException(WarehouseConnectionExceptionMessages.NotFound);

        return warehouseConnection;
    }

    #endregion

    #region UpdateWarehouseConnection

    public async Task<WarehouseConnectionResponse> UpdateWarehouseConnection(
        UpdateWarehouseConnectionRequest updateWarehouseConnectionRequest)
    {
        var warehouseConnection = await _applicationDbContext.WarehouseConnections
                                      .Include(x => x.Warehouse)
                                      .Include(x => x.Channel)
                                      .Where(x => x.WarehouseConnectionId ==
                                                  updateWarehouseConnectionRequest.WarehouseConnectionId)
                                      .FirstOrDefaultAsync()
                                  ?? throw new NotFoundException(WarehouseConnectionExceptionMessages.NotFound);

        warehouseConnection.WarehouseId = updateWarehouseConnectionRequest.WarehouseId;
        warehouseConnection.ChannelId = updateWarehouseConnectionRequest.ChannelId;
        warehouseConnection.IsActive = updateWarehouseConnectionRequest.IsActive;

        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<WarehouseConnectionResponse>(warehouseConnection);
    }

    #endregion

    #region DeleteWarehouseConnection

    public async Task<Guid> DeleteWarehouseConnection(Guid warehouseConnectionId)
    {
        var warehouseConnection = await _applicationDbContext.WarehouseConnections
                                      .Where(x => x.WarehouseConnectionId == warehouseConnectionId)
                                      .FirstOrDefaultAsync()
                                  ?? throw new NotFoundException(WarehouseConnectionExceptionMessages.NotFound);

        _applicationDbContext.WarehouseConnections.Remove(warehouseConnection);

        await _applicationDbContext.SaveChangesAsync();

        return warehouseConnectionId;
    }

    #endregion

    #region IsExist

    private async Task<bool> IsExistGeneric(Expression<Func<WarehouseConnection, bool>> filter)
    {
        var result = await _applicationDbContext.WarehouseConnections.AnyAsync(filter);

        if (result)
            throw new ConflictException(WarehouseConnectionExceptionMessages.Conflict);

        return result;
    }
    
    #endregion
}