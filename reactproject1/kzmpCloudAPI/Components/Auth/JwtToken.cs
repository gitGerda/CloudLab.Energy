using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace kzmpCloudAPI.Components.Auth
{
    public static class JwtToken
    {
        public static JwtSecurityToken CreateToken(List<Claim> claims)
        {
            return new JwtSecurityToken(
                    issuer: AuthOptions.issuer,
                    audience: AuthOptions.audience,
                    expires: DateTime.UtcNow.AddHours(3),
                    claims: claims,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.key)),
                        SecurityAlgorithms.HmacSha256)
                );
        }
    }
}
