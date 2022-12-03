using MediatR;
using sattec.Identity.Application.Common.Interfaces;
using sattec.Identity.Application.Common.Models;

namespace sattec.Identity.Application.Users.Commands.EmailConfirm
{
    public record EmailConfirmCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }
    public class EmailConfrimCommandHandler : IRequestHandler<EmailConfirmCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;


        public EmailConfrimCommandHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }
        public async Task<Result> Handle(EmailConfirmCommand request, CancellationToken cancellationToken)
        {
            var result =  _identityService.GetByEmailVerification(request.Email);

            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
