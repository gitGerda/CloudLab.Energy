using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using kzmpCloudAPI.Services;
using kzmpCloudAPI.Components.ReportsXML80020;
using kzmpCloudAPI.Components.ReportsXML80020.EnergyReport;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Database.EF_Core;
using Microsoft.EntityFrameworkCore;

namespace kzmpCloudAPI.Controllers.ReportXML80020_Controller
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]

    public class ReportXML80020Controller : ControllerBase
    {
        kzmp_energyContext dataBase;
        ReportXML80020Service reportXML80020_service;
        IWebHostEnvironment env;
        IServer server;
        IConfiguration _configuration;
        ILogger<ReportXML80020Controller> _logger;
        public ReportXML80020Controller(kzmp_energyContext db, ReportXML80020Service _reportXml80020, IWebHostEnvironment _env, IServer _server, IConfiguration configuration, ILogger<ReportXML80020Controller> logger)
        {
            dataBase = db;
            reportXML80020_service = _reportXml80020;
            env = _env;
            server = _server;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("createReport")]
        public ActionResult CreateReport([FromBody] ReportFormParameters reportFormParam)
        {
            try
            {
                _logger.LogCritical("Creating report...");
                double GeneralProfileSum = 0;
                int? reportHistoryId = null;
                if (reportXML80020_service.CreateReport(reportFormParam, out reportHistoryId, ref GeneralProfileSum))
                {
                    string serverHttpsAddr = _configuration["ADDRESS_DOWNLOADING"] ?? "";
                    string pathToReport = reportXML80020_service.PATH_TO_REPORTS.Replace(env.WebRootPath, serverHttpsAddr);

                    if (reportFormParam.energyFlag != null && reportFormParam.energyFlag == "on")
                    {

                        string companyInn = (from t in dataBase.Meters
                                             where t.CompanyName == (reportFormParam.companyName ?? "")
                                             select t.Inn).FirstOrDefault() ?? "";
                        DateTime reportDate;
                        if (!DateTime.TryParse(reportFormParam.startDate, out reportDate))
                        {
                            reportDate = DateTime.Now;
                        }

                        EnergyReport energyReport = new EnergyReport(dataBase, reportHistoryId ?? 0, companyInn, reportDate, _configuration.GetConnectionString("DefaultConnection"));
                        var energyDetalization = energyReport.CreateEnergyReport();


                        var response = new
                        {
                            reportPath = pathToReport + ".zip",
                            energyDetalization
                        };
                        return new JsonResult(response);
                    }
                    else
                    {
                        var response = new
                        {
                            reportPath = pathToReport + ".zip",
                        };
                        return new JsonResult(response);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error Message" + ex.Message);
                _logger.LogCritical("Stack Trace: " + ex.StackTrace);
                return new StatusCodeResult(500);
            }
        }

        [AllowAnonymous]
        [HttpGet("GetReportsHistory")]
        public async Task<ActionResult> GetReportsHistory(int skip, int take)
        {
            try
            {
                List<object> responseList = new List<object>();

                /*                List<ReportsHistory> response = (from t in dataBase.ReportsHistories
                                                                 select t).OrderByDescending(t => t.ReportsDate)
                                                                 .Skip(skip)
                                                                 .Take(take)
                                                                 .OrderBy(t => t.ReportsDate)
                                                                 .ToList();
                */
                /*                List<ReportsHistory> response = await (from t in dataBase.ReportsHistories
                                                                       select t)
                                                               .SkipLast(skip)
                                                               .TakeLast(take)
                                                               .OrderBy(t => t.ReportsDate)
                                                               .ToListAsync();
                */
                var response = dataBase.ReportsHistories.OrderByDescending(t => t.RecordCreateDate).Skip(skip).Take(take).ToList();

                foreach (ReportsHistory history in response)
                {
                    var detalizationFlag = (from t in dataBase.ReportsDetalizations
                                            where t.ReportId == history.Id
                                            select t).Any();

                    responseList.Add(new
                    {
                        detalizationFlag,
                        historyElement = history
                    });
                }


                return new JsonResult(responseList);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("GetReportDetalization")]
        public ActionResult GetReportDetalization(int reportId)
        {
            try
            {
                var response = (from t in dataBase.ReportsDetalizations
                                where t.ReportId == reportId
                                select t).ToList();

                return new JsonResult(response);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

    }

}
