using HangfireJobsToRabbitLibrary.Models;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Services.RabbitMQService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQLibrary.Interfaces;
using System.Text;

namespace kzmpCloudAPI.Controllers.CommunicPoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicPointsController : ControllerBase
    {
        ILogger<CommunicPointsController> _logger;
        kzmp_energyContext _database;
        IRabbitPublisher _rabbit_publisher;
        public CommunicPointsController(ILogger<CommunicPointsController> logger, kzmp_energyContext database, IRabbitPublisher rabbit_publisher)
        {
            _logger = logger;
            _database = database;
            _rabbit_publisher = rabbit_publisher;
        }
        [HttpPost("add_new_communic_point")]
        public IActionResult AddNewCommunicPoint(string name, string port, string desc)
        {
            try
            {
                //check existing communnic point with same name
                if (_database.CommunicPoints.Any(t => t.Name == name))
                    return BadRequest();

                //create new communic point
                var _communic_point = new CommunicPoint()
                {
                    Name = name,
                    Port = port,
                    Description = desc
                };
                _database.CommunicPoints.Add(_communic_point);
                _database.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();

            }
        }
        [HttpPost("change_communic_point")]
        public IActionResult ChangeCommunicPoint(int communic_point_id, string port, string desc)
        {
            try
            {
                //check existencse communic point with the same id
                if (!CheckCommPointExistencseById(communic_point_id))
                    return BadRequest();
                //get communic point object
                var _communic_point_obj = GetCommunicPoint(communic_point_id);
                //if ports different create rabbitMQ message
                if (_communic_point_obj?.Port != port)
                {
                    CreatePortConfigureRabbitMsg(_communic_point_obj.Name, port);
                    _communic_point_obj.Port = port;
                }

                _communic_point_obj.Description = desc;
                _database.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }
        [HttpPost("delete_communic_point")]
        public IActionResult DeleteCommunicPoint(int communic_point_id)
        {
            try
            {
                //check communic point existence 
                if (!CheckCommPointExistencseById(communic_point_id))
                    return BadRequest();
                //check shedules with this communic point.
                var _shedules = GetShedulesByCommunicPointId(communic_point_id);
                //if such shedule exist return shedule name 
                if (_shedules != null)
                {
                    var shedules_names = _shedules.Select(t => t.Name).ToList();
                    var response = new
                    {
                        status = false,
                        description = shedules_names
                    };
                    return new JsonResult(response);
                }
                //else get CommunicPoint object and delete from database
                DeleteCommunicPointFromDatabase(communic_point_id);
                return new JsonResult(new
                {
                    status = true,
                    description = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }
        [NonAction]
        public virtual void DeleteCommunicPointFromDatabase(int communic_point_id)
        {
            var _communic_point = GetCommunicPoint(communic_point_id);
            if (_communic_point == null)
                throw new Exception("CommunicPoint whith such Id do not exist");
            _database.CommunicPoints.Remove(_communic_point);
            _database.SaveChanges();
        }
        [NonAction]
        public virtual List<Shedule>? GetShedulesByCommunicPointId(int communic_point_id)
        {
            if (!_database.Shedules.Any(t => t.CommunicPointId == communic_point_id))
                return null;

            var result = _database.Shedules.Where(t => t.CommunicPointId == communic_point_id)
                                           .Select(t => t).ToList();
            return result;
        }
        [NonAction]
        public virtual bool CreatePortConfigureRabbitMsg(string communic_point_name, string port)
        {
            string _message = JsonConvert.SerializeObject(new PortConfiguration()
            {
                PortName = port
            });
            _rabbit_publisher.PublishMessage(_rabbit_publisher.publisher_channel, exchange_name: _rabbit_publisher.def_exchange_name, routing_key: communic_point_name, message: Encoding.UTF8.GetBytes(_message));
            return false;
        }
        [NonAction]
        public virtual CommunicPoint? GetCommunicPoint(int communic_point_id)
        {
            var result = _database.CommunicPoints.FirstOrDefault(t => t.Id == communic_point_id);
            return result;
        }

        [NonAction]
        public virtual bool CheckCommPointExistencseById(int communic_point_id)
        {
            if (_database.CommunicPoints.Any(t => t.Id == communic_point_id))
                return true;

            return false;
        }
    }
}
