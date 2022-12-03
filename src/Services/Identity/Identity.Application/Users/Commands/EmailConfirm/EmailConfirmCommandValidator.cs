using FluentValidation; 

namespace sattec.Identity.Application.Users.Commands.EmailConfirm
{
    public class EmailConfirmCommandValidator : AbstractValidator<EmailConfirmCommand>
    {
        public EmailConfirmCommandValidator()
        {

        }
    }
}
