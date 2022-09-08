using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System.Configuration;
using System.IO.Compression;
using KZMP_ENERGY.monthEnergy.energyReport;

namespace KZMP_ENERGY
{
    public partial class FormReport : Form
    {
        List<MeausuringPointInfo> meausuringList = new List<MeausuringPointInfo>()
        {
            new MeausuringPointInfo()
            {
                network_address =11,
                code ="040055560001101",
                MeausuringPointName ="Металлопосуда ввод 1 на Металлопосуда ввод 1",
                MeasuringChannelAname = "Металлопосуда 1 (А+) на Металлопосуда ввод 1",
                MeasuringChannelRname = "Металлопосуда 1 (R+) на Металлопосуда ввод 1"
            },
            new MeausuringPointInfo()
            {
                network_address =1,
                code ="040055560001102",
                MeausuringPointName ="Металлопосуда ввод 2 на Металлопосуда ввод 2",
                MeasuringChannelAname = "Металлопосуда 2 (А+) на Металлопосуда ввод 2",
                MeasuringChannelRname = "Металлопосуда 2 (R+) на Металлопосуда ввод 2"
            },
            new MeausuringPointInfo()
            {
                network_address =19,
                code ="040055560001103",
                MeausuringPointName ="Металлопосуда ввод 3 на Металлопосуда ввод 3",
                MeasuringChannelAname = "Металлопосуда 3 (А+) на Металлопосуда ввод 3",
                MeasuringChannelRname = "Металлопосуда 3 (R+) на Металлопосуда ввод 3"
            },
            new MeausuringPointInfo()
            {
                network_address = 8,
                code ="040055560001104",
                MeausuringPointName ="Металлопосуда ввод 4 на Металлопосуда ввод 4",
                MeasuringChannelAname = "Металлопосуда 4 (А+) на Металлопосуда ввод 4",
                MeasuringChannelRname = "Металлопосуда 4 (R+) на Металлопосуда ввод 4"
            }
        };
        List<MeausuringPointInfo> meausuringList2 = new List<MeausuringPointInfo>()
        {
            new MeausuringPointInfo()
            {
                network_address =18,
                code ="045501570004101",
                MeausuringPointName ="Расплав ввод 1 на Расплав ввод 1",
                MeasuringChannelAname = "Расплав 1 (А+) на Расплав ввод 1",
                MeasuringChannelRname = "Расплав 1 (R+) на Расплав ввод 1"
            },
            new MeausuringPointInfo()
            {
                network_address =86,
                code ="045501570004102",
                MeausuringPointName ="Расплав ввод 2 на Расплав ввод 2",
                MeasuringChannelAname = "Расплав 2 (А+) на Расплав ввод 2",
                MeasuringChannelRname = "Расплав 2 (R+) на Расплав ввод 2"
            },
        };
        List<MeasCheckClass> MeasCheckList = new List<MeasCheckClass>();
        List<int> time = new List<int>() {0,30,100,130,200,230,300,330,400,430,500,530,600,630,700,730,800,830,900,930,1000,1030,1100,1130,1200,1230,
        1300,1330,1400,1430,1500,1530,1600,1630,1700,1730,1800,1830,1900,1930,2000,2030,2100,2130,2200,2230,2300,2330};
        List<string> checkMonthInArch = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        List<string> checkTime = new List<string>()
        {
            "0000",
            "0030",
            "0100",
            "0130",
            "0200",
            "0230",
            "0300",
            "0330",
            "0400",
            "0430",
            "0500",
            "0530",
            "0600",
            "0630",
            "0700",
            "0730",
            "0800",
            "0830",
            "0900",
            "0930",

            "1000",
            "1030",
            "1100",
            "1130",
            "1200",
            "1230",
            "1300",
            "1330",
            "1400",
            "1430",
            "1500",
            "1530",
            "1600",
            "1630",
            "1700",
            "1730",
            "1800",
            "1830",
            "1900",
            "1930",

            "2000",
            "2030",
            "2100",
            "2130",
            "2200",
            "2230",
            "2300",
            "2330",
            "0000"
        };
        List<CompanyInfo> CompanyInfoList = new List<CompanyInfo>();
        List<MeterInfo> MeterInfoList = new List<MeterInfo>();

        int GeneralSum = 0; // общая сумма всех значений в отчёте
        double GenFloatSum = 0;
        string fd = "";
        double fdas = 0;

        //static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DBkzmp_energy;Integrated Security=True";
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        string sql_cmd = "";
        SqlCommand cmd;
        public FormReport()
        {
            InitializeComponent();

            //считывание данных из таблицы dbo.msg_number
            string sql_cmd_companyList = "select * from dbo.msg_number";
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();

                    cmd = new SqlCommand(sql_cmd_companyList, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            CompanyInfo localItem = new CompanyInfo();

                            localItem.inn = Convert.ToString(reader.GetValue(0));
                            localItem.name = Convert.ToString(reader.GetValue(1));
                            localItem.contract = Convert.ToString(reader.GetValue(2));
                            localItem.MsgNumber = Convert.ToString(reader.GetValue(3));
                            localItem.date = Convert.ToString(reader.GetValue(4));

                            CompanyInfoList.Add(localItem);
                        }

                    }
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //заполнение comboBox1 данными из таблицы dbo.msg_number
            for (int i = 0; i < CompanyInfoList.Count; i++)
            {
                comboBox1.Items.Add(CompanyInfoList[i].name);
            }
            //инициализация textBox_path
            textBox_path.Text = ConfigurationManager.AppSettings.Get("PathToUploadFile");
        }

        //функция загрузки из базы данных информации о счётчиках 
        private void down_info_meter()
        {
            // string sql_cmd_MetersInfo = "select from dbo";
        }
        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                // MessageBox.Show(FBD.SelectedPath);
                textBox_path.Text = FBD.SelectedPath;
            }
        }

        private async void iconButton1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int monthEnergy = DateTime.Now.Month - 1;
                int yearEnergy = DateTime.Now.Year;

                string path = "";
                //Create the object
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //make changes
                config.AppSettings.Settings["PathToUploadFile"].Value = textBox_path.Text;

                //save to apply changes
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                DirectoryInfo dirInfo = new DirectoryInfo(textBox_path.Text);
                if (dirInfo.Exists)
                {
                    path = textBox_path.Text;
                    path = path + @"\" + comboBox1.SelectedItem.ToString().Replace(" ", "");

                    dirInfo = new DirectoryInfo(path);
                    if (!dirInfo.Exists)
                    {
                        try
                        {
                            dirInfo.Create();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                    }
                    string monthToPath = "";
                    switch (DateTime.Now.Month)
                    {
                        case 1:
                            {
                                monthToPath = "Январь";
                                monthEnergy = 12;
                                yearEnergy = yearEnergy - 1;
                                break;
                            }
                        case 2:
                            {
                                monthToPath = "Февраль";
                                break;
                            }
                        case 3:
                            {
                                monthToPath = "Март";
                                break;
                            }
                        case 4:
                            {
                                monthToPath = "Апрель";
                                break;
                            }
                        case 5:
                            {
                                monthToPath = "Май";
                                break;
                            }
                        case 6:
                            {
                                monthToPath = "Июнь";
                                break;
                            }
                        case 7:
                            {
                                monthToPath = "Июль";
                                break;
                            }
                        case 8:
                            {
                                monthToPath = "Август";
                                break;
                            }
                        case 9:
                            {
                                monthToPath = "Сентябрь";
                                break;
                            }
                        case 10:
                            {
                                monthToPath = "Октябрь";
                                break;
                            }
                        case 11:
                            {
                                monthToPath = "Ноябрь";
                                break;
                            }
                        case 12:
                            {
                                monthToPath = "Декабрь";
                                break;
                            }
                    }




                    path = path + @"\" + DateTime.Now.Day.ToString() + "-" + monthToPath + "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();
                    dirInfo = new DirectoryInfo(path);
                    if (!dirInfo.Exists)
                    {
                        try
                        {
                            dirInfo.Create();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(@"Не удалось найти директорию указанную в поле ""Путь для сохранения отчёта""");
                    return;
                }


                GeneralSum = 0;
                GenFloatSum = 0;

                await Task.Run(() => GetMeterInfo());

                int documents_count = 0;
                await Task.Run(() => doc_count_f(ref documents_count));

                string startDateStr = datePicker_begin.Value.ToShortDateString();
                var startDate = DateTime.Parse(startDateStr);
                int current_msg_number = 0;
                try
                {
                    if (!(connection.State == ConnectionState.Open))
                    {
                        connection.Open();
                    }
                    current_msg_number = Convert.ToInt32(textBox3.Text);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                string endFolderZIP = getEndFolderNameZIP(path);
                string endFolder = endFolderZIP.Replace(".zip", "");

                dirInfo = new DirectoryInfo(endFolder);
                if (!dirInfo.Exists)
                {
                    try
                    {
                        dirInfo.Create();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }

                if (cb_monthEnergy.Checked)
                {
                    int indexMonth = comboBox_Months.SelectedIndex + 1;
                    energyReport enerReport = new energyReport(comboBox1.SelectedItem.ToString(), Convert.ToString(indexMonth),
                        textBoxYearEnergy.Text, textBox2.Text, endFolder);
                    await Task.Run(() => enerReport.createEnergyReport());
                }

                //timestamp date
                string StartData = datePickerStart.Value.ToShortDateString();
                string StartTime = timePickerStart.Value.ToShortTimeString();
                var Start = DateTime.Parse(StartData);
                string month = Convert.ToString(Start.Month);
                if (month.Length == 1)
                {
                    month = month.Insert(0, "0");
                }
                string day = Convert.ToString(Start.Day);
                if (day.Length == 1)
                {
                    day = day.Insert(0, "0");
                }
                if (StartTime.Contains(":"))
                {
                    StartTime = StartTime.Replace(":", "");
                }
                StartData = Convert.ToString(Start.Year) + month + day + StartTime;

                await Task.Run(() => statusStart());
                for (int i = 0; i < documents_count; i++)
                {
                    string current_msg_number_str = Convert.ToString(current_msg_number);
                    await Task.Run(() => createXml80020(textBox2.Text, startDate, current_msg_number_str, StartData, endFolder));
                    await Task.Run(() => startDateAddDay(ref startDate, ref current_msg_number));
                }

                await Task.Run(() => statusEnd());
                await Task.Run(() => GetZip(endFolder, endFolderZIP));



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nБудет выполнен рестарт.");
                Application.Restart();
            }

        }

        private void statusStart()
        {
            richTextBox_conStatus2.AppendText("\n----------------------------------------------------------------------------\nФОРМИРОВАНИЕ ОТЧЁТА\n----------------------------------------------------------------------------");
            richTextBox_conStatus2.ScrollToCaret();
        }
        private void statusEnd()
        {
            richTextBox_conStatus2.AppendText("\nОтчёт сформирован.");
            richTextBox_conStatus2.ScrollToCaret();
        }
        public string getEndFolderNameZIP(string path)
        {
            string sourceFolder = path;

            string contract = "";

            for (int i = 0; i < CompanyInfoList.Count; i++)
            {
                if (textBox2.Text == CompanyInfoList[i].inn)
                {
                    contract = CompanyInfoList[i].contract;
                    break;
                }
            }

            string startAddress = datePicker_begin.Value.ToShortDateString();
            var start = DateTime.Parse(startAddress);

            string month = Convert.ToString(start.Month);
            for (int i = 0; i < checkMonthInArch.Count; i++)
            {
                if (month == checkMonthInArch[i])
                {
                    month = month.Insert(0, "0");
                }
            }
            string year = Convert.ToString(start.Year);

            string endFolderName = sourceFolder + "\\" + textBox2.Text + "_" + contract + "_" + month + "_" + year + ".zip";

            return endFolderName;
        }
        //функция архивации файлов xml80020
        private async void GetZip(string sourceFolder, string endFolderName)
        {
            f(sourceFolder, endFolderName);

            richTextBox_conStatus2.AppendText("\n----------------------------------------------------------------------------\nАРХИВАЦИЯ\n----------------------------------------------------------------------------");
            richTextBox_conStatus2.ScrollToCaret();

            richTextBox_conStatus2.AppendText("\n" + endFolderName);
            richTextBox_conStatus2.ScrollToCaret();

            MessageBox.Show("Отчет готов!\nОбщая сумма значений профиля мощности (с учетом трансформации) за выбранный период: " + Convert.ToString(GenFloatSum) + " кВт\nБудет выполнен рестарт программы!\n");

            await Task.Run(() => RestartApp());
        }
        private void RestartApp()
        {
            Application.Restart();
        }
        public static void f(string source, string end)
        {
            ZipFile.CreateFromDirectory(source, end);
        }
        private void startDateAddDay(ref DateTime startDate, ref int current_msg_number)
        {
            startDate = startDate.AddDays(1);
            current_msg_number = current_msg_number + 1;
        }
        //функция подсчёта кол-ва документов
        private void doc_count_f(ref int documents_count)
        {
            string startDate = datePicker_begin.Value.ToShortDateString();
            string endDate = datePicker_end.Value.ToShortDateString();

            var Start = DateTime.Parse(startDate);
            var End = DateTime.Parse(endDate);

            var countVar = (Start - End).Duration();

            documents_count = countVar.Days;
        }
        //функция создания документа xml80020
        private void createXml80020(string inn, DateTime currentDate, string current_msg_number, string timestamp_txt, string pathToSource)
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

            string currentDateStr = Convert.ToString(currentDate.Year) + currentMonth + currentDay;
            string doc_path = pathToSource + "/80020_" + inn + "_" + currentDateStr + "_" + current_msg_number + ".xml";
            richTextBox_conStatus2.AppendText("\n" + doc_path);
            richTextBox_conStatus2.ScrollToCaret();


            string path = Environment.CurrentDirectory + @"\XMLFile1.xml";
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                fileInf.CopyTo(doc_path, true);
            }

            //создание файла xml
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(doc_path);
            XmlElement xRoot = xDoc.DocumentElement;

            xRoot.SetAttribute("number", current_msg_number);

            #region <datetime></datetime><sender></sender>
            //<datetime></datetime>
            XmlElement datetimeElem = xDoc.CreateElement("datetime");
            XmlElement timestampElem = xDoc.CreateElement("timestamp");
            XmlElement daylightsavingtimeElem = xDoc.CreateElement("daylightsavingtime");
            XmlElement dayElem = xDoc.CreateElement("day");

            XmlText timestampText = xDoc.CreateTextNode(timestamp_txt);
            XmlText daylightsavingtimeText = xDoc.CreateTextNode("1");
            XmlText dayText = xDoc.CreateTextNode(currentDateStr);

            dayElem.AppendChild(dayText);
            daylightsavingtimeElem.AppendChild(daylightsavingtimeText);
            timestampElem.AppendChild(timestampText);

            datetimeElem.AppendChild(timestampElem);
            datetimeElem.AppendChild(daylightsavingtimeElem);
            datetimeElem.AppendChild(dayElem);

            xRoot.AppendChild(datetimeElem);

            //<sender></sender>
            XmlElement SenderElem = xDoc.CreateElement("Sender");
            XmlElement SenderInnElem = xDoc.CreateElement("inn");
            XmlElement SenderNameElem = xDoc.CreateElement("name");

            XmlText SenderInnText = xDoc.CreateTextNode("1657055576");
            XmlText SenderNameText = xDoc.CreateTextNode("ООО \"Центр Электронных Услуг\"");

            SenderInnElem.AppendChild(SenderInnText);
            SenderNameElem.AppendChild(SenderNameText);

            SenderElem.AppendChild(SenderInnElem);
            SenderElem.AppendChild(SenderNameElem);

            xRoot.AppendChild(SenderElem);
            #endregion

            //<area></area>
            XmlElement areaElem = xDoc.CreateElement("area");
            XmlElement innElem = xDoc.CreateElement("inn");
            XmlElement nameElem = xDoc.CreateElement("Name");

            XmlText innText = xDoc.CreateTextNode(inn);
            XmlText nameText = xDoc.CreateTextNode(comboBox1.Text);

            innElem.AppendChild(innText);
            nameElem.AppendChild(nameText);

            areaElem.AppendChild(innElem);
            areaElem.AppendChild(nameElem);

            int meterCount = MeterInfoList.Count;

            /*if (textBox2.Text == "1623000219")
            {
                meterCount = 4;
            }
            else if (textBox2.Text == "1623013176")
            {
                meterCount = 2;
            }*/

            for (int mainIndex = 0; mainIndex < meterCount; mainIndex++)
            {
                XmlElement measuringpointElem = xDoc.CreateElement("measuringpoint");

                XmlAttribute pointCodeAttr = xDoc.CreateAttribute("code");
                XmlAttribute pointNameAttr = xDoc.CreateAttribute("name");

                // XmlText pointCodeAttrText = xDoc.CreateTextNode(meausuringList2[mainIndex].code);
                // XmlText pointNameAttrText = xDoc.CreateTextNode(meausuringList2[mainIndex].MeausuringPointName);

                XmlText pointCodeAttrText = xDoc.CreateTextNode(MeterInfoList[mainIndex].xml80020code);
                XmlText pointNameAttrText = xDoc.CreateTextNode(MeterInfoList[mainIndex].MeausuringPointNameMI);

                pointCodeAttr.AppendChild(pointCodeAttrText);
                pointNameAttr.AppendChild(pointNameAttrText);

                measuringpointElem.Attributes.Append(pointCodeAttr);
                measuringpointElem.Attributes.Append(pointNameAttr);

                for (int j = 0; j < 2; j++)
                {
                    XmlElement measuringchannelElem = xDoc.CreateElement("measuringchannel");
                    XmlAttribute channelCodeAttr = xDoc.CreateAttribute("code");
                    XmlAttribute descAttr = xDoc.CreateAttribute("desc");

                    if (j == 0)
                    {
                        XmlText channelCodeAttrText = xDoc.CreateTextNode("01");
                        // XmlText descAttrText = xDoc.CreateTextNode(meausuringList2[mainIndex].MeasuringChannelAname);
                        XmlText descAttrText = xDoc.CreateTextNode(MeterInfoList[mainIndex].MeasuringChannelAnameMI);

                        channelCodeAttr.AppendChild(channelCodeAttrText);
                        descAttr.AppendChild(descAttrText);

                        measuringchannelElem.Attributes.Append(channelCodeAttr);
                        measuringchannelElem.Attributes.Append(descAttr);

                        /*
                        sql_cmd = "select count(*) from dbo.power_profile_m where date = '"+currentDateStr+" and address="+Convert.ToString(meausuringList[i].network_address);
                        SqlCommand cmd = new SqlCommand(sql_cmd, connection);
                        int linesTocurrentDayInBd = Convert.ToInt32(cmd.ExecuteScalar());
                        */


                        /*if (textBox2.Text == "1623000219")
                        {
                            sql_cmd = "select * from dbo.power_profile_m where address=" + Convert.ToString(meausuringList[mainIndex].network_address) + " and date='" + currentDateStr + "' and time >'00:00' order by time";

                        }
                        else if(textBox2.Text == "1623013176")  
                        {
                            sql_cmd = "select * from dbo.power_profile_m where address=" + Convert.ToString(meausuringList2[mainIndex].network_address) + " and date='" + currentDateStr + "' and time >'00:00' order by time";
                            
                        }*/

                        sql_cmd = "select * from dbo.power_profile_m where id =" + MeterInfoList[mainIndex].meter_id + " and date='" + currentDateStr + "' and time >'00:00' order by time";

                        int IndexASd = 0;
                        bool fzxc = false;

                        foreach (MeasCheckClass g in MeasCheckList)
                        {

                            if (g.id == Convert.ToString(MeterInfoList[mainIndex].meter_id))
                            {
                                IndexASd = MeasCheckList.IndexOf(g);
                                fzxc = true;
                                continue;
                            }
                            //IndexASd += 1;
                        }
                        if (fzxc == false)
                        {
                            MeasCheckClass f = new MeasCheckClass();
                            f.id = Convert.ToString(MeterInfoList[mainIndex].meter_id);

                            MeasCheckList.Add(f);

                            IndexASd = MeasCheckList.IndexOf(f);
                        }

                        SqlCommand cmd = new SqlCommand(sql_cmd, connection);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            int counter = 0;
                            // int timeCounter = 0;
                            while (reader.Read())
                            {
                                XmlElement periodElem = xDoc.CreateElement("period");
                                XmlAttribute startAttr = xDoc.CreateAttribute("start");
                                XmlAttribute endAttr = xDoc.CreateAttribute("end");
                                XmlElement valueElem = xDoc.CreateElement("value");

                                string bdTimeStr = Convert.ToString(reader.GetValue(3));
                                if (bdTimeStr.Contains(":"))
                                {
                                    bdTimeStr = bdTimeStr.Replace(":", "");
                                }
                                bdTimeStr = bdTimeStr.Substring(0, 4);
                                int bdTime = Convert.ToInt32(bdTimeStr);


                                for (int i = 0; i < time.Count; i++)
                                {
                                    if (bdTime > time[i] && bdTime <= time[i + 1])
                                    {
                                        bdTime = time[i + 1];
                                    }
                                }

                                bdTime = bdTime - 5;
                                //вычисление атрибута start
                                for (int m = 0; m < time.Count; m++)
                                {
                                    if (bdTime >= time[m] && bdTime < time[m + 1])
                                    {
                                        bdTime = time[m];
                                        bdTimeStr = Convert.ToString(checkTime[m + 1]);
                                    }
                                }
                                string startTime = Convert.ToString(bdTime);
                                int b = 4 - startTime.Length;
                                if (b == 1)
                                    startTime = startTime.Insert(0, "0");
                                if (b == 2)
                                    startTime = startTime.Insert(0, "00");
                                if (b == 3)
                                    startTime = startTime.Insert(0, "000");

                                bool flagLocal = true;
                                while (flagLocal)
                                {
                                    if (startTime != checkTime[counter] && counter < checkTime.Count)
                                    {
                                        XmlElement periodElem3 = xDoc.CreateElement("period");
                                        XmlAttribute startAttr3 = xDoc.CreateAttribute("start");
                                        XmlAttribute endAttr3 = xDoc.CreateAttribute("end");
                                        XmlElement valueElem3 = xDoc.CreateElement("value");

                                        XmlText startAttrText3 = xDoc.CreateTextNode(checkTime[counter]);
                                        XmlText endAttrText3 = xDoc.CreateTextNode(checkTime[counter + 1]);

                                        startAttr3.AppendChild(startAttrText3);
                                        endAttr3.AppendChild(endAttrText3);

                                        XmlText valueElemtext3 = xDoc.CreateTextNode("0");

                                        periodElem3.Attributes.Append(startAttr3);
                                        periodElem3.Attributes.Append(endAttr3);

                                        valueElem3.AppendChild(valueElemtext3);
                                        periodElem3.AppendChild(valueElem3);

                                        measuringchannelElem.AppendChild(periodElem3);
                                        counter++;
                                    }
                                    else
                                    {
                                        flagLocal = false;
                                    }
                                }
                                XmlText startAttrText = xDoc.CreateTextNode(startTime);
                                XmlText endAttrText = xDoc.CreateTextNode(bdTimeStr);

                                startAttr.AppendChild(startAttrText);
                                endAttr.AppendChild(endAttrText);

                                object value_obj = reader.GetValue(4);
                                int K = Convert.ToInt32(MeterInfoList[mainIndex].transforamtion_ratio) / 2;
                                //float value_f = Convert.ToSingle(value_obj) * Convert.ToSingle(K);
                                //int value = Convert.ToInt32(value_f);
                                double value_d = Convert.ToDouble(value_obj) * Convert.ToDouble(K);

                                //GeneralSum = GeneralSum + value;
                                GenFloatSum = GenFloatSum + value_d;
                                MeasCheckList[IndexASd].meas += value_d;

                                XmlText valueElemtext = xDoc.CreateTextNode(Convert.ToString(value_d));

                                periodElem.Attributes.Append(startAttr);
                                periodElem.Attributes.Append(endAttr);

                                valueElem.AppendChild(valueElemtext);

                                periodElem.AppendChild(valueElem);
                                measuringchannelElem.AppendChild(periodElem);

                                counter++;
                            }
                        }
                        reader.Close();
                        //здесь код для времени 2330 - 0000
                        DateTime nextDay = currentDate.AddDays(1);

                        string nextDayDay = Convert.ToString(nextDay.Day);
                        if (nextDayDay.Length == 1)
                        {
                            nextDayDay = nextDayDay.Insert(0, "0");
                        }

                        string nextDayMonth = Convert.ToString(nextDay.Month);
                        if (nextDayMonth.Length == 1)
                        {
                            nextDayMonth = nextDayMonth.Insert(0, "0");
                        }

                        string nextDayStr = Convert.ToString(nextDay.Year) + nextDayMonth + nextDayDay;


                        string sql_cmd2 = "select Pplus from dbo.power_profile_m where id=" + MeterInfoList[mainIndex].meter_id + " and date='" + nextDayStr + "' and time ='00:00'";

                        //fd += Convert.ToString(MeterInfoList[mainIndex].meter_id) + "-->";

                        SqlCommand cmd2 = new SqlCommand(sql_cmd2, connection);
                        SqlDataReader reader2 = cmd2.ExecuteReader();

                        if (reader2.HasRows)
                        {
                            int value2 = 0;
                            double value_d2 = 0;
                            while (reader2.Read())
                            {
                                object value_obj2 = reader2.GetValue(0);
                                int K2 = Convert.ToInt32(MeterInfoList[mainIndex].transforamtion_ratio) / 2;
                                //float value_f2 = Convert.ToSingle(value_obj2) * K2;
                                // value2 = Convert.ToInt32(value_f2);
                                value_d2 = Convert.ToDouble(value_obj2) * Convert.ToDouble(K2);

                                //GeneralSum = GeneralSum + value2;
                                GenFloatSum = GenFloatSum + value_d2;

                                MeasCheckList[IndexASd].meas += value_d2;
                            }

                            XmlElement periodElem2 = xDoc.CreateElement("period");
                            XmlAttribute startAttr2 = xDoc.CreateAttribute("start");
                            XmlAttribute endAttr2 = xDoc.CreateAttribute("end");
                            XmlElement valueElem2 = xDoc.CreateElement("value");

                            XmlText startAttrText2 = xDoc.CreateTextNode("2330");
                            XmlText endAttrText2 = xDoc.CreateTextNode("0000");

                            startAttr2.AppendChild(startAttrText2);
                            endAttr2.AppendChild(endAttrText2);

                            XmlText valueElemtext2 = xDoc.CreateTextNode(Convert.ToString(value_d2));

                            periodElem2.Attributes.Append(startAttr2);
                            periodElem2.Attributes.Append(endAttr2);

                            valueElem2.AppendChild(valueElemtext2);

                            periodElem2.AppendChild(valueElem2);
                            measuringchannelElem.AppendChild(periodElem2);
                        }
                        reader2.Close();

                        fd += Convert.ToString(fdas) + ";\\n";
                        fdas = 0;

                    }
                    else
                    {
                        XmlText channelCodeAttrText = xDoc.CreateTextNode("03");
                        XmlText descAttrText = xDoc.CreateTextNode(MeterInfoList[mainIndex].MeasuringChannelRnameMI);

                        channelCodeAttr.AppendChild(channelCodeAttrText);
                        descAttr.AppendChild(descAttrText);

                        measuringchannelElem.Attributes.Append(channelCodeAttr);
                        measuringchannelElem.Attributes.Append(descAttr);
                        /*if (textBox2.Text == "1623000219")
                        {
                            sql_cmd = "select * from dbo.power_profile_m where address=" + Convert.ToString(meausuringList[mainIndex].network_address) + " and date='" + currentDateStr + "' and time >'00:00' order by time";

                        }
                        else if (textBox2.Text == "1623013176")
                        {
                            sql_cmd = "select * from dbo.power_profile_m where address=" + Convert.ToString(meausuringList2[mainIndex].network_address) + " and date='" + currentDateStr + "' and time >'00:00' order by time";

                        }*/
                        sql_cmd = "select * from dbo.power_profile_m where id =" + MeterInfoList[mainIndex].meter_id + " and date='" + currentDateStr + "' and time >'00:00' order by time";

                        SqlCommand cmd = new SqlCommand(sql_cmd, connection);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            int counter = 0;
                            while (reader.Read())
                            {
                                XmlElement periodElem = xDoc.CreateElement("period");
                                XmlAttribute startAttr = xDoc.CreateAttribute("start");
                                XmlAttribute endAttr = xDoc.CreateAttribute("end");
                                XmlElement valueElem = xDoc.CreateElement("value");

                                string bdTimeStr = Convert.ToString(reader.GetValue(3));
                                if (bdTimeStr.Contains(":"))
                                {
                                    bdTimeStr = bdTimeStr.Replace(":", "");
                                }
                                bdTimeStr = bdTimeStr.Substring(0, 4);
                                int bdTime = Convert.ToInt32(bdTimeStr);

                                for (int i = 0; i < time.Count; i++)
                                {
                                    if (bdTime > time[i] && bdTime <= time[i + 1])
                                    {
                                        bdTime = time[i + 1];
                                    }
                                }

                                bdTime = bdTime - 5;
                                //вычисление атрибута start
                                for (int m = 0; m < time.Count; m++)
                                {
                                    if (bdTime >= time[m] && bdTime < time[m + 1])
                                    {
                                        bdTime = time[m];
                                        bdTimeStr = Convert.ToString(checkTime[m + 1]);
                                    }
                                }
                                string startTime = Convert.ToString(bdTime);
                                int b = 4 - startTime.Length;
                                if (b == 1)
                                    startTime = startTime.Insert(0, "0");
                                if (b == 2)
                                    startTime = startTime.Insert(0, "00");
                                if (b == 3)
                                    startTime = startTime.Insert(0, "000");

                                bool flagLocal = true;
                                while (flagLocal)
                                {
                                    if (startTime != checkTime[counter])
                                    {
                                        XmlElement periodElem3 = xDoc.CreateElement("period");
                                        XmlAttribute startAttr3 = xDoc.CreateAttribute("start");
                                        XmlAttribute endAttr3 = xDoc.CreateAttribute("end");
                                        XmlElement valueElem3 = xDoc.CreateElement("value");

                                        XmlText startAttrText3 = xDoc.CreateTextNode(checkTime[counter]);
                                        XmlText endAttrText3 = xDoc.CreateTextNode(checkTime[counter + 1]);

                                        startAttr3.AppendChild(startAttrText3);
                                        endAttr3.AppendChild(endAttrText3);

                                        XmlText valueElemtext3 = xDoc.CreateTextNode("0");

                                        periodElem3.Attributes.Append(startAttr3);
                                        periodElem3.Attributes.Append(endAttr3);

                                        valueElem3.AppendChild(valueElemtext3);
                                        periodElem3.AppendChild(valueElem3);

                                        measuringchannelElem.AppendChild(periodElem3);
                                        counter++;
                                    }
                                    else
                                    {
                                        flagLocal = false;
                                    }
                                }

                                XmlText startAttrText = xDoc.CreateTextNode(startTime);
                                XmlText endAttrText = xDoc.CreateTextNode(bdTimeStr);

                                startAttr.AppendChild(startAttrText);
                                endAttr.AppendChild(endAttrText);

                                object value_obj = reader.GetValue(6);
                                int K = Convert.ToInt32(MeterInfoList[mainIndex].transforamtion_ratio) / 2;
                                //float value_f = Convert.ToSingle(value_obj) * Convert.ToSingle(K);
                                // int value = Convert.ToInt32(value_f);
                                double value_d = Convert.ToDouble(value_obj) * Convert.ToDouble(K);

                                //GeneralSum = GeneralSum + value;

                                XmlText valueElemtext = xDoc.CreateTextNode(Convert.ToString(value_d));

                                periodElem.Attributes.Append(startAttr);
                                periodElem.Attributes.Append(endAttr);

                                valueElem.AppendChild(valueElemtext);

                                periodElem.AppendChild(valueElem);
                                measuringchannelElem.AppendChild(periodElem);

                                counter++;
                            }
                        }
                        reader.Close();
                        //здесь код для времени 2330 - 0000
                        DateTime nextDay = currentDate.AddDays(1);

                        string nextDayDay = Convert.ToString(nextDay.Day);
                        if (nextDayDay.Length == 1)
                        {
                            nextDayDay = nextDayDay.Insert(0, "0");
                        }

                        string nextDayMonth = Convert.ToString(nextDay.Month);
                        if (nextDayMonth.Length == 1)
                        {
                            nextDayMonth = nextDayMonth.Insert(0, "0");
                        }

                        string nextDayStr = Convert.ToString(nextDay.Year) + nextDayMonth + nextDayDay;

                        /*if (textBox2.Text == "1623000219")
                        {
                            sql_cmd = "select Qplus from dbo.power_profile_m where address=" + Convert.ToString(meausuringList[mainIndex].network_address) + " and date='" + nextDayStr + "' and time ='00:00'";
                        }
                        else if (textBox2.Text == "1623013176")
                        {
                            sql_cmd = "select Qplus from dbo.power_profile_m where address=" + Convert.ToString(meausuringList2[mainIndex].network_address) + " and date='" + nextDayStr + "' and time ='00:00'";
                        }*/
                        string sql_cmd2 = "select Qplus from dbo.power_profile_m where id=" + MeterInfoList[mainIndex].meter_id + " and date='" + nextDayStr + "' and time ='00:00'";

                        SqlCommand cmd2 = new SqlCommand(sql_cmd2, connection);
                        SqlDataReader reader2 = cmd2.ExecuteReader();

                        if (reader2.HasRows)
                        {
                            int value2 = 0;
                            double value_d2 = 0;
                            while (reader2.Read())
                            {
                                object value_obj2 = reader2.GetValue(0);
                                int K2 = Convert.ToInt32(MeterInfoList[mainIndex].transforamtion_ratio) / 2;
                                //float value_f2 = Convert.ToSingle(value_obj2) * K2;
                                //value2 = Convert.ToInt32(value_f2);
                                value_d2 = Convert.ToDouble(value_obj2) * Convert.ToDouble(K2);

                                //GeneralSum = GeneralSum + value2;
                            }
                            XmlElement periodElem2 = xDoc.CreateElement("period");
                            XmlAttribute startAttr2 = xDoc.CreateAttribute("start");
                            XmlAttribute endAttr2 = xDoc.CreateAttribute("end");
                            XmlElement valueElem2 = xDoc.CreateElement("value");

                            XmlText startAttrText2 = xDoc.CreateTextNode("2330");
                            XmlText endAttrText2 = xDoc.CreateTextNode("0000");

                            startAttr2.AppendChild(startAttrText2);
                            endAttr2.AppendChild(endAttrText2);

                            XmlText valueElemtext2 = xDoc.CreateTextNode(Convert.ToString(value_d2));

                            periodElem2.Attributes.Append(startAttr2);
                            periodElem2.Attributes.Append(endAttr2);

                            valueElem2.AppendChild(valueElemtext2);

                            periodElem2.AppendChild(valueElem2);
                            measuringchannelElem.AppendChild(periodElem2);
                        }
                        reader2.Close();

                    }
                    measuringpointElem.AppendChild(measuringchannelElem);
                }
                areaElem.AppendChild(measuringpointElem);
            }
            xRoot.AppendChild(areaElem);

            xDoc.Save(doc_path);
        }

        //checkXML не используется в коде
        #region checkXML
        private void checkXMLtimes(string doc_path)
        {
            XmlDocument xDoc2 = new XmlDocument();
            xDoc2.Load(doc_path);
            XmlElement xRoot2 = xDoc2.DocumentElement;

            for (int i = 0; i < 4; i++)
            {
                for (int g = 0; g < 2; g++)
                {
                    string code = "01";
                    string startAttr = "";
                    string endAttr = "";

                    int counter = 0;

                    if (g == 1)
                    {
                        code = "03";
                    }
                    XmlNode b = xDoc2.DocumentElement;
                    XmlNodeList childnodes = xRoot2.SelectNodes("//message/area/measuringpoint[@code = '" + meausuringList[i].code + "']/measuringchannel[@code='" + code + "']/*");
                    foreach (XmlNode n in childnodes)
                    {
                        startAttr = n.Attributes["start"].Value;
                        endAttr = n.Attributes["end"].Value;

                        bool flag = true;
                        while (flag)
                        {
                            string startAttrLocal = startAttr;
                            string endAttrLocal = endAttr;
                            if (startAttr != checkTime[counter] && endAttr != checkTime[counter + 1])
                            {
                                startAttrLocal = checkTime[counter];
                                endAttrLocal = checkTime[counter + 1];

                                XmlElement periodElem3 = xDoc2.CreateElement("period");
                                XmlAttribute startAttr3 = xDoc2.CreateAttribute("start");
                                XmlAttribute endAttr3 = xDoc2.CreateAttribute("end");
                                XmlElement valueElem3 = xDoc2.CreateElement("value");

                                XmlText startAttrText3 = xDoc2.CreateTextNode(checkTime[counter]);
                                XmlText endAttrText3 = xDoc2.CreateTextNode(checkTime[counter + 1]);

                                startAttr3.AppendChild(startAttrText3);
                                endAttr3.AppendChild(endAttrText3);

                                XmlText valueElemtext3 = xDoc2.CreateTextNode("0");

                                periodElem3.Attributes.Append(startAttr3);
                                periodElem3.Attributes.Append(endAttr3);

                                valueElem3.AppendChild(valueElemtext3);
                                periodElem3.AppendChild(valueElem3);

                                //XmlNode local = xRoot.SelectSingleNode("//message/area/measuringpoint[@code = '" + meausuringList[i].code + "']/measuringchannel[@code='" + code + "']/period[@end='"+startAttrLocal+"']");
                                XmlNode local = xDoc2.SelectSingleNode("//message/area/measuringpoint[@code = '" + meausuringList[i].code + "']/measuringchannel[@code='" + code + "']/period[@end='" + startAttrLocal + "']/.");
                                // xDoc.DocumentElement.InsertBefore(periodElem3, xDoc.DocumentElement.FirstChild);

                                xDoc2.DocumentElement.InsertBefore(periodElem3, xDoc2.DocumentElement.SelectSingleNode("/measuringpoint[@code = '" + meausuringList[i].code + "']/measuringchannel[@code='" + code + "']/period[@end='" + startAttrLocal + "']"));
                                //XElement d = xDoc.Ele
                                // b.InsertBefore(periodElem3, local);
                                //xDoc.DocumentElement.InsertBefore(periodElem3, local);
                                counter++;
                            }
                            else
                            {
                                flag = false;
                            }

                        }
                        counter++;
                    }
                }
            }
            xDoc2.Save(doc_path);

        }

        private void checkXMLtimesLINQ(string doc_path)
        {
            XDocument xDocLinq = XDocument.Load(doc_path);

            string code = "01";

            for (int i = 0; i < 4; i++)
            {

                for (int g = 0; g < 2; g++)
                {
                    string startAttr = "";
                    string endAttr = "";

                    int counter = 0;

                    if (g == 1)
                    {
                        code = "03";
                    }
                    // string xpath = "/area/measuringpoint[@code=\"" + meausuringList[i].code + "\"]/measuringchannel[@code=\"" + code + "\"]/period";
                    //XElement element = xDocLinq.XPathSelectElement(xpath);
                    foreach (XElement xe in xDocLinq.Element("message").Elements())
                    // foreach()
                    {
                        startAttr = xe.Attribute("start").Value;
                        endAttr = xe.Attribute("end").Value;

                        bool flag = true;
                        while (flag)
                        {
                            if (startAttr != checkTime[counter] && endAttr != checkTime[counter + 1])
                            {
                                string startAttrLocal = checkTime[counter];
                                string endAttrLocal = checkTime[counter + 1];

                                XElement periodElem = new XElement("period");

                                XAttribute periodStartAttr = new XAttribute("start", checkTime[counter]);
                                XAttribute periodEndAttr = new XAttribute("end", checkTime[counter + 1]);

                                periodElem.Add(periodStartAttr);
                                periodElem.Add(periodEndAttr);

                                XElement ValueElem = new XElement("value", "0");

                                periodElem.Add(ValueElem);

                                xe.AddBeforeSelf(periodElem);
                                counter++;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        counter++;
                    }

                }
                code = "01";
            }
            xDocLinq.Save(doc_path);
        }
        #endregion

        //функция для определения инн организации в textBox2
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string companyName = comboBox1.Text;
                string inn = "";

                for (int i = 0; i < CompanyInfoList.Count; i++)
                {
                    if (CompanyInfoList[i].name == companyName)
                    {
                        inn = CompanyInfoList[i].inn;
                    }
                }
                textBox2.Text = inn;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //определение msg_mumber
            try
            {
                string beginDate = datePicker_begin.Value.ToShortDateString();
                var beginDateVar = DateTime.Parse(beginDate);

                string dateOfMsgNumber = "";
                int msgfNumberFromDB = 0;

                if (textBox2.Text != "")
                {
                    bool localFlag = false;
                    for (int i = 0; i < CompanyInfoList.Count; i++)
                    {
                        if (textBox2.Text == CompanyInfoList[i].inn)
                        {
                            dateOfMsgNumber = CompanyInfoList[i].date;
                            msgfNumberFromDB = Convert.ToInt32(CompanyInfoList[i].MsgNumber);
                            localFlag = true;
                            break;
                        }
                    }
                    if (localFlag == false)
                    {
                        MessageBox.Show("ИНН организации отсутствует в базе данных.");
                    }
                }

                var DBdateVar = DateTime.Parse(dateOfMsgNumber);
                var count = (beginDateVar - DBdateVar).Duration();

                //MessageBox.Show(Convert.ToString(count));
                textBox3.Text = Convert.ToString(msgfNumberFromDB + Convert.ToInt32(count.Days));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //функция подсчета msg_number на начало периода. Вызов происходит при изменении даты начала периода.
        private void datePicker_begin_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                string beginDate = datePicker_begin.Value.ToShortDateString();
                var beginDateVar = DateTime.Parse(beginDate);

                string dateOfMsgNumber = "";
                int msgfNumberFromDB = 0;

                if (textBox2.Text != "")
                {
                    bool localFlag = false;
                    for (int i = 0; i < CompanyInfoList.Count; i++)
                    {
                        if (textBox2.Text == CompanyInfoList[i].inn)
                        {
                            dateOfMsgNumber = CompanyInfoList[i].date;
                            msgfNumberFromDB = Convert.ToInt32(CompanyInfoList[i].MsgNumber);
                            localFlag = true;
                            break;
                        }
                    }
                    if (localFlag == false)
                    {
                        MessageBox.Show("ИНН организации отсутствует в базе данных.");
                    }
                }

                var DBdateVar = DateTime.Parse(dateOfMsgNumber);
                var count = (beginDateVar - DBdateVar).Duration();

                //MessageBox.Show(Convert.ToString(count));
                textBox3.Text = Convert.ToString(msgfNumberFromDB + Convert.ToInt32(count.Days));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //функция создания листа c элементами класса MeterInfo.
        private void GetMeterInfo()
        {
            string sql_cmd = "select id_meter, MeasuringpointName, MeasuringchannelA, MeasuringchannelR, XML80020code,Transformation_ratio " +
                  "from dbo.meter where INN LIKE '" + textBox2.Text + "' order by XML80020code";
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }

                SqlCommand cmd = new SqlCommand(sql_cmd, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MeterInfo localItem = new MeterInfo();

                        localItem.meter_id = Convert.ToString(reader.GetValue(0));
                        localItem.MeausuringPointNameMI = Convert.ToString(reader.GetValue(1));
                        localItem.MeasuringChannelAnameMI = Convert.ToString(reader.GetValue(2));
                        localItem.MeasuringChannelRnameMI = Convert.ToString(reader.GetValue(3));
                        localItem.xml80020code = Convert.ToString(reader.GetValue(4));
                        localItem.transforamtion_ratio = Convert.ToString(reader.GetValue(5));

                        MeterInfoList.Add(localItem);
                    }
                }
                reader.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FormReport_Load(object sender, EventArgs e)
        {
            textBoxYearEnergy.Text = DateTime.Now.Year.ToString();

            int currentMonth = DateTime.Now.Month;
            string prevMonth = "";
            string yearDate = DateTime.Now.Year.ToString();

            switch (currentMonth)
            {
                case 1:
                    {
                        comboBox_Months.SelectedIndex = 11;
                        prevMonth = "12";
                        textBoxYearEnergy.Text = Convert.ToString(DateTime.Now.Year - 1);
                        break;
                    }
                case 2:
                    {
                        comboBox_Months.SelectedIndex = 0;
                        prevMonth = "1";
                        break;
                    }
                case 3:
                    {
                        comboBox_Months.SelectedIndex = 1;
                        prevMonth = "2";
                        break;
                    }
                case 4:
                    {
                        comboBox_Months.SelectedIndex = 2;
                        prevMonth = "3";
                        break;
                    }
                case 5:
                    {
                        comboBox_Months.SelectedIndex = 3;
                        prevMonth = "4";
                        break;
                    }
                case 6:
                    {
                        comboBox_Months.SelectedIndex = 4;
                        prevMonth = "5";
                        break;
                    }
                case 7:
                    {
                        comboBox_Months.SelectedIndex = 5;
                        prevMonth = "6";
                        break;
                    }
                case 8:
                    {
                        comboBox_Months.SelectedIndex = 6;
                        prevMonth = "7";
                        break;
                    }
                case 9:
                    {
                        comboBox_Months.SelectedIndex = 7;
                        prevMonth = "8";
                        break;
                    }
                case 10:
                    {
                        comboBox_Months.SelectedIndex = 8;
                        prevMonth = "9";
                        break;
                    }
                case 11:
                    {
                        comboBox_Months.SelectedIndex = 9;
                        prevMonth = "10";
                        break;
                    }
                case 12:
                    {
                        comboBox_Months.SelectedIndex = 10;
                        prevMonth = "11";
                        break;
                    }
            }
        }

        private void textBoxYearEnergy_Click(object sender, EventArgs e)
        {
            textBoxYearEnergy.Text = "";
        }
    }
    public class MeausuringPointInfo
    {
        public int network_address;
        public string code;
        public string MeausuringPointName;
        public string MeasuringChannelAname;
        public string MeasuringChannelRname;
    }

    public class CompanyInfo
    {
        public string name;
        public string inn;
        public string contract;
        public string MsgNumber;
        public string date;
    }

    public class MeterInfo
    {
        public string meter_id;
        public string MeausuringPointNameMI;
        public string MeasuringChannelAnameMI;
        public string MeasuringChannelRnameMI;
        public string xml80020code;
        public string transforamtion_ratio;
    }
    public class MeasCheckClass
    {
        public string id;
        public double meas;
    }
}
