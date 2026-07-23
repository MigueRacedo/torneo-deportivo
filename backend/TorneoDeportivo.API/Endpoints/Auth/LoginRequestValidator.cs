using FastEndpoints;
using FluentValidation;

namespace TorneoDeportivo.API.Endpoints.Auth;

/// <summary>
/// Valida que el <see cref="LoginRequest"/> tenga un email con formato válido y una contraseña no vacía.
/// </summary>
public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
