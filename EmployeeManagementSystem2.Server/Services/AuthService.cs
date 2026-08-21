using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagementSystem2.Server.Data;
using EmployeeManagementSystem2.Server.DTOs;
using EmployeeManagementSystem2.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagementSystem2.Server.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = _context.AuthLoginSpResults
            .FromSqlRaw("EXEC dbo.usp_Auth_LoginByEmail @Email", new SqlParameter("@Email", request.Email))
            .AsNoTracking()
            .AsEnumerable()
            .FirstOrDefault();

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return null;
        }

        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE dbo.Users SET LastLoginDate = {DateTime.UtcNow} WHERE Id = {user.UserId}",
            cancellationToken);

        var tokenUser = new User
        {
            Id = user.UserId,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash
        };

        var token = GenerateJwtToken(tokenUser);
        return new AuthResponse(token, user.Username, user.Email, user.UserId, user.EmployeeId);
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var passwordHash = HashPassword(request.Password);

        try
        {
            var result = await _context.AuthRegisterSpResults
                .FromSqlRaw(
                    "EXEC dbo.usp_Auth_RegisterUser @Username, @Email, @PasswordHash, @FirstName, @LastName, @DepartmentId, @RoleId",
                    new SqlParameter("@Username", request.Username),
                    new SqlParameter("@Email", request.Email),
                    new SqlParameter("@PasswordHash", passwordHash),
                    new SqlParameter("@FirstName", request.FirstName),
                    new SqlParameter("@LastName", request.LastName),
                    new SqlParameter("@DepartmentId", request.DepartmentId),
                    new SqlParameter("@RoleId", request.RoleId))
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return null;
            }

            var tokenUser = new User
            {
                Id = result.UserId,
                Username = result.Username,
                Email = result.Email,
                PasswordHash = passwordHash
            };

            var token = GenerateJwtToken(tokenUser);
            return new AuthResponse(token, result.Username, result.Email, result.UserId, result.EmployeeId);
        }
        catch (DbUpdateException)
        {
            return null;
        }
        catch (SqlException)
        {
            return null;
        }
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
