using MediatR;
using Microsoft.AspNetCore.Http;
using sattec.Identity.Application.Common.Interfaces;
using sattec.Identity.Application.Common.Models;

namespace sattec.Identity.Application.Users.Commands.BrandInformation.CreateBrandInformation
{
    public record CreateBrandInformationCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public string BrandName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public IFormFile Logo { get; set; }
        public string BrandOwner { get; set; }
    }
    public class CreateBrandInformationCommandHandler : IRequestHandler<CreateBrandInformationCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public CreateBrandInformationCommandHandler(IIdentityService identityService, IApplicationDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }
        public async Task<Result> Handle(CreateBrandInformationCommand request, CancellationToken cancellationToken)
        {
            var user =
                _identityService.CreateBrandInformation
                (request.UserId.ToString(),
                request.BrandName,
                request.Address,
                request.PhoneNumber,
                request.RegistrationNumber,
                request.Logo,
                request.BrandOwner);

            await _context.SaveChangesAsync(cancellationToken);

            return user;
        }
    }
}
