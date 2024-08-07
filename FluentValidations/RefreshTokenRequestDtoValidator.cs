using FluentValidation;
using gateway.api.Models.Token;

namespace gateway.api.FluentValidations
{
    public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Please enter a valid user ID")
                .NotNull().WithMessage("Please enter a valid user ID");

            RuleFor(x => x.RefreshToken)
                .NotEqual(Guid.Empty);
        }
    }
}
