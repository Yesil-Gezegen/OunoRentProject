using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Channel.Request;
using Shared.DTO.Channel.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class ChannelRepository : IChannelRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    private IImageService _imageService;

    public ChannelRepository(ApplicationDbContext applicationDbContext, IMapper mapper, IImageService imageService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _imageService = imageService;
    }

    public async Task<ChannelResponse> CreateChannel(CreateChannelRequest createChannelRequest)
    {
        var channel = new Channel();

        channel.Name = createChannelRequest.Name.Trim();
        channel.Logo = await _imageService.SaveImageAsync(createChannelRequest.Logo);
        channel.IsActive = createChannelRequest.IsActive;

        _applicationDbContext.Channels.Add(channel);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<ChannelResponse>(channel);
    }

    public async Task<List<GetChannelsResponse>> GetChannels(Expression<Func<GetChannelsResponse, bool>>? predicate = null)
    {
        var channels = _applicationDbContext.Channels
            .AsNoTracking();

        if (predicate != null)
        {
            var channelPredicate = _mapper.MapExpression<Expression<Func<Channel, bool>>>(predicate);
            channels = channels.Where(channelPredicate);
        }
        
        var channelList = await channels
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();
        
        var channelResponse = _mapper.Map<List<GetChannelsResponse>>(channelList);

        return channelResponse;
    }
    
    public async Task<GetChannelResponse> GetChannel(Guid channelId)
    {
        var channel = await _applicationDbContext.Channels
                          .FirstOrDefaultAsync(x => x.ChannelId == channelId)
                      ?? throw new NotFoundException(ChannelExceptionMessages.NotFound);
        
        return _mapper.Map<GetChannelResponse>(channel);
    }

    public async Task<ChannelResponse> UpdateChannel(UpdateChannelRequest updateChannelRequest)
    {
        var channel = await _applicationDbContext.Channels
                          .FirstOrDefaultAsync(x => x.ChannelId == updateChannelRequest.ChannelId)
                      ?? throw new NotFoundException(ChannelExceptionMessages.NotFound);

        channel.Name = updateChannelRequest.Name.Trim();
        channel.IsActive = updateChannelRequest.IsActive;

        if (updateChannelRequest.Logo != null)
        {
            await _imageService.DeleteImageAsync(channel.Logo);
            channel.Logo = await _imageService.SaveImageAsync(updateChannelRequest.Logo);
        }

        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<ChannelResponse>(channel);
    }

    public async Task<Guid> DeleteChannel(Guid channelId)
    {
        var channel = await _applicationDbContext.Channels
                          .FirstOrDefaultAsync(x => x.ChannelId == channelId)
                      ?? throw new NotFoundException(ChannelExceptionMessages.NotFound);

        _applicationDbContext.Channels.Remove(channel);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return channelId;
    }
}