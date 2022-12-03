using MediatR;
using Microsoft.AspNetCore.Http;
using sattec.Identity.Application.Common.Interfaces;
using sattec.Identity.Application.Common.Models;

namespace sattec.Identity.Application.Users.Commands.BrandInformation.UpdateBrandInformationCommand
{
    public record UpdateBrandInformationCommand : IRequest<Result>
    {
        public Guid userId { get; set; }
        public string BrandName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public IFormFile Logo { get; set; }
        public string BrandOwner { get; set; }
    }
    public class UpdateBrandInformationCommandHandler : IRequestHandler<UpdateBrandInformationCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        public UpdateBrandInformationCommandHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }
        public async Task<Result> Handle(UpdateBrandInformationCommand request, CancellationToken cancellationToken)
        {
            var user = _identityService.UpdateBrandInformation
               (request.userId.ToString(),
               request.BrandName,
               request.Address,
               request.PhoneNumber,
               request.RegistrationNumber,
               request.Logo,
               request.BrandOwner
               );

            await _context.SaveChangesAsync(cancellationToken);

            return user;
        }
    }
}
