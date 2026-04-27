using ChatApp.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Auth.RefreshToken
{
    public record RefreshTokenCommand(string? RefreshToken, string? IpAddress) : IRequest<AuthResponse?>;
}
