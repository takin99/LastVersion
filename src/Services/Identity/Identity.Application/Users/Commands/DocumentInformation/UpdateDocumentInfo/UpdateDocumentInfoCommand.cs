using MediatR;
using Microsoft.AspNetCore.Http;
using sattec.Identity.Application.Common.Interfaces;
using sattec.Identity.Application.Common.Models;

namespace sattec.Identity.Application.Users.Commands.DocumentInformation.UpdateDocumentInfo
{
    public record UpdateDocumentInfoCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
    }
    public class UpdateDocumentInfoCommandHandler : IRequestHandler<UpdateDocumentInfoCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        public UpdateDocumentInfoCommandHandler(IIdentityService identityService, IApplicationDbContext context, IFileService fileService)
        {
            _identityService = identityService;
            _context = context;
        }
        public async Task<Result> Handle(UpdateDocumentInfoCommand request, CancellationToken cancellationToken)
        {
            var result = _identityService.UpdateDocument(request.UserId.ToString(), request.Description, request.File);

            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
