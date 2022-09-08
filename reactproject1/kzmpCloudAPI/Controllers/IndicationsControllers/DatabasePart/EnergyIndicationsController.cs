using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace kzmpCloudAPI.Controllers.IndicationsControllers.DatabasePart
{
    [Route("api/indications/[controller]")]
    [ApiController]
    [Authorize]
    public class EnergyIndicationsController : ControllerBase
    {
        kzmp_energyContext dataBase;
        ILogger<EnergyIndicationsController> _logger;
        public EnergyIndicationsController(kzmp_energyContext db, ILogger<EnergyIndicationsController> logger)
        {
            dataBase = db;
            _logger = logger;
        }
        [HttpDelete("delete_energy_indications")]
        public ActionResult DeleteEnergyIndications(int meter_id, string month, int year)
        {
            try
            {
                //Проверка существования записи
                if (!dataBase.EnergyTables.Where(t => t.MeterId == meter_id && t.Month == month && t.Year == year).Any())
                    return BadRequest();

                //Удаление, если есть
                var _records_list = dataBase.EnergyTables.Where(t => t.MeterId == meter_id && t.Month == month && t.Year == year).Select(t => t).ToList();
                dataBase.EnergyTables.RemoveRange(_records_list.ToArray());
                dataBase.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("GetEnergyIndicByAddr")]
        public ActionResult GetEnergyIndicByAddr(int? pageNumber, int? recordsCountToPage, int[]? addresses, bool? countFlag)
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
                        int localRecordsCount = (from t in dataBase.EnergyTables
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

                List<EnergyTable> energyRecords = new List<EnergyTable>();
                int startPosition = Convert.ToInt32((pageNumber - 1) * recordsCountToPage);
                int endPosition = Convert.ToInt32(pageNumber * recordsCountToPage);

                string rawSql = "select * from EnergyTable where ";
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
                energyRecords = dataBase.EnergyTables.FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                    Select(t => t).
                    OrderBy(t => t.MeterId).
                    Skip(startPosition).
                    Take((int)recordsCountToPage).
                    ToList();

                return new JsonResult(energyRecords);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("GetEnergyIndicByAddrStart")]
        public ActionResult GetEnergyIndicByAddrStart(string? startDate, int? pageNumber, int? recordsCountToPage, int[]? addresses, bool? countFlag)
        {
            try
            {
                if (addresses == null || startDate == null)
                {
                    return new BadRequestResult();
                }
                DateTime _startDate = Convert.ToDateTime(startDate);
                if (countFlag == true)
                {
                    int generalRecordsCount = 0;

                    foreach (int address in addresses)
                    {
                        int localRecordsCount = (from t in dataBase.EnergyTables
                                                 where t.Address == address && t.Year >= _startDate.Year
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

                List<EnergyTable> energyRecords = new List<EnergyTable>();
                int startPosition = Convert.ToInt32((pageNumber - 1) * recordsCountToPage);
                int endPosition = Convert.ToInt32(pageNumber * recordsCountToPage);

                List<MySqlParameter> rawSqlParams = new List<MySqlParameter>();
                rawSqlParams.Add(new MySqlParameter("@startDate", _startDate.Year));
                string rawSql = "select * from EnergyTable where Year >= @startDate and (";

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

                energyRecords = dataBase.EnergyTables.
                    FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                    OrderBy(t => t.MeterId).
                    Skip(startPosition).
                    Take((int)recordsCountToPage).
                    ToList();

                return new JsonResult(energyRecords);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("GetEnergyIndicByAddrEnd")]
        public ActionResult GetEnergyIndicByAddrEnd(string? endDate, int? pageNumber, int? recordsCountToPage, int[]? addresses, bool? countFlag)
        {
            try
            {
                if (addresses == null || endDate == null)
                {
                    return new BadRequestResult();
                }
                DateTime _endDate = Convert.ToDateTime(endDate);
                if (countFlag == true)
                {
                    int generalRecordsCount = 0;

                    foreach (int address in addresses)
                    {
                        int localRecordsCount = (from t in dataBase.EnergyTables
                                                 where t.Address == address && t.Year <= _endDate.Year
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

                List<EnergyTable> energyRecords = new List<EnergyTable>();
                int startPosition = Convert.ToInt32((pageNumber - 1) * recordsCountToPage);
                int endPosition = Convert.ToInt32(pageNumber * recordsCountToPage);

                List<MySqlParameter> rawSqlParams = new List<MySqlParameter>();
                rawSqlParams.Add(new MySqlParameter("@endDate", _endDate.Year));
                string rawSql = "select * from EnergyTable where Year <= @endDate and (";

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

                energyRecords = dataBase.EnergyTables.
                    FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                    OrderBy(t => t.MeterId).
                    Skip(startPosition).
                    Take((int)recordsCountToPage).
                    ToList();

                return new JsonResult(energyRecords);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("GetEnergyIndicByAddrStartEnd")]
        public ActionResult GetEnergyIndicByAddrStartEnd(string? startDate, string? endDate, int? pageNumber, int? recordsCountToPage, int[]? addresses, bool? countFlag)
        {
            try
            {
                if (countFlag == null || addresses == null || startDate == null || endDate == null)
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                DateTime _startDate = Convert.ToDateTime(startDate);
                DateTime _endDate = Convert.ToDateTime(endDate);

                if (countFlag == true)
                {
                    int recordsCount = 0;

                    foreach (int address in addresses)
                    {
                        int localCount = (from t in dataBase.EnergyTables
                                          where t.Address == address && t.Year >= _startDate.Year && t.Year <= _endDate.Year
                                          select t).Count();

                        recordsCount += localCount;
                    }


                    var countResult = new
                    {
                        count = recordsCount
                    };
                    return new JsonResult(countResult);
                }

                if (recordsCountToPage == null || pageNumber == null || pageNumber == 0)
                {
                    return new BadRequestResult();
                }

                List<EnergyTable> energyRecords = new List<EnergyTable>();
                int startPosition = Convert.ToInt32((pageNumber - 1) * recordsCountToPage);
                int endPosition = Convert.ToInt32(pageNumber * recordsCountToPage);

                List<MySqlParameter> rawSqlParams = new List<MySqlParameter>();
                rawSqlParams.Add(new MySqlParameter("@endDate", _endDate.Year));
                rawSqlParams.Add(new MySqlParameter("@startDate", _startDate.Year));
                string rawSql = "select * from EnergyTable where Year <= @endDate and Year >= @startDate and (";

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

                energyRecords = dataBase.EnergyTables.
                    FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                    OrderBy(t => t.MeterId).
                    Skip(startPosition).
                    Take((int)recordsCountToPage).
                    ToList();

                return new JsonResult(energyRecords);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
