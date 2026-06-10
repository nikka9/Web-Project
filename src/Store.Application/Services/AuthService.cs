using System.Security.Cryptography;
using System.Text;
using Store.Application.DTOs;
using Store.Application.Exceptions;
using Store.Application.Interfaces;
using Store.Domain.Constants;
using Store.Domain.Entities;
using Store.Domain.Interfaces;

namespace Store.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IRepository<ApplicationUser> userRepository,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var username = dto.Username.Trim();
        var normalizedUsername = username.ToLower();
        var normalizedRole = NormalizeRole(dto.Role);

        var existingUser = await _userRepository.FirstOrDefaultAsync(
            user => user.Username.ToLower() == normalizedUsername,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new AppValidationException("Username is already registered.");
        }

        var user = new ApplicationUser
        {
            Username = username,
            Password = HashPassword(dto.Password),
            Role = normalizedRole
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = dto.Username.Trim().ToLower();
        var user = await _userRepository.FirstOrDefaultAsync(
            applicationUser => applicationUser.Username.ToLower() == normalizedUsername,
            cancellationToken);

        if (user is null || user.Password != HashPassword(dto.Password))
        {
            throw new AppValidationException("Invalid username or password.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(ApplicationUser user)
    {
        return new AuthResponseDto
        {
            Username = user.Username,
            Role = user.Role,
            Token = _jwtTokenService.CreateToken(user)
        };
    }

    private static string NormalizeRole(string role)
    {
        var normalizedRole = role.Trim().ToLowerInvariant();

        return normalizedRole switch
        {
            Roles.Admin => Roles.Admin,
            Roles.User => Roles.User,
            _ => throw new AppValidationException("Role must be 'admin' or 'user'.")
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
