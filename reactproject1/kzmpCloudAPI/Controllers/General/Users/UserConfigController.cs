using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Database.EF_Core;

namespace kzmpCloudAPI.Controllers.General.Users
{
    [Route("api/users/[controller]")]
    [Authorize]
    [ApiController]
    public class UserConfigController : ControllerBase
    {
        kzmp_energyContext dataBase;
        public UserConfigController(kzmp_energyContext db)
        {
            dataBase = db;
        }

        [HttpGet("GetUserConfig")]
        public ActionResult GetUserConfig(string login, string propName)
        {
            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(propName))
                {
                    return new BadRequestResult();
                }

                string? userConfPropValue = (from user in dataBase.UsersConfigs
                                             where user.UserLogin == login && user.ConfigPropertyName == propName
                                             select user.ConfigPropertyValue).FirstOrDefault();
                var responseObj = new
                {
                    propValue = userConfPropValue
                };

                return new JsonResult(responseObj);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("SetUserConfig")]
        public ActionResult SetUserConfig(string login, string propName, string propValue)
        {
            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(propName))
                {
                    return new BadRequestResult();
                }

                UsersConfig? userConf = (from user in dataBase.UsersConfigs
                                         where user.UserLogin == login && user.ConfigPropertyName == propName
                                         select user).FirstOrDefault();
                if (userConf != null)
                {
                    userConf.ConfigPropertyValue = propValue;
                    dataBase.SaveChanges();

                    return new OkResult();
                }
                else
                {
                    return new BadRequestResult();
                }
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

    }
}
