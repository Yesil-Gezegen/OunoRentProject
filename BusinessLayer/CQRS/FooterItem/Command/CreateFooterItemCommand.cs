using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.DTO.FooterItem.Request;
using Shared.DTO.FooterItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FooterItem.Command;

public sealed record CreateFooterItemCommand(CreateFooterItemRequest CreateFooterItemRequest) : IRequest<FooterItemResponse>;

internal class CreateFooterItemCommandHandler : IRequestHandler<CreateFooterItemCommand , FooterItemResponse>
{
    private readonly IFooterItemRepository _footerItemRepository;
    private readonly IMapper _mapper;

    public CreateFooterItemCommandHandler(IFooterItemRepository footerItemRepository, IMapper mapper)
    {
        _footerItemRepository = footerItemRepository;
        _mapper = mapper;
    }

    public async Task<FooterItemResponse> Handle(CreateFooterItemCommand request, CancellationToken cancellationToken)
    {
        var footerItem = await _footerItemRepository.CreateFooterItem(request.CreateFooterItemRequest);
        
        var footerItemResponse = _mapper.Map<FooterItemResponse>(footerItem);
        
        return footerItemResponse;
    }
}
