using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using kzmpCloudAPI.Components.ReportsXML80020;
using MySqlConnector;
using KzmpEnergyReportLibrary.Models;
using KzmpEnergyReportLibrary.Actions;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Interfaces;

namespace kzmpCloudAPI.Services
{
    public class ReportXML80020Service
    {
        kzmp_energyContext dataBase;
        IWebHostEnvironment env;
        ILogger logger;
        DocumentXml80020? DOCUMENTS_XML80020;
        IConfiguration _configuration;

        public string PATH_TO_REPORTS = "";
        string? PATH_TO_XML80020_REPORT;
        public ReportXML80020Service(kzmp_energyContext _dataBase, IWebHostEnvironment _env, ILogger<ReportXML80020Service> _logger, IConfiguration configuration)
        {
            dataBase = _dataBase;
            env = _env;
            logger = _logger;
            _configuration = configuration;
        }

        public bool CreateReport(ReportFormParameters reportFormParam, out int? reportHistoryId, ref double GenFloatSum)
        {
            //VARS
            bool BAD_RESULT = false;
            bool GOOD_RESULT = true;
            DateTime START_DATE;
            DateTime END_DATE;
            int MSG_NUMBER;
            DateTime START_DATE_FOR_MSG_NUMBER;
            int COUNT_OF_DOCS;
            string COMPANY_NAME = reportFormParam?.companyName ?? "";
            reportHistoryId = null;
            DOCUMENTS_XML80020 = new DocumentXml80020();
            DateTime recordCurrentDate = DateTime.Now;

            string currentDateStr = Convert.ToString(DateTime.Now).Replace(".", "-").Replace(" ", "_").Replace(":", "-");

            PATH_TO_REPORTS = env.WebRootPath + @"/reports_xml80020/" + currentDateStr + "-" + Convert.ToString(DateTime.Now.Millisecond);

            if (!Directory.Exists(PATH_TO_REPORTS))
            {
                try
                {
                    Directory.CreateDirectory(PATH_TO_REPORTS);
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex.Message);
                };
            }

            bool flagStartDateConvert = DateTime.TryParse(reportFormParam?.startDate, out START_DATE);
            bool flagEndDateConvert = DateTime.TryParse(reportFormParam?.endDate, out END_DATE);

            COUNT_OF_DOCS = ReportFiles.GetCountOfDocsFromPeriod(START_DATE, END_DATE);

            bool flagMsgNumberConverting = int.TryParse((from t in dataBase.MsgNumbers
                                                         where t.CompanyName == COMPANY_NAME
                                                         select t.MsgNumber1).FirstOrDefault(), out MSG_NUMBER);

            bool flagStartDateMsgNumbConv = DateTime.TryParse((from t in dataBase.MsgNumbers
                                                               where t.CompanyName == COMPANY_NAME
                                                               select t.Date).FirstOrDefault(), out START_DATE_FOR_MSG_NUMBER);

            var INFO = (from t in dataBase.Meters
                        where t.CompanyName == COMPANY_NAME
                        select new
                        {
                            COMPANY_INN = Convert.ToString(t.Inn ?? ""),
                            SENDER_INN = Convert.ToString(t.Inncenter ?? ""),
                            SENDER_NAME = Convert.ToString(t.CenterName ?? "")
                        }).FirstOrDefault();

            if (COMPANY_NAME == "" || INFO == null || INFO?.COMPANY_INN == "" || INFO?.SENDER_INN == "" || INFO?.SENDER_NAME == ""
                || COUNT_OF_DOCS == 0 || flagStartDateConvert == false || flagEndDateConvert == false || flagMsgNumberConverting == false || flagStartDateMsgNumbConv == false || !Directory.Exists(PATH_TO_REPORTS))
            {
                return BAD_RESULT;
            }

            PATH_TO_XML80020_REPORT = GetPathToReport(INFO?.COMPANY_INN ?? "", START_DATE);
            if (PATH_TO_XML80020_REPORT == null)
            {
                return BAD_RESULT;
            }
            else
            {
                if (!Directory.Exists(PATH_TO_XML80020_REPORT))
                {
                    try
                    {
                        Directory.CreateDirectory(PATH_TO_XML80020_REPORT);
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex.Message);
                        return BAD_RESULT;
                    };
                }
            }

            MSG_NUMBER = DOCUMENTS_XML80020.CalcMsgNumber(START_DATE, MSG_NUMBER, START_DATE_FOR_MSG_NUMBER);
            List<MeterInfo> meterInfoList = GetMeterInfoList(INFO?.COMPANY_INN ?? "");
            if (meterInfoList.Count() == 0)
            {
                return BAD_RESULT;
            }

            string serverHttpsAddr = _configuration["ADDRESS_DOWNLOADING"] ?? "";
            string pathToReport = PATH_TO_REPORTS.Replace(env.WebRootPath, serverHttpsAddr);

            ReportsHistory reportHistory = new ReportsHistory()
            {
                CompanyInn = INFO?.COMPANY_INN ?? "",
                CompanyName = COMPANY_NAME,
                PeriodStartDate = START_DATE,
                PeriodEndDate = END_DATE,
                RecordCreateDate = recordCurrentDate,
                ReportPath = pathToReport + ".zip",
            };

            for (int i = 0; i <= COUNT_OF_DOCS; i++)
            {
                GetPowerProfileRecord(START_DATE, ref meterInfoList);
                DOCUMENTS_XML80020.CreateDocXml80020(INFO?.COMPANY_INN ?? "", COMPANY_NAME, INFO?.SENDER_INN ?? "",
                    INFO?.SENDER_NAME ?? "", START_DATE, PATH_TO_XML80020_REPORT,
                    reportFormParam?.reportDate?.Replace(".", "") ?? "", MSG_NUMBER.ToString(), meterInfoList, ref GenFloatSum);

                START_DATE = START_DATE.AddDays(1);
                MSG_NUMBER++;
            }

            DateTime reportDate;
            DateTime.TryParse(reportFormParam?.reportDate ?? "", out reportDate);
            reportHistory.ReportsDate = reportDate;

            dataBase.ReportsHistories.Add(reportHistory);
            dataBase.SaveChanges();

            /*reportHistoryId = (from t in dataBase.ReportsHistories
                               where t.CompanyInn == reportHistory.CompanyInn
                                   && t.ReportsDate == reportHistory.ReportsDate && t.PeriodStartDate == reportHistory.PeriodStartDate
                                   && t.PeriodEndDate == reportHistory.PeriodEndDate && t.RecordCreateDate == reportHistory.RecordCreateDate
                               select t.Id).FirstOrDefault();*/
            reportHistoryId = (from t in dataBase.ReportsHistories
                               where t.RecordCreateDate == reportHistory.RecordCreateDate
                               select t.Id).FirstOrDefault();

            if (GetArchivesForReport())
            {
                return GOOD_RESULT;
            }
            else
            {
                return BAD_RESULT;
            }

        }

        public bool GetArchivesForReport()
        {
            try
            {
                if (PATH_TO_XML80020_REPORT != null)
                {
                    /*
                     * Заметка: не работает ZipFile.CreateFromDirectory(PATH_TO_XML80020_REPORT...) и
                     * Directory.Delete(PATH_TO_XML80020_REPORT...) при запуске приложения в докер контейнере сформированном через консоль.

                    /*                    logger.LogCritical("Path to xml 80020: " + PATH_TO_XML80020_REPORT + ". Timeout...");
                                        ZipFile.CreateFromDirectory(PATH_TO_XML80020_REPORT, PATH_TO_XML80020_REPORT + ".zip");
                                        Directory.Delete(PATH_TO_XML80020_REPORT, true);
                    */
                    ZipFile.CreateFromDirectory(PATH_TO_REPORTS, PATH_TO_REPORTS + ".zip");
                    Directory.Delete(PATH_TO_REPORTS, true);

                    return true;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                return false;
            }
        }

        string? GetPathToReport(string companyInn, DateTime startDate)
        {
            string? contract = (from t in dataBase.MsgNumbers
                                where t.CompanyInn == companyInn
                                select t.Contract).FirstOrDefault()?.ToString();
            if (contract != null)
            {
                string _month = startDate.Month.ToString();
                if (startDate.Month < 10)
                {
                    _month = _month.Insert(0, "0");
                }
                string pathZipReport = PATH_TO_REPORTS + @"/" + companyInn + "_" + contract + "_" +
                    _month + "_" + startDate.Year.ToString();

                return pathZipReport;
            }

            return null;
        }
        List<MeterInfo> GetMeterInfoList(string CompanyInn)
        {
            List<MeterInfo> meterInfoList = (from t in dataBase.Meters
                                             where t.Inn == CompanyInn
                                             select new MeterInfo
                                             {
                                                 meter_id = t.IdMeter.ToString(),
                                                 MeasuringChannelAnameMI = t.MeasuringchannelA ?? "",
                                                 MeasuringChannelRnameMI = t.MeasuringchannelR ?? "",
                                                 MeausuringPointNameMI = t.MeasuringpointName ?? "",
                                                 xml80020code = t.Xml80020code ?? "",
                                                 transformation_ratio = t.TransformationRatio ?? ""
                                             }).OrderBy(t => t.MeausuringPointNameMI).ToList();
            return meterInfoList;
        }
        void GetPowerProfileRecord(DateTime startDate, ref List<MeterInfo> meterInfoList)
        {
            DateTime nextDate = startDate.AddDays(1);

            foreach (MeterInfo meterInfoObj in meterInfoList)
            {
                meterInfoObj.PowerProfileTableRecList = new List<PowerProfileTableRec>();

                string rawSql = "select * from power_profile_m where id = @meterID and date = @startDate and time > '00:00'";

                List<MySqlParameter> rawSqlParams = new List<MySqlParameter>();
                rawSqlParams.Add(new MySqlParameter("@meterID", meterInfoObj.meter_id));
                rawSqlParams.Add(new MySqlParameter("@startDate", startDate.ToString("yyyy-MM-dd")));

                meterInfoObj.PowerProfileTableRecList = dataBase.PowerProfileMs.FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                                                                    Select(t => new PowerProfileTableRec()
                                                                    {
                                                                        RowNumber = t.RowNumber,
                                                                        Id = t.Id,
                                                                        Address = t.Address,
                                                                        Date = t.Date,
                                                                        Time = t.Time,
                                                                        Pplus = t.Pplus,
                                                                        Pminus = t.Pminus,
                                                                        Qplus = t.Qplus,
                                                                        Qminus = t.Qminus

                                                                    }).
                                                                    OrderBy(t => t.Time).
                                                                    ToList();

                rawSql = "select * from power_profile_m where id = @meterID and date = @nextDate and time = '00:00'";
                rawSqlParams.Clear();
                rawSqlParams.Add(new MySqlParameter("@meterID", meterInfoObj.meter_id));
                rawSqlParams.Add(new MySqlParameter("@nextDate", nextDate.ToString("yyyy-MM-dd")));

                meterInfoObj.PowerProfileTableRecList_2330_0000 = dataBase.PowerProfileMs.FromSqlRaw(rawSql, rawSqlParams.ToArray()).
                                                                    Select(t => new PowerProfileTableRec()
                                                                    {
                                                                        RowNumber = t.RowNumber,
                                                                        Id = t.Id,
                                                                        Address = t.Address,
                                                                        Date = t.Date,
                                                                        Time = t.Time,
                                                                        Pplus = t.Pplus,
                                                                        Pminus = t.Pminus,
                                                                        Qplus = t.Qplus,
                                                                        Qminus = t.Qminus

                                                                    }).FirstOrDefault() ?? new PowerProfileTableRec()
                                                                    {
                                                                        RowNumber = 0,
                                                                        Id = 0,
                                                                        Address = 0,
                                                                        Date = startDate,
                                                                        Time = "00:00",
                                                                        Pplus = null,
                                                                        Pminus = null,
                                                                        Qplus = null,
                                                                        Qminus = null
                                                                    };
            }
        }
    }
}
