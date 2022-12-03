using FluentValidation;

namespace sattec.Identity.Application.Users.Commands.BrandInformation.CreateBrandInformation
{
    internal class CreateBrandInformationCommandValidator : AbstractValidator<CreateBrandInformationCommand>
    {
        public CreateBrandInformationCommandValidator()
        {
            RuleFor(v => v.PhoneNumber)
               .Length(11)
               .NotEmpty();
        }
    }
}
