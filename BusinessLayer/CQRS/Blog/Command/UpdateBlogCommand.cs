using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared.DTO.Blog.Request;
using Shared.DTO.Blog.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Blog.Command;

public sealed record UpdateBlogCommand(UpdateBlogRequest UpdateBlogRequest) : IRequest<BlogResponse>
{
    internal class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, BlogResponse>
    {
        private readonly IBlogRepository _blogRepository;

        public UpdateBlogCommandHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<BlogResponse> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            return await _blogRepository.UpdateBlog(request.UpdateBlogRequest);
        }
    }
}
