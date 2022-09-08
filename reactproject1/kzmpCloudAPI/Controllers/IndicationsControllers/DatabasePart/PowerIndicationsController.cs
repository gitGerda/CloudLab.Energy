using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;
using System.Globalization;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;

namespace kzmpCloudAPI.Controllers.IndicationsControllers.DatabasePart
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/indications/[controller]")]
    [ApiController]
    [Authorize]
    public class PowerIndicationsController : ControllerBase
    {
        kzmp_energyContext dataBase;
        ILogger<PowerIndicationsController>? _logger;
        public PowerIndicationsController(kzmp_energyContext db, ILogger<PowerIndicationsController>? logger = null)
        {
            dataBase = db;
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            _logger = logger;
        }

        [HttpDelete("delete_indications")]
        public ActionResult DeleteIndications(int meter_address, string start_date, string end_date)
        {
            try
            {
                DateTime _start;
                DateTime _end;

                if (DateTime.TryParse(start_date, out _start) == false || DateTime.TryParse(end_date, out _end) == false)
                    return BadRequest();

                while (dataBase.PowerProfileMs.Where(t => t.Address == meter_address && t.Date >= _start && t.Date <= _end).Any())
                {
                    var _records = dataBase.PowerProfileMs.Where(t => t.Address == meter_address && t.Date >= _start && t.Date <= _end).Take(50).ToList();
                    dataBase.PowerProfileMs.RemoveRange(_records);
                    dataBase.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }
        [AllowAnonymous]
        [HttpPost("create_test_records")]
        public ActionResult CreateTestRecords(int meter_id, int _address, string start_date, string end_date)
        {
            var _max_row_number = dataBase.PowerProfileMs.Max(t => t.RowNumber);

            DateTime _start = DateTime.Parse(start_date);
            _start = new DateTime(_start.Year, _start.Month, _start.Day, 0, 0, 0);
            DateTime _end = DateTime.Parse(end_date);

            while (DateTime.Compare(_start, _end) < 0)
            {
                var _next_day = _start.AddDays(1).Day;
                while (_start.Day != _next_day)
                {
                    var _record = new PowerProfileM()
                    {
                        Address = _address,
                        Date = _start,
                        Id = meter_id,
                        Pminus = 0,
                        Pplus = 0,
                        Qminus = 0,
                        Qplus = 0,
                        RowNumber = _max_row_number,
                        Time = _start.TimeOfDay
                    };
                    _start = _start.AddMinutes(30);
                    _max_row_number++;
                    dataBase.PowerProfileMs.Add(_record);
                }
                _start = _start.AddDays(1);
                dataBase.SaveChanges();
            }
            return Ok();
        }

        [HttpPost("GetIndicByAddr")]
        public ActionResult GetIndicByAddr(int? pageNumber, int? recordsCountToPage, int[]? addresses, bool? countFlag)
        {
            try
            {
                if (addresses == null)
                {
                    return new BadRequestResult();
                }
                if (countFlag == true)
                {
                    int generalRecordsCount = 0;

                    foreach (int address in addresses)
                    {
                        int localRecordsCount = (from t in dataBase.PowerProfileMs
                                                 where t.Address == address
                                                 select t).Count();
                        generalRecordsCount += localRecordsCount;
                    }

                    var countResult = new
                    {
                        count = generalRecordsCount
                    };
                    return new JsonResult(countResult);
                }

                if (recordsCountToPage == null || pageNumber == null || pageNumber == 0)
                {
                    return new BadRequestResult();
                }

                List<PowerProfileM> powerRecords = new List<PowerProfileM>();
                int startPosition = Convert.ToInt32((pageNumber - 1) * recordsCountToPage);
                int endPosition = Convert.ToInt32(pageNumber * recordsCountToPage);

                string rawSql = "select * from power_profile_m where ";
                List<MySqlParameter> rawSqlParams = new List<MySqlParameter>();
                for (int i = 0; i < addresses.Length; i++)
                {
                    rawSql += $" address = @addresses{i}";
                    rawSqlParams.Add(new MySqlParameter($"@addresses{i}", addresses[i]));

                    int nextIndex = i + 1;
                    if (nextIndex < addresses.Length)
                    {
                        rawSql += " or ";
                    }
                };
                powerRecords = dataBase.PowerProfileMs.FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                    Select(t => t).
                    Skip(startPosition).
                    Take((int)recordsCountToPage).
                    OrderBy(t => t.RowNumber).
                    ToList();

                return new JsonResult(powerRecords);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("GetIndicByAddrStartEnd")]
        public ActionResult GetIndicByAddrStartEnd(int? pageNumber, int? recordsCountToPage, int[]? addresses,
            string? _startDate, string? _endDate, bool? countFlag)
        {
            try
            {
                if (countFlag == null || addresses == null || _startDate == null || _endDate == null)
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                DateTime startDate = Convert.ToDateTime(_startDate);
                DateTime endDate = Convert.ToDateTime(_endDate);

                if (countFlag == true)
                {
                    int recordsCount = 0;

                    foreach (int address in addresses)
                    {
                        int localCount = (from t in dataBase.PowerProfileMs
                                          where t.Address == address && t.Date >= startDate && t.Date <= endDate
                                          select t).Count();

                        recordsCount += localCount;
                    }


                    var countResult = new
                    {
                        count = recordsCount
                    };
                    return new JsonResult(countResult);
                }
                else
                {
                    if (pageNumber == null || pageNumber == 0 || recordsCountToPage == null)
                    {
                        return new StatusCodeResult(StatusCodes.Status400BadRequest);
                    }
                    int startPosition = Convert.ToInt32((pageNumber - 1) * recordsCountToPage);
                    int endPosition = Convert.ToInt32(pageNumber * recordsCountToPage);

                    List<PowerProfileM> GeneralRecords = new List<PowerProfileM>();

                    foreach (int address in addresses)
                    {
                        if (GeneralRecords.Count() >= endPosition)
                        {
                            break;
                        }
                        List<PowerProfileM> localRecordsList = (from t in dataBase.PowerProfileMs
                                                                where t.Address == address && t.Date >= startDate && t.Date <= endDate
                                                                select t).ToList();
                        GeneralRecords.AddRange(localRecordsList);
                    }
                    if (GeneralRecords.Count() <= startPosition)
                    {
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                    }
                    else if (GeneralRecords.Count() < endPosition)
                    {
                        GeneralRecords = GeneralRecords.GetRange(startPosition, GeneralRecords.Count() - startPosition);
                    }
                    else
                    {
                        GeneralRecords = GeneralRecords.GetRange(startPosition, (int)recordsCountToPage);
                    }

                    var result = new
                    {
                        countOfRecords = GeneralRecords.Count(),
                        records = GeneralRecords
                    };

                    return new JsonResult(GeneralRecords);
                }
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("GetIndicByAddrStart")]
        public ActionResult GetIndicByAddrStart(int? pageNumber, int? recordsCountToPage, int[]? addresses, string? startDate,
            bool? countFlag)
        {
            try
            {
                if (addresses == null || startDate == null)
                {
                    return new BadRequestResult();
                }
                if (countFlag == true)
                {
                    int generalRecordsCount = 0;

                    foreach (int address in addresses)
                    {
                        int localRecordsCount = (from t in dataBase.PowerProfileMs
                                                 where t.Address == address && t.Date >= Convert.ToDateTime(startDate)
                                                 select t).Count();
                        generalRecordsCount += localRecordsCount;
                    }

                    var countResult = new
                    {
                        count = generalRecordsCount
                    };
                    return new JsonResult(countResult);
                }
                if (recordsCountToPage == null || pageNumber == null || pageNumber == 0)
                {
                    return new BadRequestResult();
                }

                List<PowerProfileM> powerRecords = new List<PowerProfileM>();
                int startPosition = Convert.ToInt32((pageNumber - 1) * recordsCountToPage);
                int endPosition = Convert.ToInt32(pageNumber * recordsCountToPage);

                DateTime startDateD = Convert.ToDateTime(startDate);

                List<MySqlParameter> rawSqlParams = new List<MySqlParameter>();
                rawSqlParams.Add(new MySqlParameter("@startDate", startDateD.ToString("yyyy-MM-dd")));
                string rawSql = "select * from power_profile_m where date >= @startDate and (";

                for (int i = 0; i < addresses.Length; i++)
                {
                    rawSql += $" address = @addresses{i}";
                    rawSqlParams.Add(new MySqlParameter($"@addresses{i}", addresses[i]));

                    int nextIndex = i + 1;
                    if (nextIndex < addresses.Length)
                    {
                        rawSql += " or ";
                    }
                    else
                    {
                        rawSql += ")";
                    }
                };

                powerRecords = dataBase.PowerProfileMs.
                    FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                    Skip(startPosition).
                    Take((int)recordsCountToPage).
                    OrderBy(t => t.RowNumber).
                    ToList();

                return new JsonResult(powerRecords);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("GetIndicByAddrEnd")]
        public ActionResult GetIndicByAddrEnd(int? pageNumber, int? recordsCountToPage, int[]? addresses, string? endDate,
            bool? countFlag)
        {
            try
            {
                if (addresses == null || endDate == null)
                {
                    return new BadRequestResult();
                }
                if (countFlag == true)
                {
                    int generalRecordsCount = 0;

                    foreach (int address in addresses)
                    {
                        int localRecordsCount = (from t in dataBase.PowerProfileMs
                                                 where t.Address == address && t.Date <= Convert.ToDateTime(endDate)
                                                 select t).Count();
                        generalRecordsCount += localRecordsCount;
                    }

                    var countResult = new
                    {
                        count = generalRecordsCount
                    };
                    return new JsonResult(countResult);
                }
                if (recordsCountToPage == null || pageNumber == null || pageNumber == 0)
                {
                    return new BadRequestResult();
                }

                List<PowerProfileM> powerRecords = new List<PowerProfileM>();
                int startPosition = Convert.ToInt32((pageNumber - 1) * recordsCountToPage);
                int endPosition = Convert.ToInt32(pageNumber * recordsCountToPage);

                DateTime endDateD = Convert.ToDateTime(endDate);

                List<MySqlParameter> rawSqlParams = new List<MySqlParameter>();
                rawSqlParams.Add(new MySqlParameter("@endDate", endDateD.ToString("yyyy-MM-dd")));
                string rawSql = "select * from power_profile_m where date <= @endDate and (";

                for (int i = 0; i < addresses.Length; i++)
                {
                    rawSql += $" address = @addresses{i}";
                    rawSqlParams.Add(new MySqlParameter($"@addresses{i}", addresses[i]));

                    int nextIndex = i + 1;
                    if (nextIndex < addresses.Length)
                    {
                        rawSql += " or ";
                    }
                    else
                    {
                        rawSql += ")";
                    }
                };

                powerRecords = dataBase.PowerProfileMs.
                    FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                    Skip(startPosition).
                    Take((int)recordsCountToPage).
                    OrderBy(t => t.RowNumber).
                    ToList();

                return new JsonResult(powerRecords);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_last_indic_datetime")]
        public IActionResult GetLastIndicationDateTime(string meter_address)
        {
            try
            {
                int meter_address_int32;
                DateTime? date;
                TimeSpan? time;

                if (!Int32.TryParse(meter_address, out meter_address_int32))
                    throw new Exception("Could not convert meter_address parametr to Int32");

                var any_result = (from t in dataBase.PowerProfileMs
                                  where t.Address == meter_address_int32
                                  select t).Any();

                if (any_result == false)
                    return new NotFoundResult();

                date = (from t in dataBase.PowerProfileMs
                        where t.Address == meter_address_int32
                        select t)?.Max(t => t.Date);

                if (date != null)
                {
                    time = (from t in dataBase.PowerProfileMs
                            where t.Address == meter_address_int32 && t.Date == date
                            select t).Max(t => t.Time);
                    var response = new
                    {
                        last_date = date?.ToShortDateString(),
                        last_time = time
                    };

                    return new JsonResult(response);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex.Message);
                return new BadRequestResult();
            }
        }

    }
}
