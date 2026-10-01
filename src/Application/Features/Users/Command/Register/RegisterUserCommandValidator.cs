using System.Globalization;
using FluentValidation;

namespace Application.Features.Users.Command.Register;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.FullName).NotEmpty().MaximumLength(200);
        RuleFor(c => c.UserName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
        RuleFor(c => c.Country).NotEmpty();
        RuleFor(c => c.State).NotEmpty();
        RuleFor(c => c.City).NotEmpty();
        RuleFor(c => c.Timezone).NotEmpty();
        RuleFor(c => c.Language)
            .NotEmpty()
            .Must(language => int.TryParse(language, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            .WithMessage("'Language' must be a numeric language id.");
    }
}
