using MediatR;
using sattec.Identity.Application.Common.Interfaces;
using sattec.Identity.Application.Common.Models;

namespace sattec.Identity.Application.Users.Commands.FinalizeRegistration
{
    public record FinalizeRegistrationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
    public class FinalizeRegistrationCommandHandler : IRequestHandler<FinalizeRegistrationCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        public FinalizeRegistrationCommandHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }
        public async Task<Result> Handle(FinalizeRegistrationCommand request, CancellationToken cancellationToken)
        {
            var user = _identityService.GetUserNameAsync(request.Id.ToString());
            if (user == null)

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
            
        }
    }
}
