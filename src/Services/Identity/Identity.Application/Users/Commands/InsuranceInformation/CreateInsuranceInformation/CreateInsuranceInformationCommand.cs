using MediatR;
using sattec.Identity.Application.Common.Interfaces;
using sattec.Identity.Application.Common.Models;

namespace sattec.Identity.Application.Users.Commands.InsuranceInformation.CreateInsuranceInformation
{
    public record CreateInsuranceInformationCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public bool IsInsurancse { get; set; }
        public string name { get; set; }
        public string InsuranceNumber { get; set; }
    }
    public class CreateInsuranceInformationCommandHandler : IRequestHandler<CreateInsuranceInformationCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        public CreateInsuranceInformationCommandHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }
        public async Task<Result> Handle(CreateInsuranceInformationCommand request, CancellationToken cancellationToken)
        {
            if (request.IsInsurancse == true)
            {
                var user = _identityService.CreateInsuranceInfo(request.UserId.ToString(), request.name, request.InsuranceNumber);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
