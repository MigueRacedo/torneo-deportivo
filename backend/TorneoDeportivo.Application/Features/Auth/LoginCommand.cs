using MediatR;

namespace TorneoDeportivo.Application.Features.Auth;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
