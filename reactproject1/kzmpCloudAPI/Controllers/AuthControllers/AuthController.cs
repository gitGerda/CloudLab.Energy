using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using kzmpCloudAPI.Components.Auth;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;

namespace kzmpCloudAPI.Controllers.AuthControllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        kzmp_energyContext databaseContext;

        public AuthController(kzmp_energyContext context)
        {
            databaseContext = context;
        }

        [NonAction]
        [HttpPost("CreateNewUser")]
        [Authorize]
        public ActionResult CreateNewUser()
        {
            try
            {


                string login = "";
                string password = "";

                if (login == null || password == null)
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                UsersAuth? userFromDb = (from t in databaseContext.UsersAuths
                                         where t.UserName.ToLower() == login.ToLower()
                                         select t).FirstOrDefault();

                if (userFromDb != null)
                {
                    return new StatusCodeResult(StatusCodes.Status304NotModified);
                }


                UsersAuth user = new UsersAuth();
                user.UserName = login;
                user.Salt = Encoding.ASCII.GetString(PasswordHash.CreateSalt());
                user.UserPwd = PasswordHash.CreateHashFromPwd(password, Encoding.ASCII.GetBytes(user.Salt));


                databaseContext.UsersAuths.Add(user);
                databaseContext.SaveChanges();

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
                var jwt = JwtToken.CreateToken(claims);
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);


                return new JsonResult(encodedJwt);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost("logIn")]
        public ActionResult LogIn([FromForm] string? login, [FromForm] string? pwd)
        {
            if (login == null || pwd == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            UsersAuth? user = (from t in databaseContext.UsersAuths
                               where t.UserName.ToLower() == login.ToLower()
                               select t).FirstOrDefault();

            if (user == null)
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            try
            {
                string hashFromPwd = PasswordHash.CreateHashFromPwd(pwd, Encoding.ASCII.GetBytes(user.Salt));
                if (hashFromPwd == user.UserPwd)
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
                    var jwt = JwtToken.CreateToken(claims);
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                    SessionInfo? sessionInfo = (from t in databaseContext.SessionInfos
                                                where t.UserId == user.UserId
                                                select t).FirstOrDefault();

                    if (sessionInfo != null)
                    {
                        databaseContext.SessionInfos.Remove(sessionInfo);
                        databaseContext.SaveChanges();
                    }

                    sessionInfo = new SessionInfo()
                    {
                        UserId = user.UserId,
                        SessionToken = encodedJwt,
                        DeviceId = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                        AuthTime = DateTime.Now
                    };
                    databaseContext.SessionInfos.Add(sessionInfo);
                    databaseContext.SaveChanges();

                    var response = new
                    {
                        name = "aspNetToken",
                        token = encodedJwt
                    };

                    return new JsonResult(response);
                }
                else
                {
                    return new StatusCodeResult(StatusCodes.Status401Unauthorized);
                }
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
