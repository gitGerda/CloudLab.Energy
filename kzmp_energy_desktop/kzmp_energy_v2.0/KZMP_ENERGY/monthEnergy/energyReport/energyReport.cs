using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using KZMP_ENERGY.monthEnergy.energyReport;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using System.IO;
using AngleSharp;
using AngleSharp.Dom;

namespace KZMP_ENERGY.monthEnergy.energyReport
{
    public class energyReport
    {
        string companyName;
        string periodOfReport;
        string monthOfReport;
        string monthOfReportName;
        string nextMonthOfReport;
        string yearOfReport;
        string nextYearOfReport;
        string companyInn;

        string connectionString;

        string endFolderName;

        List<meterInfo> metersList = new List<meterInfo>();
        SqlConnection connection;
        SqlCommand cmd;
        public energyReport(string companyName, string month,string year, string companyInn, string endFolder)
        {
            this.endFolderName = endFolder;

            this.companyName = companyName;
            this.companyInn = companyInn;

            this.monthOfReport = month;
            this.yearOfReport = year;

            switch(month)
            {
                case "12": { 
                        nextMonthOfReport = "1";
                        nextYearOfReport=Convert.ToString(Convert.ToInt32(year)+1); 
                        break; }
                default:
                    {
                        nextMonthOfReport = Convert.ToString(Convert.ToInt32(month) + 1);
                        nextYearOfReport = "";
                        break;
                    }
            }

            switch(month)
            {
                case "1": { monthOfReportName = "Январь";break; }
                case "2": { monthOfReportName = "Февраль"; break; }
                case "3": { monthOfReportName = "Март"; break; }
                case "4": { monthOfReportName = "Апрель"; break; }
                case "5": { monthOfReportName = "Май"; break; }
                case "6": { monthOfReportName = "Июнь"; break; }
                case "7": { monthOfReportName = "Июль"; break; }
                case "8": { monthOfReportName = "Август"; break; }
                case "9": { monthOfReportName = "Сентябрь"; break; }
                case "10": { monthOfReportName = "Октябрь"; break; }
                case "11": { monthOfReportName = "Ноябрь"; break; }
                case "12": { monthOfReportName = "Декабрь"; break; }
            }


            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            connection = new SqlConnection(connectionString);

            
        }

        public void createEnergyReport()
        {
            getMeters();
            //1.Получить общие суммы мощностей для каждого счётчика
            getGeneralPowersSum();
            //2.Получить значения энергий
            getEnergy();
            //3.Сформировать html отчёт 
            createEnergyReportHtml(endFolderName);
        }

        public async void createEnergyReportHtml(string endFolder)
        {
            try 
            {
                string htmlLayoutPath = Environment.CurrentDirectory+@"\htmlLayouts\energyReportLayout.html";
                string reportFilePath = endFolder + @"\..\EnergyReport.html";
               
                FileInfo fileInf = new FileInfo(htmlLayoutPath);
                if(fileInf.Exists)
                {
                    fileInf.CopyTo(reportFilePath,true);
                }

                string htmlReportContext = "";

                using (FileStream fs = File.OpenRead(reportFilePath))
                {
                    byte[] array = new byte[fs.Length];
                    fs.Read(array, 0, array.Length);
                    htmlReportContext = System.Text.Encoding.UTF8.GetString(array);
                }
                File.Delete(reportFilePath);

                IBrowsingContext context = BrowsingContext.New();
                AngleSharp.Dom.IDocument doc = await context.OpenAsync(req => req.Content(htmlReportContext));

                //ЗАПОЛНЕНИЕ ПОЛЯ "ДАТА ФОРМИРОВАНИЯ ОТЧЁТА"
                IElement reportDateInput = doc.QuerySelector("#reportDateInput");
                reportDateInput.SetAttribute("value", DateTime.Now.ToString());
                //reportDateInput.TextContent = DateTime.Now.ToString();

                //ЗАПОЛНЕНИЕ ПОЛЯ "НАИМЕНОВАНИЕ ОРГАНИЗАЦИИ"
                IElement companyNameInput = doc.QuerySelector("#companyNameInput");
                string compNamInText = companyName + " (ИНН: " + companyInn + " )";
                companyNameInput.SetAttribute("value", compNamInText);
                //companyNameInput.TextContent = companyName + " (ИНН: " + companyInn + " )";

                //ЗАПОЛНЕНИЕ ПОЛЯ "ПЕРИОД"
                IElement periodInput = doc.QuerySelector("#periodInput");
                string perInpText = monthOfReportName + " " + yearOfReport;

                periodInput.SetAttribute("value", perInpText);


                //ЗАПОЛНЕНИЕ ТАБЛИЦЫ
                IElement tableBody = doc.QuerySelector("#meters_table tbody");
                int i = 1;
                foreach(meterInfo meter in metersList)
                {
                    double powers = Convert.ToDouble(meter.powerSum)/2;
                    double energyTotal = Convert.ToDouble(meter.totalValue);

                    double diff = Math.Abs(powers - energyTotal);
                    string checkColumnStr = "";
                    string rowClassName = "";

                    if(diff<=5 && energyTotal!=0)
                    {
                        rowClassName = "table-success";
                        checkColumnStr = "Ок ( ~"+diff.ToString("0.000")+" )";
                    }
                    else if(diff >5 && diff <=10)
                    {
                        rowClassName = "table-warning";
                        checkColumnStr="Предупреждение ( ~"+ diff.ToString("0.000") + " )";
                    }
                    else
                    {
                        rowClassName = "table-danger";
                        checkColumnStr = "Проверьте данные ( ~" + diff.ToString("0.000") + " )";
                    }

                    IElement row = doc.CreateElement("tr");
                    row.SetAttribute("class", rowClassName);

                    IElement th = doc.CreateElement("th");
                    th.SetAttribute("scope", "row");
                    th.TextContent = Convert.ToString(i);
                    row.AppendChild(th);

                    string[] meterData = new string[] {meter.typeOfmeter, meter.addressOfmeter, meter.transRatio,
                                                      meter.startValue, meter.endValue, meter.totalValue, meter.powerSum,
                                                      meter.date, checkColumnStr};
                    for(int g=0;g<9;g++)
                    {
                        if(String.IsNullOrEmpty(meterData[g]))
                        {
                            meterData[g] = "NotDefined";
                        }

                        IElement td = doc.CreateElement("td");
                        td.TextContent = meterData[g];
                        row.AppendChild(td);
                    }

                    tableBody.AppendChild(row);

                    i++;
                }

                htmlReportContext = doc.ToHtml();
                doc.Close();

                using (FileStream zx = File.OpenWrite(reportFilePath))
                {
                    byte[] array = System.Text.Encoding.UTF8.GetBytes(htmlReportContext);
                    zx.Write(array, 0, array.Length);
                }

                System.Diagnostics.Process.Start(reportFilePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void getMeters()
        {
            try
            {
                string sql_cmd = "select id_meter, type, address,Interface, Transformation_ratio " +
                             "from dbo.meter " +
                             "where INN like '" + companyInn + "';";
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();

                    cmd = new SqlCommand(sql_cmd, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            meterInfo meterObj = new meterInfo();
                            meterObj.id = Convert.ToString(reader.GetValue(0));
                            meterObj.typeOfmeter = Convert.ToString(reader.GetValue(1));
                            meterObj.addressOfmeter = Convert.ToString(reader.GetValue(2));
                            meterObj.meterInter = Convert.ToString(reader.GetValue(3));
                            meterObj.transRatio = Convert.ToString(reader.GetValue(4));

                            metersList.Add(meterObj);
                        }
                    }
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void getGeneralPowersSum()
        {
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }

                foreach (meterInfo meterObj in metersList)
                {
                    string periodStart = "01." + monthOfReport + "." + yearOfReport;
                    string periodEnd = "01." + nextMonthOfReport + ".";
                    double powerSum = 0;

                    if (nextYearOfReport!="")
                    {
                        periodEnd += nextYearOfReport;
                    }
                    else
                    {
                        periodEnd += yearOfReport;
                    }

                    string sql_cmd = "select Pplus " +
                                     "from dbo.power_profile_m " +
                                     "where date>='"+periodStart+"' and date<'"+periodEnd+"' " +
                                        "and (date!='"+periodStart+"' or time!='00:00:00') and id= "+meterObj.id+";";

                    cmd = new SqlCommand(sql_cmd,connection);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string pPlus = Convert.ToString(reader.GetValue(0));

                            double pPlusDoub = Convert.ToDouble(pPlus);
                            powerSum += pPlusDoub;
                        }
                    }
                    reader.Close();


                    sql_cmd = "select Pplus " +
                              "from dbo.power_profile_m " +
                              "where date='" + periodEnd + "' and time='00:00:00' and id= "+ meterObj.id + ";";
                    cmd = new SqlCommand(sql_cmd, connection);

                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string pPlus = Convert.ToString(reader.GetValue(0));

                            double pPlusDoub = Convert.ToDouble(pPlus);
                            powerSum += pPlusDoub;
                        }
                    }
                    reader.Close();

                    meterObj.powerSum = Convert.ToString(powerSum);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void getEnergy()
        {
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }

                foreach (meterInfo meterObj in metersList)
                {
                    string sql_cmd = "select StartValue, EndValue, Total, Date " +
                                     "from dbo.EnergyTable " +
                                     "where MeterID = " + meterObj.id+
                                     " and Year = "+yearOfReport+" and Month='"+monthOfReportName+"';";

                    cmd = new SqlCommand(sql_cmd, connection);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            meterObj.startValue = Convert.ToString(reader.GetValue(0));
                            meterObj.endValue = Convert.ToString(reader.GetValue(1));
                            meterObj.totalValue = Convert.ToString(reader.GetValue(2));
                            meterObj.date = Convert.ToString(reader.GetValue(3));
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
