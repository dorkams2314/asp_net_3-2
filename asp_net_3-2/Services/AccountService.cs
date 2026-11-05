using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using asp_net_3_2.Domain;
using asp_net_3_2.DTOs;
using asp_net_3_2.DTOs.Auth;
using asp_net_3_2.DTOs.Users;
using asp_net_3_2.Models;
using asp_net_3_2.Repositories;
using asp_net_3_2.Security;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace asp_net_3_2.Services;

public sealed class AccountService(IAccountRepository repository, IPasswordHasher<User> hasher,
    IMapper mapper, IOptions<JwtOptions> options) : IAccountService
{
    public async Task<AuthResponse> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
    {
        var login = dto.Login.ToLowerInvariant();
        if (await repository.ExistsAsync(user => user.Login == login, cancellationToken))
        {
            throw new ConflictException("This login is already taken.");
        }

        var user = new User
        {
            Login = login,
            UserName = dto.UserName.Trim(),
            IsActive = true,
            Role = await repository.GetUserRoleAsync(cancellationToken)
        };
        user.PasswordHash = hasher.HashPassword(user, dto.Password);
        repository.Add(user);
        await repository.SaveChangesAsync(cancellationToken);

        return IssueToken(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
    {
        var user = await repository.FindByLoginAsync(dto.Login.ToLowerInvariant(), cancellationToken);
        if (user is null || !user.IsActive || user.PasswordHash.StartsWith('$'))
        {
            return null;
        }

        PasswordVerificationResult result;
        try
        {
            result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        }
        catch (FormatException)
        {
            return null;
        }

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = hasher.HashPassword(user, dto.Password);
            await repository.SaveChangesAsync(cancellationToken);
        }

        return IssueToken(user);
    }

    public async Task<UserDto> GetAsync(int id, CancellationToken cancellationToken) =>
        mapper.Map<UserDto>(await repository.FindAccountAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException($"User {id} was not found."));

    public async Task<PageResult<UserDto>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var result = await repository.GetPageAsync(page, pageSize, cancellationToken);
        return new PageResult<UserDto>(mapper.Map<List<UserDto>>(result.Items), result.TotalCount);
    }

    public async Task UpdateProfileAsync(int id, UpdateProfileDto dto, CancellationToken cancellationToken)
    {
        var user = await repository.FindAccountAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException($"User {id} was not found.");
        user.UserName = dto.UserName.Trim();
        await repository.SaveChangesAsync(cancellationToken);
    }

    private AuthResponse IssueToken(User user)
    {
        var jwt = options.Value;
        var expires = DateTime.UtcNow.AddMinutes(jwt.LifetimeMinutes);
        var token = new JwtSecurityToken(jwt.Issuer, jwt.Audience,
            [new Claim("sub", user.Id.ToString()), new Claim("role", user.Role.Name),
                new Claim("jti", Guid.NewGuid().ToString())],
            notBefore: DateTime.UtcNow, expires: expires,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                SecurityAlgorithms.HmacSha256));

        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), expires, mapper.Map<UserDto>(user));
    }
}
