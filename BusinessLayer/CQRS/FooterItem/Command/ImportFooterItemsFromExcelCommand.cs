using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Interface;

namespace BusinessLayer.CQRS.FooterItem.Command;

    public sealed record ImportFooterItemsFromExcelCommand(IFormFile File) : IRequest<Unit>
    {
        public class ImportFooterItemsFromExcelCommandHandler : IRequestHandler<ImportFooterItemsFromExcelCommand, Unit>
        {
            private readonly IFooterItemRepository _footerItemRepository;

            public ImportFooterItemsFromExcelCommandHandler(IFooterItemRepository footerItemRepository)
            {
                _footerItemRepository = footerItemRepository;
            }

            public async Task<Unit> Handle(ImportFooterItemsFromExcelCommand request, CancellationToken cancellationToken)
            {
                using (var stream = new MemoryStream())
                {
                    await request.File.CopyToAsync(stream, cancellationToken);
                    await _footerItemRepository.ImportFooterItemsFromExcel(stream);
                }

                return Unit.Value;
            }
        }
    }