using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tasker.Core.Interfaces.Services;

namespace Tasker.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public ClaimsPrincipal GetPrincipalFromAccessToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = _config["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid access token");

        return principal;
    }
}
