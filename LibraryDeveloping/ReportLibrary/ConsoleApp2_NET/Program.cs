using KzmpEnergyReportLibrary.Actions;
using ConsoleApp2_NET.ReportLibraryDevelopment;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ConsoleApp2_NET
{
    public class Program
    {
        public static void Main()
        {
            /*ReportFormParameters reportFormParameters = new ReportFormParameters()
            {
                companyName = "АО Кукморский завод Металлопосуды",
                reportDate = "21.04.2022",
                startDate = "01.10.2021",
                endDate = "01.11.2021",
                energyReportMonth = "",
                energyReportYear = ""
            };
            double GeneralPowersSum = 0;

            Report report = new Report();
            bool resultFlag = report.CreateReportDevelopment(reportFormParameters, ref GeneralPowersSum);
            Console.WriteLine(GeneralPowersSum);

            Console.WriteLine(DateTime.Now.Millisecond.ToString());*/
            /*kzmp_energyContext database = new kzmp_energyContext();

            string rawSQL = "select Pplus " +
                "from dbo.power_profile_m where date>='@periodStart' and date<'@periodEnd' " +
                                "and (date!='@periodStart' or time!='00:00:00') and id= @meterId";

            List<SqlParameter> rawSqlParams = new List<SqlParameter>();
            rawSqlParams.Add(new SqlParameter("@periodStart", "01.10.21"));
            rawSqlParams.Add(new SqlParameter("@periodEnd", "01.11.21"));
            rawSqlParams.Add(new SqlParameter("@meterID", "8"));

            var res1 = (from t in database.PowerProfileMs
                        where t.Date >= Convert.ToDateTime("01.10.21") &&
                              t.Date < Convert.ToDateTime("01.11.21") &&
                              (t.Date != Convert.ToDateTime("01.10.21") || t.Time != TimeSpan.Zero) &&
                              t.Id == 8
                        select t.Pplus).Sum();*/

            // var res1 = database.PowerProfileMs.FromSqlRaw(rawSQL, rawSqlParams.ToArray()).ToList();
            /*var meters = (from t in database.Meters
                          where t.Inn == "1623000219"
                          select new
                          {
                              id = t.IdMeter,
                              type = t.Type,
                              address = t.Address,
                              transRatio = t.TransformationRatio
                          }).ToList();*/
            /* DateTime reportsDate;
             DateTime periodStartDate;
             DateTime periodEndDate;

             DateTime.TryParse("29.04.22", out reportsDate);
             DateTime.TryParse("01.04.22", out periodStartDate);
             DateTime.TryParse("29.04.22", out periodEndDate);

             ReportsHistory reportHist = new ReportsHistory()
             {
                 CompanyName = "1",
                 ReportsDate = reportsDate,
                 PeriodStartDate = periodStartDate,
                 PeriodEndDate = periodEndDate,
                 CompanyInn = "testInn"
             };

             int? idOfReportHostoryRec = (from t in database.ReportsHistories
                                          where t.CompanyInn == reportHist.CompanyInn && t.CompanyInn == reportHist.CompanyInn
                                              && t.ReportsDate == reportHist.ReportsDate && t.PeriodStartDate == reportHist.PeriodStartDate
                                              && t.PeriodEndDate == reportHist.PeriodEndDate
                                          select t.Id).FirstOrDefault();*/


            var dataBase = new kzmp_energyContext();

            var response = (from t in dataBase.ReportsHistories
                            select t).Skip(1).Take(10).OrderBy(t=>t.Id).ToList();
            string g = JsonSerializer.Serialize(response);
            
            Console.WriteLine();
            Console.ReadKey();
        }
    }

    
}
