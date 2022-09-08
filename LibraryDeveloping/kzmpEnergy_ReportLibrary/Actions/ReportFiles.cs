using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KzmpEnergyReportLibrary.Variables;

namespace KzmpEnergyReportLibrary.Actions
{
    public class ReportFiles
    {
        internal static string GetPathToSaveDoc(string companyInn, DateTime currentDate, string msgNumber, string pathToSource, out string currentDateStr)
        {
            string currentMonth = Convert.ToString(currentDate.Month);
            if (currentMonth.Length == 1)
            {
                currentMonth = currentMonth.Insert(0, "0");
            }

            string currentDay = Convert.ToString(currentDate.Day);
            if (currentDay.Length == 1)
            {
                currentDay = currentDay.Insert(0, "0");
            }

            currentDateStr = Convert.ToString(currentDate.Year) + currentMonth + currentDay;
            string doc_path = pathToSource + @"\80020_" + companyInn + "_" + currentDateStr + "_" + msgNumber + ".xml";

            return doc_path;
        }

        public static int GetCountOfDocsFromPeriod(DateTime startDateD, DateTime endDateD)
        {
            string startDate = startDateD.ToShortDateString();
            string endDate = endDateD.ToShortDateString();

            var Start = DateTime.Parse(startDate);
            var End = DateTime.Parse(endDate);

            var countVar = (Start - End).Duration();
            return countVar.Days;
        }

        public static string GetEndFolderNameZip(DateTime startDate, string companyContract, string companyInn, string sourceFolder)
        {
            string startDateShortStr = startDate.ToShortDateString();
            var start = DateTime.Parse(startDateShortStr);
            string month = Convert.ToString(start.Month);
            foreach (string _check_month in CommonVariables.CHECK_MONTH)
            {
                if (month == _check_month)
                {
                    month = month.Insert(0, "0");
                }
            };
            string year = Convert.ToString(start.Year);
            string endFolderName = sourceFolder + "\\" + companyInn + "_" + companyContract + "_" + month + "_" + year + ".zip";

            return endFolderName;
        }
    }


}
