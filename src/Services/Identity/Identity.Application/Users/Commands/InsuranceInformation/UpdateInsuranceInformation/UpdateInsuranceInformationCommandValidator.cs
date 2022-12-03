using FluentValidation;

namespace sattec.Identity.Application.Users.Commands.InsuranceInformation.UpdateInsuranceInformation
{
    public class UpdateInsuranceInformationCommandValidator : AbstractValidator<UpdateInsuranceInformationCommand>
    {
        public UpdateInsuranceInformationCommandValidator()
        {
            RuleFor(v => v.InsuranceNumber)
             .Length(8)
             .NotEmpty();
        }
    }
}
