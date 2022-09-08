using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using kzmpCloudAPI.Controllers.Meters;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authorization;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;

namespace kzmpCloudAPI.Controllers.Meters
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MetersController : ControllerBase
    {
        kzmp_energyContext dataBase;
        ILogger<MetersController> _logger;
        public MetersController(kzmp_energyContext db, ILogger<MetersController> logger)
        {
            dataBase = db;
            _logger = logger;
        }

        [HttpGet("remove_company_by_inn")]
        public ActionResult DeleteCompany(string company_inn)
        {
            try
            {
                //Проверка существования
                if (dataBase.MsgNumbers.Where(t => t.CompanyInn == company_inn).Any())
                {
                    //Проверка существования счётчиков сопоставленных с этой компанией
                    if (dataBase.Meters.Where(t => t.Inn == company_inn).Any())
                    {
                        //Если такие счётчики есть BadResult
                        return BadRequest();
                    }
                    else
                    {
                        //Если нет счётчиков, которые сопоставлены с этой компанией
                        //удаляем компанию
                        var _company = dataBase.MsgNumbers.Where(t => t.CompanyInn == company_inn).Select(t => t).First();
                        dataBase.MsgNumbers.Remove(_company);
                        dataBase.SaveChanges();
                        return Ok();
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("create_update_company")]
        public ActionResult CreateUpdateCompany([FromQuery] MsgNumber data)
        {
            try
            {
                //Проверка существования компании
                if (dataBase.MsgNumbers.Where(t => t.CompanyInn == data.CompanyInn).Any())
                {
                    //Если есть компания с таким же ИНН
                    var _company = dataBase.MsgNumbers.Where(t => t.CompanyInn == data.CompanyInn).Select(t => t).First();
                    _company.CompanyName = data.CompanyName;
                    _company.MsgNumber1 = data.MsgNumber1;
                    _company.Date = data.Date;
                    _company.Contract = data.Contract;

                    //Проверка таблицы Meters
                    if (dataBase.Meters.Where(t => t.Inn == data.CompanyInn).Any())
                    {
                        var _meters = dataBase.Meters.Where(t => t.Inn == data.CompanyInn).Select(t => t).ToList();
                        foreach (var _meter in _meters)
                        {
                            _meter.CompanyName = data.CompanyName;
                        }
                    }
                }
                else
                {
                    //Если такой компании нет
                    dataBase.MsgNumbers.Add(data);
                }

                dataBase.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet("get_full_companies_info")]
        public ActionResult GetCompanyInfoFull()
        {
            try
            {
                var _response = dataBase.MsgNumbers.Select(t => t).ToList();
                return new JsonResult(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();

            }
        }

        [HttpGet("getAllMetersInfo")]
        public ActionResult GetAllMetersInfo()
        {
            try
            {

                List<MetersByCompanyINN> metersByCompanyINNs = new List<MetersByCompanyINN>();
                List<string> DifferentInn = (from meter in dataBase.Meters
                                             select meter.Inn).ToList().Distinct().ToList();

                foreach (string currentInn in DifferentInn)
                {
                    if (string.IsNullOrEmpty(currentInn))
                    {
                        continue;
                    }

                    MetersByCompanyINN? metersByCompanyINN = (from meter in dataBase.Meters
                                                              where meter.Inn.Equals(currentInn)
                                                              select new MetersByCompanyINN
                                                              {
                                                                  companyName = meter.CompanyName,
                                                                  centerInn = meter.Inncenter,
                                                                  centerName = meter.CenterName
                                                              }).FirstOrDefault();

                    if (metersByCompanyINN != null)
                    {
                        metersByCompanyINN.companyInn = currentInn;
                        var xmll80020info = (from msgNumber in dataBase.MsgNumbers
                                             where msgNumber.CompanyInn.Equals(currentInn)
                                             select new
                                             {
                                                 msgNumber.Contract,
                                                 msgNumber.MsgNumber1,
                                                 msgNumber.Date
                                             }).FirstOrDefault();
                        if (xmll80020info != null)
                        {

                            metersByCompanyINN.contractNumber = xmll80020info.Contract;
                            metersByCompanyINN.xml80020MsgNumber = xmll80020info.MsgNumber1;
                            metersByCompanyINN.xml80020MsgNumberDate = xmll80020info.Date;
                        }

                        metersByCompanyINN.meters = new List<MeterInfoBySim>();

                        List<string> differentSIM = (from meter in dataBase.Meters
                                                     where meter.Inn.Equals(currentInn)

                                                     select meter.Sim).ToList().Distinct().ToList();
                        foreach (string currentSim in differentSIM)
                        {
                            MeterInfoBySim meterInfoBySim = new MeterInfoBySim();
                            meterInfoBySim.sim = currentSim;

                            meterInfoBySim.metersINFO = (from meter2 in dataBase.Meters
                                                         where meter2.Inn.Equals(currentInn) && meter2.Sim.Equals(currentSim)
                                                         select new MeterInfo
                                                         {
                                                             id_meter = meter2.IdMeter.ToString(),
                                                             type = meter2.Type,
                                                             address = meter2.Address.ToString(),
                                                             sim = meter2.Sim,
                                                             Interface = meter2.Interface,
                                                             lastIndicationDate = null,
                                                             lastIndicationTime = null
                                                         }).ToList();

                            foreach (var meter in meterInfoBySim.metersINFO)
                            {
                                meter.lastIndicationDate = (from power in dataBase.PowerProfileMs
                                                            where power.Id == Convert.ToInt32(meter.id_meter)
                                                            select power.Date).Max(value => (DateTime?)value);

                                meter.lastIndicationTime = (from power in dataBase.PowerProfileMs
                                                            where power.Id == Convert.ToInt32(meter.id_meter)
                                                            && power.Date == meter.lastIndicationDate
                                                            select power.Time).Max(value =>
                                                                (TimeSpan?)value);
                            };

                            metersByCompanyINN.meters.Add(meterInfoBySim);
                        }
                        metersByCompanyINNs.Add(metersByCompanyINN);
                    }

                }

                return new JsonResult(metersByCompanyINNs);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }


        [HttpGet("getMeterInfo")]
        public ActionResult GetMeterInfo(int meterID)
        {
            try
            {
                var meterInfo = (from meter in dataBase.Meters
                                 where meter.IdMeter == meterID
                                 select meter).FirstOrDefault();
                return new JsonResult(meterInfo);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }

        [HttpGet("getCompanies")]
        public ActionResult GetCompanies()
        {
            try
            {
                var companies = (from t in dataBase.MsgNumbers
                                 select new
                                 {
                                     name = t.CompanyName,
                                     inn = t.CompanyInn
                                 }).ToList();
                return new JsonResult(companies);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }

        [HttpGet("getCompaniesSenders")]
        public ActionResult GetCompaniesSenders()
        {
            try
            {
                var companies = (from meter in dataBase.Meters
                                 select meter.CenterName).Distinct().ToList();
                return new JsonResult(companies);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }

        [HttpGet("getMeterTypes")]
        public ActionResult GetMeterTypes()
        {
            try
            {
                var companies = (from meter in dataBase.Meters
                                 select meter.Type).Distinct().ToList();
                return new JsonResult(companies);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }

        [HttpGet("getMeterInterfaces")]
        public ActionResult GetMeterInterfaces()
        {
            try
            {
                var companies = (from meter in dataBase.Meters
                                 select meter.Interface).Distinct().ToList();
                return new JsonResult(companies);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }

        [HttpPost("changeMeterData")]
        public ActionResult ChangeMeterData()
        {
            try
            {
                int IdMeter = 0;
                int AddressMeter = 0;
                var form = Request.Form.ToList();
                Dictionary<string, StringValues> formDic = Request.Form.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (!int.TryParse(formDic["idMeter"].ToString(), out IdMeter))
                {
                    return new StatusCodeResult(304); // 304: Not Modified
                };
                if (IdMeter == 0)
                {
                    return new StatusCodeResult(304); // 304: Not Modified
                }

                Meter? meter = (from meterObj in dataBase.Meters
                                where meterObj.IdMeter == IdMeter
                                select meterObj).FirstOrDefault();
                if (meter == null)
                {
                    return new StatusCodeResult(404); //404: Not Found
                }

                if (meter.Type != formDic["Type"].ToString())
                {
                    meter.Type = formDic["Type"].ToString();
                }
                if (int.TryParse(formDic["Address"].ToString(), out AddressMeter))
                {
                    if (meter.Address != AddressMeter)
                    {
                        meter.Address = AddressMeter;
                    }
                }
                if (meter.Sim != formDic["Sim"].ToString())
                {
                    meter.Sim = formDic["Sim"].ToString();
                }
                if (meter.Inn != formDic["Inn"].ToString())
                {
                    meter.Inn = formDic["Inn"].ToString();
                }
                if (meter.CompanyName != formDic["CompanyName"].ToString())
                {
                    meter.CompanyName = formDic["CompanyName"].ToString();
                }
                if (meter.Inncenter != formDic["Inncenter"].ToString())
                {
                    meter.Inncenter = formDic["Inncenter"].ToString();
                }
                if (meter.CenterName != formDic["CenterName"].ToString())
                {
                    meter.CenterName = formDic["CenterName"].ToString();
                }
                if (meter.MeasuringpointName != formDic["MeasuringpointName"].ToString())
                {
                    meter.MeasuringpointName = formDic["MeasuringpointName"].ToString();
                }
                if (meter.MeasuringchannelA != formDic["MeasuringchannelA"].ToString())
                {
                    meter.MeasuringchannelA = formDic["MeasuringchannelA"].ToString();
                }
                if (meter.MeasuringchannelR != formDic["MeasuringchannelR"].ToString())
                {
                    meter.MeasuringchannelR = formDic["MeasuringchannelR"].ToString();
                }
                if (meter.Xml80020code != formDic["Xml80020code"].ToString())
                {
                    meter.Xml80020code = formDic["Xml80020code"].ToString();
                }
                if (meter.TransformationRatio != formDic["TransformationRatio"].ToString())
                {
                    meter.TransformationRatio = formDic["TransformationRatio"].ToString();
                }
                if (meter.Interface != formDic["Interface"].ToString())
                {
                    meter.Interface = formDic["Interface"].ToString();
                }

                dataBase.Meters.Update(meter);
                dataBase.SaveChanges();

                return new StatusCodeResult(200); //OK
            }
            catch
            {
                return new StatusCodeResult(500); //Internal Server Error
            }
        }
        [HttpDelete("deleteMeter")]
        public ActionResult DeleteMeter(int meterID)
        {
            try
            {
                Meter? meter = (from el in dataBase.Meters
                                where el.IdMeter == meterID
                                select el).FirstOrDefault();
                if (meter != null)
                {
                    if (dataBase.ShedulesMeters.Where(t => t.MeterId == meterID).Any())
                        return BadRequest();

                    dataBase.Meters.Remove(meter);
                    dataBase.SaveChanges();
                    return Ok();
                }

                return BadRequest();
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("create_meter")]
        public ActionResult CreateMeter([FromForm] Meter data)
        {
            try
            {
                //Проверка существования счётчика с таким же id
                if (dataBase.Meters.Where(t => t.IdMeter == data.IdMeter).Any() == false)
                {
                    dataBase.Meters.Add(new Meter()
                    {
                        Address = data.Address,
                        CenterName = data.CenterName,
                        CompanyName = data.CompanyName,
                        Inn = data.Inn,
                        Inncenter = data.Inncenter,
                        Interface = data.Interface,
                        MeasuringchannelA = data.MeasuringchannelA,
                        MeasuringchannelR = data.MeasuringchannelR,
                        MeasuringpointName = data.MeasuringpointName,
                        Sim = data.Sim,
                        TransformationRatio = data.TransformationRatio,
                        Type = data.Type,
                        Xml80020code = data.Xml80020code
                    });
                    if (dataBase.SaveChanges() == 1)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("getMetersByCompany")]
        public ActionResult GetMetersByCompany(string company)
        {
            try
            {
                List<Meter> meters = (from t in dataBase.Meters
                                      where t.CompanyName == company
                                      select t).ToList();
                return new JsonResult(meters);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
