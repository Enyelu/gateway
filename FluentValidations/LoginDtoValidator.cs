using FluentValidation;
using gateway.api.Models.Login;

namespace gateway.api.FluentValidations
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress().WithMessage("Must be a valid email address");

            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull().WithMessage("Email cannot be null/empty");
        }
    }
}