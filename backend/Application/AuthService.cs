using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Letiao.ATS.Api.Domain.Entities;
using Letiao.ATS.Api.Repository;
using Microsoft.IdentityModel.Tokens;

namespace Letiao.ATS.Api.Application;

/// <summary>
/// 登录请求 DTO
/// </summary>
public record LoginRequest(string Username, string Password);

/// <summary>
/// 登录成功返回 DTO
/// </summary>
public record LoginResult(string Token, string RealName, List<string> Roles);

/// <summary>
/// 认证业务服务：密码校验、JWT 签发
/// </summary>
public class AuthService(UserRepository userRepo, IConfiguration config)
{
    /// <summary>
    /// 登录验证，成功返回 JWT Token
    /// </summary>
    public async Task<LoginResult?> LoginAsync(LoginRequest request)
    {
        var user = await userRepo.FindByUsernameAsync(request.Username);
        if (user == null) return null;

        // 校验密码（SHA256 哈希比对）
        if (!VerifyPassword(request.Password, user.PasswordHash)) return null;

        var token = GenerateJwtToken(user);
        return new LoginResult(token, user.RealName, user.Roles);
    }

    /// <summary>生成密码哈希（SHA256，用于初始化种子数据）</summary>
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static bool VerifyPassword(string plain, string hash) =>
        HashPassword(plain) == hash;

    private string GenerateJwtToken(SysUser user)
    {
        var jwtSection = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("uid", user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("realName", user.RealName),
        };

        // 每个角色单独添加一条 Claim
        foreach (var role in user.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(jwtSection["ExpireHours"] ?? "8")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
