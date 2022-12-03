using FluentValidation;

namespace sattec.Identity.Application.Users.Commands.InsuranceInformation.CreateInsuranceInformation
{
    public class CreateInsuranceInformationCommandValidator : AbstractValidator<CreateInsuranceInformationCommand>
    {
        public CreateInsuranceInformationCommandValidator()
        {
            RuleFor(v => v.InsuranceNumber)
              .Length(8)
              .NotEmpty();
        }
    }
}
