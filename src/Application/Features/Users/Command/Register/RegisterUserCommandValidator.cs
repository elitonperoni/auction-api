using FluentValidation;

namespace Application.Features.Users.Command.Register;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.FullName).NotEmpty();
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
    }
}
