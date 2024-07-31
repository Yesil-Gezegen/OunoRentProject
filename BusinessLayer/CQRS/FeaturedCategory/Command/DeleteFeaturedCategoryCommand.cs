using MediatR;
using Shared.DTO.FeaturedCategories.Request;
using Shared.Interface;

namespace BusinessLayer.CQRS.FeaturedCategory.Command;

public sealed record DeleteFeaturedCategoryCommand(Guid FeaturedCategoryId)
    : IRequest<Guid>;

public class DeleteFeaturedCategoryCommandHandler : IRequestHandler<DeleteFeaturedCategoryCommand, Guid>
{
    private readonly IFeaturedCategoryRepository _featuredCategoryRepository;

    public DeleteFeaturedCategoryCommandHandler(IFeaturedCategoryRepository featuredCategoryRepository)
    {
        _featuredCategoryRepository = featuredCategoryRepository;
    }

    public async Task<Guid> Handle(DeleteFeaturedCategoryCommand request, CancellationToken cancellationToken)
    {
        var result =
            await _featuredCategoryRepository.DeleteFeaturedCategoryAsync(request.FeaturedCategoryId);
        return result;
    }
}