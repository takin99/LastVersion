using MediatR;
using Microsoft.AspNetCore.Http;
using sattec.Identity.Application.Common.Interfaces;
using sattec.Identity.Application.Common.Models;

namespace sattec.Identity.Application.Users.Commands.DocumentInformation.CreateDocumentInfo
{
    public record CreateDocumentInfoCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
    }
    public class CreateDocumentInfoCommandHandler : IRequestHandler<CreateDocumentInfoCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        public CreateDocumentInfoCommandHandler(IIdentityService identityService, IApplicationDbContext context, IFileService fileService)
        {
            _identityService = identityService;
            _context = context;
        }
        public async Task<Result> Handle(CreateDocumentInfoCommand request, CancellationToken cancellationToken)
        {
            var result =  _identityService.CreateDocument(request.UserId.ToString(), request.Description, request.File);

            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
