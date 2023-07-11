using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sandbox.Lib;

public class JwtSecrets
{
    public string Issuer { get; }
    public string Audience { get; }
    public string Secret { get; }

    public JwtSecrets(string issuer, string audience, string secret)
    {
        Issuer = issuer;
        Audience = audience;
        Secret = secret;
    }
}

public class Jwt
{
    private readonly JwtSecrets secrets;

    public Jwt(JwtSecrets secrets)
    {
        this.secrets = secrets;
    }

    public string Token(string uid)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrets.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, uid),
        };

        var jwt = new JwtSecurityToken(
            issuer: secrets.Issuer,
            expires: DateTime.Now.AddMinutes(15),
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}