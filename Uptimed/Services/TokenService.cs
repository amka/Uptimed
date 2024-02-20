using Uptimed.Models;

namespace Uptimed.Services;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

public class TokenService(ILogger<TokenService> logger, IConfiguration configurationManager)
{
    public string CreateToken(ApplicationUser user)
    {
        configurationManager.GetSection("Jwt");
        var expiration = DateTime.UtcNow.AddMinutes(configurationManager.GetValue<int>("Jwt:TokenExpiration"));
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();

        logger.LogInformation("JWT Token created");

        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken(ApplicationUser user)
    {
        configurationManager.GetSection("Jwt");
        var expiration = DateTime.UtcNow.AddMinutes(configurationManager.GetValue<int>("Jwt:RefreshTokenExpiration"));
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();

        logger.LogInformation("JWT Token created");

        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
        DateTime expiration) =>
        new(
            configurationManager.GetValue<string>("Jwt:Issuer"),
            configurationManager.GetValue<string>("Jwt:Audience"),
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private List<Claim> CreateClaims(ApplicationUser user)
    {
        var jwtSub = user.Id;

        try
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwtSub!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                // new Claim(ClaimTypes.NameIdentifier, user.Id),
                // new Claim(ClaimTypes.Name, user.UserName!),
                // new Claim(ClaimTypes.Email, user.Email!),
                // new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var symmetricSecurityKey = configurationManager.GetValue<string>("Jwt:Key") ?? "TheKeyOfTheUptimed";

        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(symmetricSecurityKey)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
    
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationManager.GetValue<string>("Jwt:Key")?? "TheKeyOfTheUptimed")),
            ValidateLifetime = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;

    }
}