using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Globalization;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;

namespace kzmpCloudAPI.Components.ReportsXML80020.EnergyReport
{
    public class EnergyReport
    {
        kzmp_energyContext database;
        readonly int REPORT_ID;
        readonly string COMPANY_INN;
        readonly DateTime REPORT_DATE;
        readonly string START_DATE;
        readonly string END_DATE;
        readonly string _dbConnStr;
        public EnergyReport(kzmp_energyContext _db, int _report_id, string _company_inn, DateTime _reportDate, string dbConnStr)
        {
            database = _db;
            REPORT_ID = _report_id;
            COMPANY_INN = _company_inn;
            REPORT_DATE = _reportDate;
            _dbConnStr = dbConnStr;

            int startMonth = _reportDate.Month;
            int startYear = _reportDate.Year;

            START_DATE = "01." + startMonth.ToString() + "." + startYear.ToString();

            if (startMonth == 12)
            {
                END_DATE = "01.01." + Convert.ToString(startYear + 1);
            }
            else
            {
                END_DATE = "01." + Convert.ToString(startMonth + 1) + "." + startYear.ToString();
            }
        }

        public List<ReportsDetalization> CreateEnergyReport()
        {
            // CultureInfo.CurrentCulture = new CultureInfo("ru-RU");

            List<MeterInfoModel> meters = (from t in database.Meters
                                           where t.Inn == COMPANY_INN
                                           select new MeterInfoModel
                                           {
                                               id = t.IdMeter,
                                               type = t.Type ?? "",
                                               address = t.Address.ToString(),
                                               transRatio = t.TransformationRatio ?? ""
                                           }).ToList();

            List<ReportsDetalization> reportDetalizList = new List<ReportsDetalization>();

            foreach (MeterInfoModel meter in meters)
            {
                //POWER SUM GETTING PART
                double? powerSum1 = 0;
                double? powerSum2 = 0;
                int meterAddr = 0;

                powerSum1 = (from t in database.PowerProfileMs
                             where t.Date >= Convert.ToDateTime(START_DATE) &&
                                   t.Date < Convert.ToDateTime(END_DATE) &&
                                   (t.Date != Convert.ToDateTime(START_DATE) || t.Time != TimeSpan.Zero) &&
                                   t.Id == meter.id
                             select t.Pplus).Sum();

                powerSum2 = (from t in database.PowerProfileMs
                             where t.Date == Convert.ToDateTime(END_DATE) &&
                                   t.Time == TimeSpan.Zero &&
                                   t.Id == meter.id
                             select t.Pplus).FirstOrDefault();

                meter.powerSum = Convert.ToString(powerSum1 + powerSum2) ?? "NotDefined";

                //ENERGY GETTING PART
                var energy = (from t in database.EnergyTables
                              where t.MeterId == meter.id && t.Year == REPORT_DATE.Year && t.Month == REPORT_DATE.Month.ToString()
                              select t).FirstOrDefault();

                if (energy != null)
                {
                    meter.startValue = energy.StartValue ?? "";
                    meter.endValue = energy.EndValue ?? "";
                    meter.totalValue = energy.Total ?? "";
                    meter.date = energy.Date ?? "";
                }

                ReportsDetalization reportDetalizRec = new ReportsDetalization()
                {
                    ReportId = REPORT_ID,
                    TblColMeterType = meter.type,
                    TblColTransformRatio = meter.transRatio,
                    TblColPeriodStart = meter.startValue,
                    TblColPeriodEnd = meter.endValue,
                    TblColEnergySum = meter.totalValue,
                    TblColPowerSum = meter.powerSum,
                    TblColDateOfRead = meter.date
                };

                if (int.TryParse(meter.address, out meterAddr))
                {
                    reportDetalizRec.TblColMeterAddress = meterAddr;
                }

                try
                {
                    double powers = Convert.ToDouble(meter.powerSum) / 2;
                    double energyTotal = Convert.ToDouble(meter.totalValue?.Replace(".", ","));
                    double diff = Math.Abs(powers - energyTotal);

                    if (diff <= 5 && energyTotal != 0)
                    {
                        reportDetalizRec.DetalizationType = "table-success";
                        reportDetalizRec.TblColCheck = "Ок ( ~" + diff.ToString("0.000") + " )";
                    }
                    else if (diff > 5 && diff <= 10)
                    {
                        reportDetalizRec.DetalizationType = "table-warning";
                        reportDetalizRec.TblColCheck = "Предупреждение ( ~" + diff.ToString("0.000") + " )";
                    }
                    else
                    {
                        reportDetalizRec.DetalizationType = "table-danger";
                        reportDetalizRec.TblColCheck = "Проверьте данные ( ~" + diff.ToString("0.000") + " )";
                    }
                }
                catch
                {
                    reportDetalizRec.DetalizationType = "table-danger";
                    reportDetalizRec.TblColCheck = "Не удалось провести проверку";
                }

                reportDetalizList.Add(reportDetalizRec);
                database.ReportsDetalizations.Add(reportDetalizRec);
            }
            database.SaveChanges();
            return reportDetalizList;
        }
    }
}
