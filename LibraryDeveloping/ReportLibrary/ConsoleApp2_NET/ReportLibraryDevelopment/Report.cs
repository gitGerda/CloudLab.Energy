/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KzmpEnergyReportLibrary.Actions;
using KzmpEnergyReportLibrary.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;


namespace ConsoleApp2_NET.ReportLibraryDevelopment
{
    public class Report
    {
        DocumentXml80020 DOCUMENTS_XML80020;
        kzmp_energyContext dataBase;
        readonly string PATH_TO_REPORTS;
        readonly string PATH_TO_REPORTS_ZIP;

        public Report()
        {
            DOCUMENTS_XML80020 = new DocumentXml80020();

            string currentDateStr = Convert.ToString(DateTime.Now).Replace(".", "-").Replace(" ", "_").Replace(":", "-");

            PATH_TO_REPORTS = Environment.CurrentDirectory.ToString() + @"\ReportsXML80020_Documents\" + currentDateStr;
            PATH_TO_REPORTS_ZIP = PATH_TO_REPORTS + ".zip";

            if (!Directory.Exists(PATH_TO_REPORTS))
            {
                try
                {
                    Directory.CreateDirectory(PATH_TO_REPORTS);
                }
                catch (Exception ex)
                {

                };
            }
            dataBase = new kzmp_energyContext();
        }
        public bool CreateReportDevelopment(ReportFormParameters reportFormParam, ref double GenFloatSum)
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

            DateTime.TryParse(reportFormParam?.startDate, out START_DATE);
            DateTime.TryParse(reportFormParam?.endDate, out END_DATE);
            COUNT_OF_DOCS = ReportFiles.GetCountOfDocsFromPeriod(START_DATE, END_DATE);

            bool flagMsgNumberConverting = Int32.TryParse((from t in dataBase.MsgNumbers
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
                || COUNT_OF_DOCS == 0 || flagMsgNumberConverting == false || flagStartDateMsgNumbConv == false || !Directory.Exists(PATH_TO_REPORTS))
            {
                return BAD_RESULT;
            }

            MSG_NUMBER = DOCUMENTS_XML80020.CalcMsgNumber(START_DATE, MSG_NUMBER, START_DATE_FOR_MSG_NUMBER);
            List<MeterInfo> meterInfoList = GetMeterInfoList(INFO.COMPANY_INN);

            if (meterInfoList.Count() == 0)
            {
                return BAD_RESULT;
            }

            for (int i = 0; i < COUNT_OF_DOCS; i++)
            {
                GetPowerProfileRecord(START_DATE, ref meterInfoList);
                DOCUMENTS_XML80020.CreateDocXml80020(INFO.COMPANY_INN, COMPANY_NAME, INFO.SENDER_INN, INFO.SENDER_NAME, START_DATE, PATH_TO_REPORTS,
                    reportFormParam?.reportDate.Replace(".", "") ?? "", MSG_NUMBER.ToString(), meterInfoList, ref GenFloatSum);

                START_DATE = START_DATE.AddDays(1);
                MSG_NUMBER++;
            }
            try
            {
                ZipFile.CreateFromDirectory(PATH_TO_REPORTS, PATH_TO_REPORTS_ZIP);
            }
            catch
            {
                return BAD_RESULT;
            }
            return GOOD_RESULT;
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
                                             })
                                             .OrderBy(t => t.MeausuringPointNameMI)
                                             .ToList();
            return meterInfoList;
        }
        void GetPowerProfileRecord(DateTime startDate, ref List<MeterInfo> meterInfoList)
        {
            DateTime nextDate = startDate.AddDays(1);

            foreach (MeterInfo meterInfoObj in meterInfoList)
            {
                meterInfoObj.PowerProfileTableRecList = new List<PowerProfileTableRec>();

                string rawSql = "select * from dbo.power_profile_m where id = @meterID and date = @startDate and time > '00:00'";

                List<SqlParameter> rawSqlParams = new List<SqlParameter>();
                rawSqlParams.Add(new SqlParameter("@meterID", meterInfoObj.meter_id));
                rawSqlParams.Add(new SqlParameter("@startDate", startDate.ToShortDateString()));

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

                rawSql = "select * from dbo.power_profile_m where id = @meterID and date = @nextDate and time = '00:00'";
                rawSqlParams.Clear();
                rawSqlParams.Add(new SqlParameter("@meterID", meterInfoObj.meter_id));
                rawSqlParams.Add(new SqlParameter("@nextDate", nextDate.ToShortDateString()));

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
*/