using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace kzmpCloudAPI.Components.Auth
{
    public static class AuthOptions
    {
        public const string issuer = "cloudLab";
        public const string audience = "cloudLabAudience";
        public const string key = "mysupersecret_secretkey!123";
    }
}
