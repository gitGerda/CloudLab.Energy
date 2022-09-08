using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using kzmpCloudAPI.Components.IndicationsReading;
using Microsoft.AspNetCore.Authorization;
using kzmpCloudAPI.Services;
using kzmpCloudAPI.Components.HangfireSheduler;
using kzmpCloudAPI.Components.IndicationsReading;
using Newtonsoft.Json;
using System.Text;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Database.EF_Core;

namespace kzmpCloudAPI.Controllers.Shedules
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShedulesController : ControllerBase
    {
        private kzmp_energyContext _db;
        private ILogger _logger;
        private IConfiguration _conf;
        private IHangfireSheduler<SheduleCreateUpdateModel> _sheduler;
        public ShedulesController(kzmp_energyContext db, ILogger<ShedulesController> logger, IConfiguration conf, IHangfireSheduler<SheduleCreateUpdateModel> sheduler)
        {
            _db = db;
            _logger = logger;
            _conf = conf;
            _sheduler = sheduler;
        }
        [HttpGet("info")]
        public IActionResult GetGeneralInfoAboutShedules(string? search_str)
        {
            try
            {
                List<SheduleInfo> response = new List<SheduleInfo>();
                int skip = 0;
                while (true)
                {
                    List<SheduleInfo> data = (from t in _db.Shedules
                                              select new SheduleInfo()
                                              {
                                                  shedule_id = t.Id,
                                                  name = t.Name,
                                                  communicPointName = "",
                                                  communicPointId = t.CommunicPointId,
                                                  countRemoteMeters = 0,
                                                  countRemoteModems = 0,
                                                  creating_date = t.CreatingDate,
                                                  lastReadingDate = null,
                                                  shedule = t.Shedule1,
                                                  status = t.Status
                                              }).Where(t => t.name.StartsWith(search_str ?? ""))
                                              .OrderByDescending(t => t.shedule_id)
                                              .Skip(skip).Take(20).ToList();
                    if (data.Count() == 0)
                        break;

                    response.AddRange(data);
                    skip = skip + 20;
                }

                foreach (SheduleInfo shedule in response)
                {
                    if ((from t in _db.ShedulesLogs
                         where t.SheduleId == shedule.shedule_id
                         select t.DateTime).Any())
                    {
                        shedule.lastReadingDate = (from t in _db.ShedulesLogs
                                                   where t.SheduleId == shedule.shedule_id
                                                   select t.DateTime).Max();
                    }

                    shedule.communicPointName = (from t in _db.CommunicPoints
                                                 where t.Id == shedule.communicPointId
                                                 select t.Name).FirstOrDefault() ?? "";

                    var meterIds = (from t in _db.ShedulesMeters
                                    where t.SheduleId == shedule.shedule_id
                                    select t.MeterId).ToArray();

                    if (meterIds.Length == 0)
                        continue;

                    shedule.countRemoteMeters = meterIds.Length;

                    shedule.countRemoteModems = (from t in _db.Meters
                                                 where meterIds.Contains(t.IdMeter)
                                                 select t.Sim).Distinct().Count();
                }

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return new BadRequestResult();
            }
        }

        [HttpGet("communic_points")]
        public IActionResult GetCommunicationPoints()
        {
            try
            {
                List<CommunicPoint> response = new List<CommunicPoint>();

                int skip = 0;
                while (true)
                {
                    var data = (from t in _db.CommunicPoints
                                select t).OrderByDescending(t => t.Id)
                                .Skip(skip).Take(20).ToList();
                    if (data.Count() == 0)
                        break;
                    response.AddRange(data);
                    skip = skip + 20;
                }

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return new BadRequestResult();
            }
        }

        [HttpPost("change_shedule_status")]
        public IActionResult ChangeSheduleStatus(int shedule_id, bool status)
        {
            try
            {
                var _shedule = ChangeStatusField(shedule_id, status);
                //get SheduleCreateUpdateModel
                var _shedule_update_model = GetSheduleCreateUpdateModel(_shedule);
                //check status and create or delete hangfire job
                if (status == true)
                {
                    //create hangfire job
                    var result = _sheduler.CreateShedule(_shedule_update_model).Result;
                }
                else
                {
                    //delete hangfire job
                    _sheduler.DeleteSheduleFromSheduler(shedule_id);
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return new BadRequestResult();
            }
        }
        [HttpPost("delete_shedule")]
        public IActionResult DeleteShedule(int shedule_id)
        {
            try
            {
                _sheduler.DeleteSheduleFull(shedule_id);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return new BadRequestResult();
            }
        }
        [HttpGet("get_shedule_settings_meters")]
        public IActionResult GetSheduleSettingsMeters(int? shedule_id = null)
        {
            try
            {
                List<SheduleSettingsMeters> response = new List<SheduleSettingsMeters>();

                int skip = 0;
                while (true)
                {
                    var data = (from t in _db.Meters
                                select new SheduleSettingsMeters
                                {
                                    address = t.Address,
                                    communic_interface = t.Interface ?? "",
                                    company = t.CompanyName ?? "",
                                    meter_id = t.IdMeter,
                                    sim_number = t.Sim,
                                    type = t.Type ?? "",
                                    xml80020 = t.Xml80020code ?? ""
                                }).Skip(skip).Take(20);
                    if (data.Count() == 0)
                        break;

                    response.AddRange(data);
                    skip = skip + 20;
                }

                skip = 0;
                if (shedule_id != null)
                {
                    while (true)
                    {
                        var data = (from t in _db.ShedulesMeters
                                    where t.SheduleId == shedule_id
                                    select t.MeterId).Skip(skip).Take(20).ToList();

                        if (data.Count() == 0)
                            break;

                        foreach (int meter_id in data)
                        {
                            var data2 = (from t in response
                                         where t.meter_id == meter_id
                                         select t).FirstOrDefault();
                            int index = response.IndexOf(data2 ?? new SheduleSettingsMeters());
                            if (index != -1)
                            {
                                response[index].select_flag = true;
                            }
                        }
                        skip = skip + 20;
                    }
                }

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return new BadRequestResult();
            }
        }
        [HttpPost("create_update_shedule")]
        public async Task<IActionResult> CreateUpdateShedule([FromBody] SheduleCreateUpdateModel data)
        {
            try
            {
                if (data.periodicity != "every week" && data.periodicity != "every day" && data.periodicity != "every month")
                    return new BadRequestResult();

                if (data.shedule_id == null)
                {
                    if (CheckSheduleExisting(data.shedule_name))
                        return new BadRequestResult();

                    //creating new shedule and job for hangfire
                    data.shedule_id = WriteNewSheduleToDb(data);
                    await _sheduler.CreateShedule(data);
                }
                else
                {
                    //updating shedule and hangfire job
                    await _sheduler.UpdateShedule(data);
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return new BadRequestResult();
            }
        }

        [HttpGet("get_logs")]
        public IActionResult GetAllLogs(int skip, int take)
        {
            try
            {
                var _sheduleLogsList = _GetDataFromShedulesLogsTable(skip, take);
                if (_sheduleLogsList != null)
                {
                    var result = _getResponseToLogsRequest(_sheduleLogsList);
                    var result_json = JsonConvert.SerializeObject(result);
                    return Content(result_json, "application/json", Encoding.UTF8);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get_logs_by_shedule")]
        public IActionResult GetLogsBySheduleId(int shedule_id, int skip, int take)
        {
            try
            {
                var _shedulesLogsList = _getDataFromSheduleLogsTableBySheduleId(shedule_id: shedule_id,
                    skip: skip,
                    take: take);

                if (_shedulesLogsList != null)
                {
                    var result = _getResponseToLogsRequest(_shedulesLogsList);
                    var result_json = JsonConvert.SerializeObject(result);
                    return Content(result_json, "application/json", Encoding.UTF8);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("reset_all_logs")]
        public IActionResult ResetAllLogs()
        {
            try
            {
                _resetLogs();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }

        [NonAction]
        public virtual void _resetLogs()
        {
            int _skip = 0;
            int _take = 40;

            while (true)
            {
                var _records = _db.ShedulesLogs.Skip(_skip).Take(_take).ToList();

                if (_records.Count == 0)
                    break;

                _db.ShedulesLogs.RemoveRange(_records);
                _skip = _skip + _take;
            }
            _db.SaveChanges();
        }
        [NonAction]
        public virtual List<LogsResponseModel> _getResponseToLogsRequest(List<ShedulesLog> shedulesLogs)
        {
            List<LogsResponseModel> result = shedulesLogs.Select(t => new LogsResponseModel()
            {
                id = t.Id,
                dateTime = t.DateTime,
                description = t.Description,
                shedule_id = t.SheduleId,
                shedule_name = _db.Shedules.Where(x => x.Id == t.SheduleId).Select(x => x.Name).FirstOrDefault() ?? "",
                status = t.Status
            }).ToList();
            return result;
        }
        [NonAction]
        public virtual List<ShedulesLog>? _getDataFromSheduleLogsTableBySheduleId(int shedule_id, int skip, int take)
        {
            var result = _db.ShedulesLogs.Where(t => t.SheduleId == shedule_id).Skip(skip).Take(take).OrderByDescending(t => t.Id).ToList();
            return result;
        }
        [NonAction]
        public virtual List<ShedulesLog>? _GetDataFromShedulesLogsTable(int skip, int take)
        {
            /*            return _db.ShedulesLogs.Skip(skip).Take(take).OrderByDescending(t => t.Id).ToList();
            */
            return _db.ShedulesLogs.OrderByDescending(t => t.Id).Skip(skip).Take(take).ToList();
        }
        [NonAction]
        public virtual SheduleCreateUpdateModel GetSheduleCreateUpdateModel(Shedule shedule)
        {
            var result = new SheduleCreateUpdateModel()
            {
                shedule_id = shedule.Id,
                communic_point_id = shedule.CommunicPointId,
                periodicity = shedule.Shedule1,
                shedule_name = shedule.Name,
                status = shedule.Status,
                selected_meters_id = new List<int>()
            };

            if (_db.ShedulesMeters.Any(t => t.SheduleId == shedule.Id))
            {
                result.selected_meters_id = _db.ShedulesMeters.Where(t => t.SheduleId == shedule.Id).Select(t => t.MeterId).ToList();
            }

            return result;
        }
        [NonAction]
        public virtual Shedule ChangeStatusField(int shedule_id, bool status)
        {
            var shedule = (from t in _db.Shedules
                           where t.Id == shedule_id
                           select t).FirstOrDefault();

            if (shedule == null)
                throw new Exception();

            shedule.Status = status;
            _db.SaveChanges();

            return shedule;
        }
        [NonAction]
        public virtual bool CheckSheduleExisting(string shedule_name)
        {
            return _db.Shedules.Any(t => t.Name == shedule_name);
        }
        [NonAction]
        public virtual int? WriteNewSheduleToDb(SheduleCreateUpdateModel data)
        {
            DateTime _start_date = new DateTime();
            var _start_date_flag = DateTime.TryParse(data?.start_date, out _start_date);

            Shedule shedule = new Shedule()
            {
                Name = data.shedule_name,
                CommunicPointId = data.communic_point_id,
                CreatingDate = _start_date_flag == true ? _start_date : DateTime.Now,
                Shedule1 = data.periodicity,
                Status = true
            };

            _db.Shedules.Add(shedule);
            _db.SaveChanges();

            int? shedule_id = (from t in _db.Shedules
                               where t.Name == data.shedule_name
                               select t.Id)?.FirstOrDefault();

            if (shedule_id != null)
            {
                foreach (var _meter_id in data.selected_meters_id)
                {
                    var _shedules_meter_record = new ShedulesMeter()
                    {
                        MeterId = _meter_id,
                        SheduleId = shedule_id ?? -1
                    };
                    _db.ShedulesMeters.Add(_shedules_meter_record);
                }
                _db.SaveChanges();
            }

            return shedule_id;
        }

        public struct LogsResponseModel
        {
            public int id;
            public string status;
            public DateTime dateTime;
            public int shedule_id;
            public string? shedule_name;
            public string description;
        }

    }
}
