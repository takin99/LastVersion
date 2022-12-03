using FluentValidation;

namespace sattec.Identity.Application.Users.Commands.ConfirmMobile
{
    public class MobileConfirmCommandValidator : AbstractValidator<MobileConfirmCommand>
    {
        public MobileConfirmCommandValidator()
        {
          RuleFor(v => v.Code)
         .MinimumLength(6)
         .NotEmpty();
        }
    }
}
