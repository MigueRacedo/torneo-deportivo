using FluentValidation;

namespace TorneoDeportivo.Application.Features.Auth;

/// <summary>
/// Valida que el <see cref="LoginCommand"/> tenga un email con formato válido y una contraseña no vacía.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
