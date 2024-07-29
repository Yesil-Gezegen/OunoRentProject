using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.Blog.Command;

public sealed record DeleteBlogCommand(Guid BlogId) : IRequest<Guid>
{
    internal class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand, Guid>
    {
        private readonly IBlogRepository _blogRepository;

        public DeleteBlogCommandHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<Guid> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
        {
            return await _blogRepository.DeleteBlog(request.BlogId);
        }
    }
}
