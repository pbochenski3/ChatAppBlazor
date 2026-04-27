using ChatApp.Application.DTO;
using ChatApp.Application.Feature.Auth.RefreshToken;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Interfaces.Repository;
using ChatApp.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return null;
        var storedToken = await _userRepository.GetRefreshTokenWithUserAsync(request.RefreshToken);
        if (storedToken == null || !storedToken.IsActive)
        {
            return null; 
        }
        storedToken.Revoked = DateTime.UtcNow;
        storedToken.RevokedByIp = request.IpAddress;
        var user = storedToken.User;
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        var newRefreshToken = UserRefreshToken.Create(
        user.UserID,
        newRefreshTokenValue,
        request.IpAddress,
        request.RefreshToken
    );
        await _userRepository.AddRefreshTokenAsync(newRefreshToken);
        return AuthResponse.CreateResponse(user, newAccessToken, newRefreshTokenValue);
    }
}
