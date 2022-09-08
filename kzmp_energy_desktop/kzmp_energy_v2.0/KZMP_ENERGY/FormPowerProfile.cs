using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Security.Cryptography;
using System.Threading;
using GsmComm.GsmCommunication;
using System.Windows.Forms.VisualStyles;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace KZMP_ENERGY
{
    public partial class FormPowerProfile : Form
    {
        //static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DBkzmp_energy;Integrated Security=True";
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public static SqlConnection connection = new SqlConnection(connectionString);
        string sql_cmd = "";
        bool global_status_flag;
        bool crc_check_flag = false;

        int timeOver = 0;
        bool timeOverFlag = false;

        // public static SerialPort port = FormConnectionParameter.port;
        public static SerialPort port;
        public bool flag = true;
        byte[] massiv_last_parameter = new byte[11];
        public static byte[] young_bytes = new byte[] { 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xA0, 0xB0, 0xC0, 0xD0, 0xE0 };
        public static byte older_true;
        public static byte young_true;
        public bool power_profile_flag = true;

        public bool end_flag = true;
        public bool auth_flag = true;

        // T - период интегрирования, А - постоянная счетчика
        public float T = 0;
        public float A = 0;
        //значения постоянных счетчика
        List<int> Alist = new List<int> { 5000, 25000, 1250, 500, 1000, 250 };

        List<int> meter_address = new List<int>();
        List<string> meter_type = new List<string>();
        List<string> meter_id = new List<string>();
        List<meter_date_time_class> meter_date_time_list = new List<meter_date_time_class>();

        int selected_index=0;
        FormMainMenu g;

        List<int> index = new List<int>();

        public static List<string> bagList = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

        int meterListCallIndex = 0;
        bool meterListCallIndexFlag = true;
        bool meterListCallIndexFlag2 = true;

        List<EndInfoAboutSession> EndInfoAboutSessionList = new List<EndInfoAboutSession>();
        public int index_endInfoAboutSession = 0;

        //переменная index_current_meter_address используется при чтении даты последнего измения в datagridview1
        public int index_current_meter_address = 0;
        
        //переменная для установки 3-го байта в запросе 06h для счетчиков меркурий 234.
        public byte bit17for234 = 0x03;

        //флаги интерфейса
        bool GSM_modem_flag = false;
        bool GSM_gateway_flag = false;

        public FormPowerProfile(FormMainMenu f)
        {
            InitializeComponent();
            //System.Globalization.CultureInfo.GetCultureInfo("en-US");
            //progressBar1.Visible = false;
            
            port = FormConnectionParameter.port;

            if (port.CDHolding) { global_status_flag = true; }
            else { global_status_flag = false; }

                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.AllowUserToAddRows = false;

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 10);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(91, 202, 113);
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
                dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
                dataGridView1.RowsDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 10);

                g = f;

                try
                {
                    sql_cmd = "select id_meter, type, address from dbo.meter where SIM='" + FormConnectionParameter.sim_number + "'";
                    if (!(connection.State == ConnectionState.Open))
                    {
                        connection.Open();
                    }
                    SqlCommand cmd2 = new SqlCommand(sql_cmd, connection);
                    SqlDataReader reader2 = cmd2.ExecuteReader();

                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            string meter = Convert.ToString(reader2.GetValue(0)) + " (Сетевой адрес = " + Convert.ToString(reader2.GetValue(1)) + ")";
                            // comboBox1.Items.Add(meter);

                            //dataGridView1.Rows.Add(null,reader2.GetValue(0),reader2.GetValue(1));

                            meter_address.Add(Convert.ToInt32(reader2.GetValue(2)));
                            meter_type.Add(Convert.ToString(reader2.GetValue(1)));
                            meter_id.Add(Convert.ToString(reader2.GetValue(0)));
                        }
                    }
                    reader2.Close();

                    //читаю дату и время последнего измерения 
                    for (int i = 0; i < meter_id.Count; i++)
                    {
                        meter_date_time_class local_class = new meter_date_time_class();
                        string local_sql_cmd = "select max(date) from dbo.power_profile_m where id =" + meter_id[i];
                        if (!(connection.State == ConnectionState.Open))
                        {
                            connection.Open();
                        }
                        SqlCommand cmd_local = new SqlCommand(local_sql_cmd, connection);
                        SqlDataReader reader_local = cmd_local.ExecuteReader();
                        if (reader_local.HasRows)
                        {
                            while (reader_local.Read())
                            {
                                //local_class.date = Convert.ToString(reader_local.GetValue(0));
                                string zxcv = Convert.ToString(reader_local.GetValue(0));
                                if (zxcv != "")
                                {
                                    zxcv = zxcv.Substring(0, 10);
                                }
                                local_class.date = zxcv;

                            }
                            meter_date_time_list.Add(local_class);
                        }
                        reader_local.Close();
                    }
                    for (int i = 0; i < meter_id.Count; i++)
                    {
                        meter_date_time_class local_class = new meter_date_time_class();
                        string local_sql_cmd = "select max(time) from dbo.power_profile_m where id =" + meter_id[i] + " and date='" + meter_date_time_list[i].date + "';";
                        if (!(connection.State == ConnectionState.Open))
                        {
                            connection.Open();
                        }
                        SqlCommand cmd_local = new SqlCommand(local_sql_cmd, connection);
                        SqlDataReader reader_local = cmd_local.ExecuteReader();
                        if (reader_local.HasRows)
                        {
                            while (reader_local.Read())
                            {
                                meter_date_time_list[i].time = Convert.ToString(reader_local.GetValue(0));
                                //meter_date_time_list[i].time = meter_date_time_list[i].time.Substring(0, 7);
                                //MessageBox.Show(meter_date_time_list[i].time);
                            }
                        }
                        reader_local.Close();
                    }

                    //выгрузка информации о счётчиках в dataGridView
                    for (int i = 0; i < meter_id.Count; i++)
                    {
                        string date_time = meter_date_time_list[i].date + " " + meter_date_time_list[i].time;
                        object y = date_time;
                        dataGridView1.Rows.Add(true, meter_type[i], meter_address[i], y);
                    }

                    /*for (int i = 0; i < 10; i++)
                    {
                        dataGridView1.Rows.Add(null,null,null);
                    }*/

                    GSM_modem_flag = FormConnectionParameter.GSM_modem_flag;
                    GSM_gateway_flag = FormConnectionParameter.GSM_gateway_flag;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            if (!global_status_flag)
            {
                //StatusTextBox.Text = "NO CARRIER";

                MessageBox.Show(
                "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                "kzmp_energy notification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

                Application.Restart();
            }
            else 
            {
                //StatusTextBox.Text = "CONNECTED";
            }
        }
        private async void iconButton1_MouseDown_func() 
        {
            try
            {
                int address = 0;
                //selected_index = comboBox1.SelectedIndex;

                //блокировка кнопок формы
                g.btnHome.Enabled = false;
                g.iconButton1.Enabled = false;
                g.iconButton2.Enabled = false;
                g.iconButton3.Enabled = false;

                g.iconButton5.Enabled = false;
                g.iconButton6.Enabled = false;
                g.iconButton4.Enabled = false;

                iconButton1.Enabled = false;
                dataGridView1.Enabled = false;
                datePickerStart.Enabled = false;
                datePickerEnd.Enabled = false;
                timePickerStart.Enabled = false;
                timePickerEnd.Enabled = false;
                comboBox_Months.Enabled = false;



                //инициализация переменной, в которой хранится число счётчиков для которых нужно снять данные
                //meterCallIndex служит и как индекс следующего счётчика и как число характеризующее количество вызовов iconButton1_MouseDown
                if (meterListCallIndexFlag)
                {
                    meterListCallIndex = index.Count;
                    meterListCallIndexFlag = false;

                    //проверка выбран ли хотя бы один счётчик для снятия данных
                    if (meterListCallIndex == 0)
                    {
                        meterListCallIndexFlag2 = false;
                        richTextBox_conStatus2.AppendText("\nERROR: Не выбран ни один счётчик!");
                        richTextBox_conStatus2.ScrollToCaret();
                    }
                    else 
                    {
                        meterListCallIndexFlag2 = true;
                        richTextBox_conStatus2.AppendText("\nСледующие счетчики добавлены в лист ожидания:");
                        richTextBox_conStatus2.ScrollToCaret();

                        for (int i = 0; i < meterListCallIndex; i++)
                        {
                            richTextBox_conStatus2.AppendText("\n\tСетевой адрес: " + meter_address[index[i]]);
                            richTextBox_conStatus2.ScrollToCaret();
                        }
                    }
                    meterListCallIndex = meterListCallIndex - 1;
                }

                if (meterListCallIndexFlag2)
                {
                    //EndInfoAboutSessionList лист отчета в котором указывается как был прочитан счетчик - успех или нет
                    EndInfoAboutSession local = new EndInfoAboutSession();
                    local.meter_address = Convert.ToString(meter_address[index[meterListCallIndex]]);
                    EndInfoAboutSessionList.Add(local);

                    //получение адреса счётчика 
                    address = meter_address[index[meterListCallIndex]];

                    //переменная index_current_meter_address используется при чтении даты последнего измения в datagridview1
                    index_current_meter_address = index[meterListCallIndex];

                    //await Task.Run(()=>MeterConnectionAsync());

                    //await Task.Run(()=>write_disconnect());


                    //MeterConnection(address);
                    if (GSM_modem_flag)
                    {
                        await Task.Run(() => MeterConnection(address));
                    }
                    else if (GSM_gateway_flag)
                    {
                        await Task.Run(()=>MeterConnectionGateway(address));
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);

                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);
            }
        }
        private void iconButton1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

                if (port.CDHolding) 
                {
                    StatusTextBox.ForeColor = Color.LimeGreen;
                   StatusTextBox.Text = "CONNECTED";
                    global_status_flag = true;
                }
                else 
                {
                    StatusTextBox.Text = "NO CARRIER";
                    global_status_flag = false;
                }
                //получение индексов отмеченных галочкой в dataGridView1 счётчиков
                if (global_status_flag)
                {
                    int k = dataGridView1.Rows.Count;
                    for (int i = 0; i < k; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value != null)
                        {
                            index.Add(i);
                        }
                    }
                    index_endInfoAboutSession = 0;

                    iconButton1_MouseDown_func();
                }
                else 
                {
                    MessageBox.Show(
                    "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                    "kzmp_energy notification",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                    Application.Restart();
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        //функция определения количества 30 минут между датами начала и датой последней записи
        private int Count30(string last_data)
        {

            string StartData = datePickerStart.Value.ToShortDateString();
            string StartTime = timePickerStart.Value.ToShortTimeString();
            //string EndData = datePickerEnd.Value.ToShortDateString();
            // string EndTime = timePickerEnd.Value.ToShortTimeString();

            string meter_last_date = dataGridView1.Rows[index_current_meter_address].Cells[3].Value.ToString();
            if (meter_last_date == "" || meter_last_date == " ") { meter_last_date = "01.01.0001 00:00"; }

            var start1 = DateTime.Parse(StartData + " " + StartTime);
            var start2 = DateTime.Parse(meter_last_date);

            

            //var Start = DateTime.Parse(StartData + " " + StartTime);
            int count =0;
            try
            {
                var End = DateTime.Parse(last_data);
                //var countVar = (Start - End).Duration();

                // count = Convert.ToInt32(countVar.Days) * 24 * 2 + Convert.ToInt32(countVar.Hours) * 2 + Convert.ToInt32(countVar.Minutes) / 30;

                int compare = DateTime.Compare(start2, start1);

                if (compare > 0)
                {
                    var countVar = (start2 - End).Duration();
                    count = Convert.ToInt32(countVar.Days) * 24 * 2 + Convert.ToInt32(countVar.Hours) * 2 + Convert.ToInt32(countVar.Minutes) / 30;
                }
                else 
                {
                    var countVar = (start1 - End).Duration();
                    count = Convert.ToInt32(countVar.Days) * 24 * 2 + Convert.ToInt32(countVar.Hours) * 2 + Convert.ToInt32(countVar.Minutes) / 30;
                }

                //MessageBox.Show(Convert.ToString(countVar));
                
            }
            catch 
            {
                count = 2;
            }

            return count;
        }
        //функция определения количества 30 минут между датами конца и текущими запрашиваемым значением
        private int Count30_end()
        {

            string StartData = datePickerStart.Value.ToShortDateString();
            string StartTime = timePickerStart.Value.ToShortTimeString();

            string EndData = datePickerEnd.Value.ToShortDateString();
            string EndTime = timePickerEnd.Value.ToShortTimeString();

            var Start = DateTime.Parse(StartData + " " + StartTime);
            //var End = DateTime.Parse(last_data);
            var End = DateTime.Parse(EndData + " " + EndTime);

            var countVar = (Start - End).Duration();

            int count = Convert.ToInt32(countVar.Days) * 24 * 2 + Convert.ToInt32(countVar.Hours) * 2 + Convert.ToInt32(countVar.Minutes) / 30;
            //MessageBox.Show(Convert.ToString(countVar));
            count = Convert.ToInt32(Math.Ceiling(Convert.ToSingle(count) / Convert.ToSingle(2)));
            return count;
        }
        //функция вычисления CRC16
        public static byte[] CalculateCrc16Modbus(byte[] bytes)
        {
            CRC16.CrcStdParams.StandartParameters.TryGetValue(CRC16.CrcAlgorithms.Crc16Modbus, out CRC16.Parameters crc_p);
            CRC16.Crc crc = new CRC16.Crc(crc_p);
            crc.Initialize();
            var crc_bytes = crc.ComputeHash(bytes);
            return crc_bytes;
        }
        //функция вычисления CRC24
        public static byte[] CalculateCrc24(byte[] bytes) 
        {
            CRC16.CrcStdParams.StandartParameters.TryGetValue(CRC16.CrcAlgorithms.Crc24, out CRC16.Parameters crc_p);
            CRC16.Crc crc = new CRC16.Crc(crc_p);
            crc.Initialize();
            var crc_bytes = crc.ComputeHash(bytes);
            return crc_bytes;
        }

        //функция установления соединения с счётчиком 
        public async void MeterConnection(int address)
        {
            //ТЕСТ СВЯЗИ СО СЧЁТЧИКОМ
            //формирование запроса
            // msg345 = new List<byte>();
            try
            {
                byte hex = Convert.ToByte(address);

                byte[] meter_connection_test = new byte[4];
                byte[] meter_connection_test_crc = new byte[] { hex, 0x00 };
                byte[] crc = CalculateCrc16Modbus(meter_connection_test_crc);

                meter_connection_test[0] = hex;
                meter_connection_test[1] = 0x00;
                meter_connection_test[2] = crc[0];
                meter_connection_test[3] = crc[1];

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nТЕСТИРОВАНИЕ СОЕДИНЕНИЯ СО СЧЁТЧИКОМ (адрес: " + Convert.ToString(address) + ")\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                timeOver = 0;
                timeOverFlag = true;
                if (timeOverFlag)
                {
                    await Task.Run(() => write(meter_connection_test, meter_connection_test.Length));
                    await Task.Run(() => read4());
                }

                //АУТЕНТИФИКАЦИЯ И АВТОРИЗАЦИЯ НА СЧЁТЧИКЕ
                byte[] ath_meter_crc = new byte[] { hex, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
                byte[] crc2 = CalculateCrc16Modbus(ath_meter_crc);
                byte[] ath_meter = new byte[11] { hex, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, crc2[0], crc2[1] };

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nАУТЕНТИФИКАЦИЯ И АВТОРИЗАЦИЯ НА СЧЁТЧИКЕ(адрес: " + Convert.ToString(address) + ")\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                await Task.Run(() => write(ath_meter, ath_meter.Length));
                await Task.Run(() => read_for_auth(ath_meter, ath_meter.Length, hex, address));
            }
            catch (Exception ex)
            {

                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);
            }
        }
        //кнопка закрытия канала связи с счётчиком
        private void iconButton4_MouseDown(object sender, MouseEventArgs e)
        {
            byte[] cl_crc = new byte[] { 1, 0x02 };
            byte[] crc = CalculateCrc16Modbus(cl_crc);
            byte[] cl = new byte[] { Convert.ToByte(1), 0x02, crc[0], crc[1] };

            port.DiscardInBuffer();
            port.DiscardOutBuffer();
            port.BaseStream.Flush();

            // port.DataReceived -= DataReceivedHandler4;
            //port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler3);

            port.Write(cl, 0, 4);
            Thread.Sleep(2000);
        }
        //функция обработки массива байтов последней записи
        private async void LastParameter(int address)
        {
            byte address_b = Convert.ToByte(address);
            //вычисление количества 30 минут между датой начала периода и датой последней записи на счётчике
            byte older = 0;
            byte young = 0;

            await Task.Run(() => finding(address, address_b, ref older, ref young));
            await Task.Run(() => power_profile(older, young, address_b, address));
        }

        private void finding(int address, byte address_b, ref byte older, ref byte young)
        {
            older = massiv_last_parameter[1];
            young = massiv_last_parameter[2];

            if (meter_type[selected_index].Contains("234"))
            {
                //string k = older.ToString("X") + young.ToString("X") + "0";
                string BagOlder = older.ToString("X");
                string BagYoung = young.ToString("X");
                string k = "";

                foreach (string a in bagList)
                {
                    if (a == BagOlder) { BagOlder = BagOlder.Insert(0,"0"); }
                    if (a == BagYoung) { BagYoung = BagYoung.Insert(0,"0"); }
                }

                k = BagOlder + BagYoung + "0";

                string localOlder = Convert.ToString(k[1]) + Convert.ToString(k[2]);
                string localYoung = Convert.ToString(k[3]) + Convert.ToString(k[4]);

                int z = Convert.ToInt32(localOlder, 16);
                int s = Convert.ToInt32(localYoung, 16);

                older = Convert.ToByte(z);
                young = Convert.ToByte(s);

                //MessageBox.Show(Convert.ToString(older)+" " + Convert.ToString(young));
            }

            byte hour = massiv_last_parameter[4];
            int hour_b = Convert.ToInt32(hour);

            byte minute = massiv_last_parameter[5];
            int minute_b = Convert.ToInt32(minute);

            byte day = massiv_last_parameter[6];
            int day_b = Convert.ToInt32(day);

            byte month = massiv_last_parameter[7];
            int month_b = Convert.ToInt32(month);

            byte year = massiv_last_parameter[8];
            int year_b = Convert.ToInt32(year);

            string last_data = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X") + " " + hour_b.ToString("X") + ":" + minute_b.ToString("X");
            int count = Count30(last_data); //кол-во 30 минут между датой начала периода и датой последней записи на счётчике

           // count = count - Convert.ToInt32(Convert.ToSingle(count) * 2 / 11);

            for (int i = count; i >= 0; i--)
            {
                if (young == 0x00)
                {
                    if (older != 0x00)
                    {
                        older = Convert.ToByte(older - 0x01);
                    }
                    else 
                    { 
                        older = 0xff;
                        //смена 17-го бита для Меркурия 234 происходит при переходе старшего байта с 0x00 в 0xff и наоборот
                        if (bit17for234 == 0x03)
                        {
                            bit17for234 = 0x83; 
                        }
                        else 
                        {
                            bit17for234 = 0x03;
                        }
                        
                    }
                    young = 0xf0;
                }
                else
                {
                    young = Convert.ToByte(young - 0x10);
                }
            }
        }

        //ФУНКЦИИ ПОЛУЧЕНИЯ ПРОФИЛЯ МОЩНОСТИ
        private async void power_profile(byte older, byte young, byte address_b, int address)
        {
            power_profile_flag = true;

            try
            { if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
            }
            catch (SqlException ex)
            {
                //MessageBox.Show(ex.Message);

                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);
            }
            //полуение id счетчика 
            sql_cmd = "select id_meter from dbo.meter where address =" + Convert.ToString(address) + " and SIM='" + FormConnectionParameter.sim_number + "'";
            SqlCommand cmd = new SqlCommand(sql_cmd, connection);
            SqlDataReader reader2 = cmd.ExecuteReader();
            int id_meter = 0;
            while (reader2.Read())
            {
                id_meter = Convert.ToInt32(reader2.GetInt32(0));
            }
            reader2.Close();
            //отправка запросов на получение данных профиля мощности и запись данных в бд

            await Task.Run(() => power_profile_getting(address_b, older, young, id_meter, address));

            //await Task.Run(() => write_closing(address_b));
            //await Task.Run(() => read4_closing());
        }
        private async void power_profile_getting(byte address_b, byte older, byte young, int id_meter, int address)
        {
            
                byte[] d = new byte[] { 0x10, 0x30, 0x50, 0x70, 0x90, 0xB0, 0xD0,0xf0 };
                foreach (byte a in d)
                {
                    if (young == a) { young = Convert.ToByte(young - 0x10); break; }
                }

                //int count_local = Count30_end();

                string EndData = datePickerEnd.Value.ToShortDateString() + " " + timePickerEnd.Value.ToShortTimeString();

                int count_local = Count30(EndData);
                count_local = Convert.ToInt32(Math.Ceiling(Convert.ToSingle(count_local) / Convert.ToSingle(2)));
                count_local = count_local + 2;


                int step = count_local * 10 / 100;
                int i = 0;
                int proc = 10;
                int fg = step;

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nЗАГРУЗКА ПРОФИЛЯ МОЩНОСТИ...(адрес: " + Convert.ToString(address) + ")\n-------------------------------------------------------------------------------------------------\n");
                richTextBox_conStatus2.ScrollToCaret();

            progressBar1.Value = 0;
            progressBar1.Maximum = count_local;
            progressBar1.Minimum = 0;
            progressBar1.Step = 1;

            byte code_write_profile = 0x03;
                if (meter_type[selected_index].Contains("234"))
                {
                    code_write_profile = bit17for234;
                }

            if (GSM_modem_flag)
            {
                crc_check_flag = false;
                timeOver = 0;
                timeOverFlag = true;
                while (count_local >= 0)
                {
                    try
                    {
                        await Task.Run(() => write_profile(code_write_profile, address_b, older, young, ref count_local));
                        await Task.Run(() => read30_power_profile(id_meter, address, ref older, ref young, ref code_write_profile, ref count_local));
                        await Task.Run(() => loading_status(ref i, ref fg, ref proc, ref step));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "kzmp_energy notification", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    }
                }
                try
                {
                    if (cb_monthEnergy.Checked)
                    {
                        KZMP_ENERGY.monthEnergy.energy energyReadClass = new monthEnergy.energy(address_b, id_meter, comboBox_Months.SelectedItem.ToString());
                        await Task.Run(() => energyReadClass.getMonthEnergy(false));
                    }
                }
                catch(Exception ex)
                {
                    richTextBox_conStatus2.AppendText(ex.Message);
                    richTextBox_conStatus2.ScrollToCaret();
                }
            }
            else if (GSM_gateway_flag)
            {
                crc_check_flag = false;
                timeOver = 0;
                timeOverFlag = true;
                while (count_local >= 0)
                {
                    try
                    {
                        await Task.Run(() => write_profile_gateway(code_write_profile, address_b, older, young, ref count_local));
                        await Task.Run(() => read30_power_profile_gateway(id_meter, address, ref older, ref young, ref code_write_profile, ref count_local));
                        await Task.Run(() => loading_status(ref i, ref fg, ref proc, ref step));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "kzmp_energy notification", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    }
                }
                try
                {
                    if (cb_monthEnergy.Checked)
                    {
                        KZMP_ENERGY.monthEnergy.energy energyReadClass = new monthEnergy.energy(address_b, id_meter, comboBox_Months.SelectedItem.ToString());
                        await Task.Run(()=> energyReadClass.getMonthEnergy(true));      
                    }
                }
                catch (Exception ex)
                {
                    richTextBox_conStatus2.AppendText(ex.Message);
                    richTextBox_conStatus2.ScrollToCaret();
                    MessageBox.Show(ex.Message);
                }
            }
            try
            {
                await Task.Run(() => write_to_box());
                if (GSM_modem_flag)
                {
                    await Task.Run(() => write_closing(address_b));
                    await Task.Run(() => read4_closing());
                }
                else if(GSM_gateway_flag)
                {
                    await Task.Run(() => write_closing_gateway(address_b));
                    await Task.Run(() => read4_closing());
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);
            }
            
        }
        private void loading_status(ref int i, ref int fg, ref int proc, ref int step)
        {
            if (crc_check_flag&&!timeOverFlag)
            {
                progressBar1.PerformStep();
            }

            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
            }
            catch (SqlException ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);
            }
        }
        //ФУНКЦИИ АСИНХРОННОЙ ОБРАБОТКИ ЗАПРОСОВ И ОТВЕТОВ
        private void write_find(byte address_b, byte older, byte young)
        {
            byte code = 0x03;
            if (meter_type[selected_index].Contains("234"))
            {
                code = 0x83;
            }
            byte[] power_crc = new byte[] { address_b, 0x06, code, older, young, 0x1e };
            byte[] crc8 = CalculateCrc16Modbus(power_crc);
            byte[] power = new byte[] { address_b, 0x06, code, older, young, 0x1e, crc8[0], crc8[1] };

            port.DiscardInBuffer();
            port.Write(power, 0, power.Length);

        }
        private void write_profile(byte code,byte address_b, byte older, byte young, ref int count_local)
        {
            /*
            byte code = 0x03;
            if (meter_type[selected_index].Contains("234"))
            {
                code = 0x83;
            }*/

            byte[] power_crc = new byte[] { address_b, 0x06, code, older, young, 0x1e };
            byte[] crc8 = CalculateCrc16Modbus(power_crc);
            byte[] power = new byte[] { address_b, 0x06, code, older, young, 0x1e, crc8[0], crc8[1] };

            if (!port.CDHolding)
            {
               // StatusTextBox.Text = "NO CARRIER";
                global_status_flag = false;

                MessageBox.Show(
                "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                "kzmp_energy notification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

                Application.Restart();
            }

            port.DiscardInBuffer();
            port.Write(power, 0, power.Length);
 
            //count_local = count_local - 1;

        }
        private void write_profile_gateway(byte code, byte address_b, byte older, byte young, ref int count_local)
        {
            /*
            byte code = 0x03;
            if (meter_type[selected_index].Contains("234"))
            {
                code = 0x83;
            }*/

            byte[] power_crc = new byte[] { address_b, 0x06, code, older, young, 0x1e };
            byte[] crc8 = CalculateCrc16Modbus(power_crc);
            byte[] power = new byte[] { address_b, 0x06, code, older, young, 0x1e, crc8[0], crc8[1] };

            byte[] power_gateway = new byte[17];

            //num
            power_gateway[3] = 0x01;
            power_gateway[4] = 0x00;
            //len
            power_gateway[5] = 0x08;
            power_gateway[6] = 0x00;
            //port
            power_gateway[7] = 0x01;
            //payload
            power_gateway[8] = power[0];
            power_gateway[9] = power[1];
            power_gateway[10] = power[2];
            power_gateway[11] = power[3];
            power_gateway[12] = power[4];
            power_gateway[13] = power[5];
            power_gateway[14] = power[6];
            power_gateway[15] = power[7];
            //checksum
            power_gateway[16] = Convert.ToByte((power_gateway[8]+power_gateway[9] + power_gateway[10] + power_gateway[11] + power_gateway[12]
                                        + power_gateway[13] + power_gateway[14] + power_gateway[15]+0xff)&0xff);
            //crc24
            byte[] local_mas = new byte[] {power_gateway[3],power_gateway[4], power_gateway[5], power_gateway[6], power_gateway[7]};
            byte[] crc24 = CalculateCrc24(local_mas);
            power_gateway[0] = crc24[0];
            power_gateway[1] = crc24[1];
            power_gateway[2] = crc24[2];

            if (!port.CDHolding)
            {
               StatusTextBox.Text = "NO CARRIER";
                global_status_flag = false;

                MessageBox.Show(
                "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                "kzmp_energy notification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

                Application.Restart();
            }

            port.DiscardInBuffer();
            port.Write(power_gateway, 0, power_gateway.Length);

            //count_local = count_local - 1;

        }
        private void write(byte[] f, int x)
        {
            try
            {
                if (!port.CDHolding) 
                { 
                    StatusTextBox.Text = "NO CARRIER";
                    global_status_flag = false;

                    MessageBox.Show(
                    "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                    "kzmp_energy notification",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                    Application.Restart();
                }

                port.DiscardInBuffer();
                port.Write(f, 0, x);
            }
            catch (Exception ex)
            {
            }
        }
        private void write_closing(byte address_b)
        {
            byte[] meter_conn_closing__for_crc = new byte[] { address_b, 0x02 };
            byte[] meter_conn_closing_crc = CalculateCrc16Modbus(meter_conn_closing__for_crc);
            byte[] meter_conn_closing = new byte[4] { address_b, 0x02, meter_conn_closing_crc[0], meter_conn_closing_crc[1] };

            if (!port.CDHolding)
            {
                StatusTextBox.Text = "NO CARRIER";
                global_status_flag = false;

                MessageBox.Show(
                "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                "kzmp_energy notification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

                Application.Restart();
            }

            port.DiscardInBuffer();
            port.Write(meter_conn_closing, 0, meter_conn_closing.Length);
            //MessageBox.Show("Профиль мощности для счетчика с сетевым адресом "+Convert.ToInt32(address_b)+" считан. Соединение с счетчиком закрыто.");


                richTextBox_conStatus2.AppendText("\nСоединение с счетчиком (сетевой адрес " + Convert.ToInt32(address_b) + ") закрыто.");
                richTextBox_conStatus2.ScrollToCaret();

            
        }
        private void write_closing_gateway(byte address_b)
        {
            byte[] meter_conn_closing__for_crc = new byte[] { address_b, 0x02 };
            byte[] meter_conn_closing_crc = CalculateCrc16Modbus(meter_conn_closing__for_crc);
            byte[] meter_conn_closing = new byte[4] { address_b, 0x02, meter_conn_closing_crc[0], meter_conn_closing_crc[1] };

            byte[] meter_conn_closing_gateway = new byte[13];

            //num
            meter_conn_closing_gateway[3] = 0x01;
            meter_conn_closing_gateway[4] = 0x00;
            //len
            meter_conn_closing_gateway[5] = 0x04;
            meter_conn_closing_gateway[6] = 0x00;
            //port
            meter_conn_closing_gateway[7] = 0x01;
            //payload
            meter_conn_closing_gateway[8] = meter_conn_closing[0];
            meter_conn_closing_gateway[9] = meter_conn_closing[1];
            meter_conn_closing_gateway[10] = meter_conn_closing[2];
            meter_conn_closing_gateway[11] = meter_conn_closing[3];
            //checksum
            meter_conn_closing_gateway[12] = Convert.ToByte((meter_conn_closing_gateway[8]+meter_conn_closing_gateway[9]+meter_conn_closing_gateway[10]
                                                +meter_conn_closing_gateway[11]+0xff)&0xff);
            //crc24
            byte[] local_mas = new byte[] {meter_conn_closing_gateway[3],meter_conn_closing_gateway[4], meter_conn_closing_gateway[5], meter_conn_closing_gateway[6]
                                             ,meter_conn_closing_gateway[7]};
            byte[] crc24 = CalculateCrc24(local_mas);
            meter_conn_closing_gateway[0] = crc24[0];
            meter_conn_closing_gateway[1] = crc24[1];
            meter_conn_closing_gateway[2] = crc24[2];

            port.DiscardInBuffer();
            port.Write(meter_conn_closing_gateway, 0, meter_conn_closing_gateway.Length);
            //MessageBox.Show("Профиль мощности для счетчика с сетевым адресом "+Convert.ToInt32(address_b)+" считан. Соединение с счетчиком закрыто.");
            richTextBox_conStatus2.AppendText("\nСоединение с счетчиком (сетевой адрес " + Convert.ToInt32(address_b) + ") закрыто.");
            richTextBox_conStatus2.ScrollToCaret();


        }
        private async void read_for_auth(byte[] z, int x, byte hex, int address)
        {
            auth_flag = true;
            Thread.Sleep(1);
            List<byte> msg = new List<byte>();
            int i = 0;
            int count = 0;
            int lng = 0;
            TimeoutTextBox2.Text = "#Auth";

            int timeToSleep = 10;

            while (lng < 4)
            {
                if (!port.CDHolding)
                {
                   StatusTextBox.Text = "NO CARRIER";
                    global_status_flag = false;

                    MessageBox.Show(
                    "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                    "kzmp_energy notification",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                    Application.Restart();
                }

                i++;
                count += port.BytesToRead;

                if (count > 0)
                {
                    byte[] ByteArray = new byte[count];
                    //ByteArray = Encoding.Default.GetBytes(port.ReadExisting());
                    port.Read(ByteArray, 0, count);
                    foreach (byte f in ByteArray)
                    {
                        msg.Add(f);
                    }
                    lng = msg.Count;
                    count = 0;
                }
                Thread.Sleep(timeToSleep);
                if (i == 500) {
                    richTextBox_conStatus2.AppendText("\nWARNING: Нет ответа. Попытка 2.");
                    richTextBox_conStatus2.ScrollToCaret(); write(z, x);
                    Thread.Sleep(500); }
                //if (i==60) { write(z, x);Thread.Sleep(500); }
                if (i == 1000) {
                    richTextBox_conStatus2.AppendText("\nWARNING: Нет ответа. Попытка 3.");
                    richTextBox_conStatus2.ScrollToCaret(); write(z, x);
                    Thread.Sleep(500); }
                // if (i == 120) { write(z, x); Thread.Sleep(500); }
                if (i == 1500) {
                    richTextBox_conStatus2.AppendText("\nWARNING: Нет ответа. Попытка 4.");
                    richTextBox_conStatus2.ScrollToCaret(); write(z, x);
                    Thread.Sleep(500); }
                // if (i == 180) { write(z, x); Thread.Sleep(500); }
                //if (i == 210) { write(z, x); Thread.Sleep(1000); }
                if (i == 2000)
                {
                    //MessageBox.Show("Ошибка авторизации на счетчике! (нет ответа)");
                    richTextBox_conStatus2.AppendText("\nERROR: нет ответа от счётчика. ");
                    richTextBox_conStatus2.ScrollToCaret();

                    auth_flag = false;
                    break;
                }
                if (auth_flag == false) { break; }
            }
            if (auth_flag)
            {
                //ЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ ВАРИАНТА ИСПОЛНЕНИЯ
                byte[] var_isp_crc = new byte[] { hex, 0x08, 0x12 };
                byte[] var_isp_crcR = CalculateCrc16Modbus(var_isp_crc);
                byte[] var_isp = new byte[] { hex, 8, 18, var_isp_crcR[0], var_isp_crcR[1] };

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ ВАРИАНТА ИСПОЛНЕНИЯ\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                crc_check_flag = false;
                timeOver = 0;
                timeOverFlag = true;
                if (!crc_check_flag||timeOverFlag)
                {
                    await Task.Run(() => write(var_isp, var_isp.Length));
                    await Task.Run(() => read_var_isp());
                }

                //ЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ О ПОСЛЕДНЕЙ ЗАПИСИ СЧЁТЧИКА
                byte[] last_parameter_crc = new byte[] { hex, 0x08, 0x13 };
                byte[] crc3 = CalculateCrc16Modbus(last_parameter_crc);
                byte[] last_parameter = new byte[5] { hex, 8, 19, crc3[0], crc3[1] };

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ О ПОСЛЕДНЕЙ ЗАПИСИ СЧЁТЧИКА\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                crc_check_flag = false;
                timeOver = 0;
                timeOverFlag = true;
                if (!crc_check_flag || timeOverFlag)
                {
                    await Task.Run(() => write(last_parameter, last_parameter.Length));
                    await Task.Run(() => read12());
                }

                //Thread.Sleep(1000);
                await Task.Run(() => LastParameter(address));
            }
            else 
            {
                //переход к следующему счётчику
                if (meterListCallIndex != 0)
                {
                    EndInfoAboutSessionList[index_endInfoAboutSession].state = "Нет ответа на запрос аутентификации.";

                    index_endInfoAboutSession = index_endInfoAboutSession + 1;

                    richTextBox_conStatus2.Clear();

                    meterListCallIndex = meterListCallIndex - 1;
                    richTextBox_conStatus2.AppendText("");
                    Thread.Sleep(2000);
                    //iconButton1_MouseDown_func();
                    await Task.Run(() => iconButton1_MouseDown_func());
                }
                else
                {
                    EndInfoAboutSessionList[index_endInfoAboutSession].state = "Нет ответа на запрос аутентификации.";

                    //закрытие соединения с модемом
                    await Task.Run(() => write_disconnect());
                    await Task.Run(() => read4());
                    await Task.Run(() => EnterEndInfoAboutSession());

                    //await Task.Run(()=>restartApp());
                }
            }
        }
        private async void read_for_auth_gateway(byte[] z, int x, byte hex, int address)
        {
            auth_flag = true;
            Thread.Sleep(1);
            List<byte> msg = new List<byte>();
            int i = 0;
            int count = 0;
            int lng = 0;
           // TimeoutTextBox2.Text = "#Auth";
            int timeToSleep = 10;
            while (lng < 11)
            {
                if (!port.CDHolding)
                {
                   // StatusTextBox.Text = "NO CARRIER";
                    global_status_flag = false;

                    MessageBox.Show(
                    "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                    "kzmp_energy notification",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                    Application.Restart();
                }

                i++;
                count += port.BytesToRead;

                if (count > 0)
                {
                    byte[] ByteArray = new byte[count];
                    //ByteArray = Encoding.Default.GetBytes(port.ReadExisting());
                    port.Read(ByteArray, 0, count);
                    foreach (byte f in ByteArray)
                    {
                        msg.Add(f);
                    }
                    lng = msg.Count;
                    count = 0;
                }
                Thread.Sleep(timeToSleep);
                if (i == 500) {
                    richTextBox_conStatus2.AppendText("\nWARNING: Нет ответа. Попытка 2.");
                    richTextBox_conStatus2.ScrollToCaret(); write(z, x);
                    Thread.Sleep(500); }
                //if (i==60) { write(z, x);Thread.Sleep(500); }
                if (i == 1000) {
                    richTextBox_conStatus2.AppendText("\nWARNING: Нет ответа. Попытка 3.");
                    richTextBox_conStatus2.ScrollToCaret();
                    write(z, x); 
                    Thread.Sleep(500); }
                // if (i == 120) { write(z, x); Thread.Sleep(500); }
                if (i == 1500) {
                    richTextBox_conStatus2.AppendText("\nWARNING: Нет ответа. Попытка 4.");
                    richTextBox_conStatus2.ScrollToCaret();
                    write(z, x); Thread.Sleep(500); }
                // if (i == 180) { write(z, x); Thread.Sleep(500); }
                //if (i == 210) { write(z, x); Thread.Sleep(1000); }
                if (i == 2000)
                {
                    //MessageBox.Show("Ошибка авторизации на счетчике! (нет ответа)");
                    richTextBox_conStatus2.AppendText("\nERROR: нет ответа от счётчика. ");
                    richTextBox_conStatus2.ScrollToCaret();

                    auth_flag = false;
                    break;
                    /*
                    await Task.Run(() => write_closing(hex));

                    await Task.Run(()=>toNull());
                    await Task.Run(() => read4_closing());

                    await Task.Run(() => timerToRestart());
                    await Task.Run(()=>restartApp());*/
                }
                if (auth_flag == false) { break; }
            }
            if (auth_flag)
            {
                //ЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ ВАРИАНТА ИСПОЛНЕНИЯ
                byte[] var_isp_crc = new byte[] { hex, 0x08, 0x12 };
                byte[] var_isp_crcR = CalculateCrc16Modbus(var_isp_crc);
                byte[] var_isp = new byte[] { hex, 8, 18, var_isp_crcR[0], var_isp_crcR[1] };

                byte[] var_isp_gateway = new byte[14];

                //payload
                var_isp_gateway[8] = var_isp[0];
                var_isp_gateway[9] = var_isp[1];
                var_isp_gateway[10] = var_isp[2];
                var_isp_gateway[11] = var_isp[3];
                var_isp_gateway[12] = var_isp[4];
                //checksum
                var_isp_gateway[13] = Convert.ToByte((var_isp_gateway[8]+var_isp_gateway[9] + var_isp_gateway[10] + var_isp_gateway[11] + var_isp_gateway[12]+0xff)&0xff);
                //num
                var_isp_gateway[3] = 0x01;
                var_isp_gateway[4] = 0x00;
                //len
                var_isp_gateway[5] = 0x05;
                var_isp_gateway[6] = 0x00;
                //port
                var_isp_gateway[7] = 0x01;
                //crc24
                byte[] local_mas = new byte[] {var_isp_gateway[3], var_isp_gateway[4], var_isp_gateway[5], var_isp_gateway[6], var_isp_gateway[7] };
                byte[] crc24 = CalculateCrc24(local_mas);
                var_isp_gateway[0] = crc24[0];
                var_isp_gateway[1] = crc24[1];
                var_isp_gateway[2] = crc24[2];

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ ВАРИАНТА ИСПОЛНЕНИЯ\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                crc_check_flag = false;
                timeOver = 0;
                timeOverFlag = true;
                if (!crc_check_flag||timeOverFlag)
                {
                    await Task.Run(() => write(var_isp_gateway, var_isp_gateway.Length));
                    await Task.Run(() => read_var_isp_gateway());
                }

                //ЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ О ПОСЛЕДНЕЙ ЗАПИСИ СЧЁТЧИКА
                byte[] last_parameter_crc = new byte[] { hex, 0x08, 0x13 };
                byte[] crc3 = CalculateCrc16Modbus(last_parameter_crc);
                byte[] last_parameter = new byte[5] { hex, 8, 19, crc3[0], crc3[1] };

                byte[] last_parameter_gateway = new byte[14];

                //num
                last_parameter_gateway[3] = 0x01;
                last_parameter_gateway[4] = 0x00;
                //len
                last_parameter_gateway[5] = 0x05;
                last_parameter_gateway[6] = 0x00;
                //port
                last_parameter_gateway[7] = 0x01;
                //payload
                last_parameter_gateway[8] = last_parameter[0];
                last_parameter_gateway[9] = last_parameter[1];
                last_parameter_gateway[10] = last_parameter[2];
                last_parameter_gateway[11] = last_parameter[3];
                last_parameter_gateway[12] = last_parameter[4];
                //checksum
                last_parameter_gateway[13] = Convert.ToByte((last_parameter_gateway[8]+ last_parameter_gateway[9] + 
                                                last_parameter_gateway[10] + last_parameter_gateway[11] + last_parameter_gateway[12]+0xff)&0xff);
                //crc24
                local_mas[0] = last_parameter_gateway[3];
                local_mas[1] = last_parameter_gateway[4];
                local_mas[2] = last_parameter_gateway[5];
                local_mas[3] = last_parameter_gateway[6];
                local_mas[4] = last_parameter_gateway[7];
                crc24 = CalculateCrc24(local_mas);
                last_parameter_gateway[0] = crc24[0];
                last_parameter_gateway[1] = crc24[1];
                last_parameter_gateway[2] = crc24[2];

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ О ПОСЛЕДНЕЙ ЗАПИСИ СЧЁТЧИКА\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                crc_check_flag = false;
                timeOver = 0;
                timeOverFlag = true;
                if (!crc_check_flag||timeOverFlag)
                {
                    await Task.Run(() => write(last_parameter_gateway, last_parameter_gateway.Length));
                    await Task.Run(() => read12_gateway());
                }

                await Task.Run(() => LastParameter(address));
            }
            else
            {
                //переход к следующему счётчику
                if (meterListCallIndex != 0)
                {
                    EndInfoAboutSessionList[index_endInfoAboutSession].state = "Нет ответа на запрос аутентификации.";

                    index_endInfoAboutSession = index_endInfoAboutSession + 1;

                    richTextBox_conStatus2.Clear();

                    meterListCallIndex = meterListCallIndex - 1;
                    Thread.Sleep(2000);
                    //iconButton1_MouseDown_func();
                    await Task.Run(() => iconButton1_MouseDown_func());
                }
                else
                {
                    EndInfoAboutSessionList[index_endInfoAboutSession].state = "Нет ответа на запрос аутентификации.";

                    //закрытие соединения с модемом
                    await Task.Run(() => write_disconnect());
                    await Task.Run(() => read4());
                    await Task.Run(() => EnterEndInfoAboutSession());

                    //await Task.Run(()=>restartApp());
                }
            }
        }

            private void EnterEndInfoAboutSession()
        {
            richTextBox_conStatus2.AppendText("\n\n.................................................................................................\nОТЧЁТ ЗА СЕССИЮ\n................................................................................................."); richTextBox_conStatus2.ScrollToCaret();

            for (int i = 0; i < EndInfoAboutSessionList.Count; i++)
            {
                richTextBox_conStatus2.AppendText("\nСетевой адрес: " + EndInfoAboutSessionList[i].meter_address + "->" + EndInfoAboutSessionList[i].state);
                richTextBox_conStatus2.ScrollToCaret();
            }

            StatusTextBox.ForeColor = Color.Red;
            StatusTextBox.Text = "DISCONNECTED";

            TimeoutTextBox2.Text = "(............)";
            textBox1.Text = "(............)";
            textBox2.Text = "(............)";

            MessageBox.Show(
            "Опрос счётчиков окончен.\nСоединение с модемом закрыто.\nОтчёт о результате опроса смотрите в окне логов.",
                "kzmp_energy notification",
           MessageBoxButtons.OK,
           MessageBoxIcon.Information,
           MessageBoxDefaultButton.Button1,
           MessageBoxOptions.DefaultDesktopOnly);

            //Application.Restart();
            //iconButton3.PerformClick();
        }
        private void timerToRestart()
        {
            //MessageBox.Show("Через 5 секунд будет выполен рестарт по одной из следующих причин: \n1)Нет ответа от счетчика на запрос аутентификации;\n2)Выполнено отключение GSM модема. ");

            MessageBox.Show(
                "Через 5 секунд будет выполен рестарт по одной из следующих причин: \n1)Нет ответа от счетчика на запрос аутентификации;\n2)Выполнено отключение GSM модема. ",
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);

            Thread.Sleep(5000);
        }
        private void toNull()
        {
            meterListCallIndex = 0;
        }
        private void restartApp() 
        {
            //MessageBox.Show("Опрос счётчиков окончен.\nСоединение с модемом закрыто.\nБудет выполнен рестарт программ.");
            MessageBox.Show(
             "Опрос счётчиков окончен.\nСоединение с модемом закрыто.\nОтчёт о результате опроса смотрите в окне логов.\nПосле нажатия на кнопку ОК будет выполнен рестарт программ.",
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.DefaultDesktopOnly);

           

            Application.Restart();
            //MessageBox.Show("Выполнен рестарт программы по одной из следующих причин: \n1)Нет ответа от счетчика на запрос аутентификации;\n2)Выполнено отключение GSM модема. ");
        }
        private void read_var_isp()
        {
            try
            {
                Thread.Sleep(500);
                timeOver = 500;
                List<byte> msg = new List<byte>();
                int i = 0;
                int count = 0;
                int lng = 0;
                while (lng < 9)
                {
                    TimeoutTextBox2.Text = Convert.ToString(timeOver) + "(мс)";
                    if (!port.CDHolding)
                    {
                        StatusTextBox.Text = "NO CARRIER";
                        global_status_flag = false;

                        MessageBox.Show(
                        "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                        "kzmp_energy notification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                        Application.Restart();
                    }

                    i++;
                    count = port.BytesToRead;
                    if (count != 0)
                    {
                        byte[] ByteArray = new byte[count];
                        //ByteArray = Encoding.Default.GetBytes(port.ReadExisting());
                        port.Read(ByteArray, 0, count);
                        foreach (byte f in ByteArray)
                        {
                            msg.Add(f);
                        }
                        lng = msg.Count;
                    }
                    Thread.Sleep(500);
                    timeOver = timeOver + 500;

                    if (timeOver >= 7000)
                    {
                        timeOverFlag = true;

                        richTextBox_conStatus2.AppendText("\n#Нет ответа от счетчика. Повтор запроса.\n");
                        richTextBox_conStatus2.ScrollToCaret();
                        break;
                    }

                    if (lng >= 9)
                    {
                        timeOverFlag = false;
                    }
                }

                if (!timeOverFlag)
                {
                    byte k = msg[2];
                    byte mask = 0b00001111;

                    int Aindex = k & mask;

                    A = Alist[Aindex];

                    //проверка CRC ответа
                    byte[] check_crc_massa = new byte[] { msg[0], msg[1], msg[2], msg[3], msg[4], msg[5], msg[6] };
                    byte[] crc_out = CalculateCrc16Modbus(check_crc_massa);
                    if (crc_out[0] == msg[7] && crc_out[1] == msg[8])
                    {
                        crc_check_flag = true;
                    }
                    else { crc_check_flag = false; }
                }
            }
            catch (Exception ex)
             {

                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);

            }

        }
        private void read_var_isp_gateway()
        {
            try
            {
                Thread.Sleep(500);
                timeOver = 500;
                List<byte> msg = new List<byte>();
                int i = 0;
                int count = 0;
                int lng = 0;
                while (lng < 18)
                {
                    TimeoutTextBox2.Text = Convert.ToString(timeOver) + "(мс)";
                    if (!port.CDHolding)
                    {
                        StatusTextBox.Text = "NO CARRIER";
                        global_status_flag = false;

                        MessageBox.Show(
                        "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                        "kzmp_energy notification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                        Application.Restart();
                    }

                    i++;
                    count = port.BytesToRead;
                    if (count != 0)
                    {
                        byte[] ByteArray = new byte[count];
                        //ByteArray = Encoding.Default.GetBytes(port.ReadExisting());
                        port.Read(ByteArray, 0, count);
                        foreach (byte f in ByteArray)
                        {
                            msg.Add(f);
                        }
                        lng = msg.Count;
                    }
                    Thread.Sleep(500);
                    timeOver = timeOver + 500;

                    if (timeOver >= 7000)
                    {
                        timeOverFlag = true;

                        richTextBox_conStatus2.AppendText("\n#Нет ответа от счетчика. Повтор запроса.\n");
                        richTextBox_conStatus2.ScrollToCaret();
                        break;
                    }

                    if (lng >= 18)
                    {
                        timeOverFlag = false;
                    }
                }

                if (!timeOverFlag)
                {
                    byte k = msg[10];
                    byte mask = 0b00001111;

                    int Aindex = k & mask;

                    A = Alist[Aindex];

                    byte[] check_crc_massa = new byte[] { msg[8], msg[9], msg[10], msg[11], msg[12], msg[13], msg[14] };
                    byte[] crc_out = CalculateCrc16Modbus(check_crc_massa);
                    if (crc_out[0] == msg[15] && crc_out[1] == msg[16])
                    {
                        crc_check_flag = true;
                    }
                    else { crc_check_flag = false; }
                }
            }
            catch (Exception ex)
            {
                richTextBox_conStatus2.AppendText("\n" + ex.Message);
                richTextBox_conStatus2.ScrollToCaret();

                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);

            }

        }

        private void read4()
        {
            try
            {
                Thread.Sleep(1000);
                timeOver = 1000;
                List<byte> msg = new List<byte>();
                int i = 0;
                int count = 0;
                int lng = 0;
                while (lng < 4)
                {
                    TimeoutTextBox2.Text = Convert.ToString(timeOver)+"(мс)";
                    if (!port.CDHolding)
                    {
                       StatusTextBox.Text = "NO CARRIER";
                        global_status_flag = false;

                        MessageBox.Show(
                        "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                        "kzmp_energy notification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                        Application.Restart();
                    }

                    i++;
                    count = port.BytesToRead;
                    if (count != 0)
                    {
                        byte[] ByteArray = new byte[count];
                        //ByteArray = Encoding.Default.GetBytes(port.ReadExisting());
                        port.Read(ByteArray, 0, count);
                        foreach (byte f in ByteArray)
                        {
                            msg.Add(f);
                        }
                        lng = lng + msg.Count;
                    }
                    Thread.Sleep(100);
                    timeOver = timeOver + 100;
                    
                    if(timeOver>=5000)
                    {
                        timeOverFlag = true;

                        richTextBox_conStatus2.AppendText("\n#Нет ответа от счетчика. Повтор запроса.\n");
                        richTextBox_conStatus2.ScrollToCaret();
                        break;
                    }

                    if (lng >= 4)
                    {
                        timeOverFlag = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification);
            }
        }
        private void read11_gateway()
        {
            try
            {
                Thread.Sleep(1000);
                timeOver = 1000;
                List<byte> msg = new List<byte>();
                int i = 0;
                int count = 0;
                int lng = 0;
                while (lng < 11)
                {
                    TimeoutTextBox2.Text = Convert.ToString(timeOver)+"(мс)";
                    if (!port.CDHolding)
                    {
                        StatusTextBox.Text = "NO CARRIER";
                        global_status_flag = false;

                        MessageBox.Show(
                        "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                        "kzmp_energy notification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                        Application.Restart();
                    }

                    i++;
                    count = port.BytesToRead;
                    if (count != 0)
                    {
                        byte[] ByteArray = new byte[count];
                        //ByteArray = Encoding.Default.GetBytes(port.ReadExisting());
                        port.Read(ByteArray, 0, count);
                        foreach (byte f in ByteArray)
                        {
                            msg.Add(f);
                        }
                        lng = lng + msg.Count;
                    }
                    Thread.Sleep(100);
                    timeOver = timeOver + 100;
                    //if (i == 20) { write(z, x); }
                    //if (i == 40) { write(z, x); }
                    //if (i == 60) { MessageBox.Show("BAD Meter. Port Closing..."); port.Close(); }
                    if (timeOver > 7000)
                    {
                        timeOverFlag = true;

                        richTextBox_conStatus2.AppendText("\n#Нет ответа от счетчика. Повтор запроса.\n");
                        richTextBox_conStatus2.ScrollToCaret();
                        break;
                    }

                    if (lng >= 11)
                    {
                        timeOverFlag = false;
                    }
                }
            }
            catch (Exception ex)
            {
                 richTextBox_conStatus2.AppendText("\n" + ex.Message);
                 richTextBox_conStatus2.ScrollToCaret();
                //MessageBox.Show(ex.Message);

                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);
            }
        }
        private async void read4_closing()
        {
            try
            {
                Thread.Sleep(500);
                List<byte> msg = new List<byte>();
                int i = 0;
                int count = 0;
                int lng = 0;
                int localTimeOver = 500;
                while (lng < 3)
                {
                    TimeoutTextBox2.Text = Convert.ToString(localTimeOver) + "(мс)";
                    if (!port.CDHolding)
                    {
                        StatusTextBox.Text = "NO CARRIER";
                        global_status_flag = false;

                        MessageBox.Show(
                        "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                        "kzmp_energy notification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                        Application.Restart();
                    }

                    i++;
                    count = port.BytesToRead;
                    if (count != 0)
                    {
                        byte[] ByteArray = new byte[count];
                        //ByteArray = Encoding.Default.GetBytes(port.ReadExisting());
                        port.Read(ByteArray, 0, count);
                        foreach (byte f in ByteArray)
                        {
                            msg.Add(f);
                        }
                        lng = msg.Count;
                    }
                    Thread.Sleep(500);
                    localTimeOver = localTimeOver + 500;

                    if (localTimeOver > 7000)
                    {
                        break;

                    }
                }
                //переход к следующему счётчику
                if (meterListCallIndex != 0)
                {
                    EndInfoAboutSessionList[index_endInfoAboutSession].state = "Успех.";
                    index_endInfoAboutSession = index_endInfoAboutSession + 1;

                    richTextBox_conStatus2.Clear();

                    meterListCallIndex = meterListCallIndex - 1;
                    Thread.Sleep(2000);
                    await Task.Run(() => iconButton1_MouseDown_func());
                }
                /*else if (meterListCallIndex == 0)
                {
                    Thread.Sleep(10000);
                    iconButton1_MouseDown_func();
                }*/
                else
                {
                    EndInfoAboutSessionList[index_endInfoAboutSession].state = "Успех.";
                    //закрытие соединения с модемом
                    await Task.Run(() => write_disconnect());
                    await Task.Run(() => read4());
                    await Task.Run(() => EnterEndInfoAboutSession());

                    //await Task.Run(() => restartApp());
                   // iconButton2.PerformClick();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);

                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);
            }
        }
        private void read12()
        {
            try
            {
                Thread.Sleep(500);
                timeOver = 500;
                List<byte> msg = new List<byte>();
                int count = 0;
                int lng = 0;
                while (lng < 12)
                {
                    if (!port.CDHolding)
                    {
                        StatusTextBox.Text = "NO CARRIER";
                        global_status_flag = false;

                        MessageBox.Show(
                        "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                        "kzmp_energy notification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                        Application.Restart();
                    }

                    count = port.BytesToRead;
                    if (count != 0)
                    {
                        byte[] ByteArray = new byte[count];
                        //string asd = port.ReadExisting();
                        //ByteArray = Encoding.Default.GetBytes(asd);
                        port.Read(ByteArray, 0, count);
                        foreach (byte f in ByteArray)
                        {
                            msg.Add(f);
                        }
                        lng = msg.Count;
                    }
                    Thread.Sleep(500);
                    timeOver = timeOver + 500;
                    if (timeOver > 7000)
                    {
                        timeOverFlag = true;

                        richTextBox_conStatus2.AppendText("\n#Нет ответа от счетчика. Повтор запроса.\n");
                        richTextBox_conStatus2.ScrollToCaret();
                        break;
                    }
                    if (lng >= 12)
                    {
                        timeOverFlag = false;
                    }
                }

                if (!timeOverFlag)
                {
                    T = Convert.ToInt32(msg[9]);

                    for (int i = 0; i < massiv_last_parameter.Length; i++)
                    {
                        massiv_last_parameter[i] = msg[i];
                    }


                    //если счетчик типа Меркурий 234, то нужно определить 3-ий байт в 06h запросе (определить 17 бит)
                    if (meter_type[selected_index].Contains("234"))
                    {
                        string first_str = massiv_last_parameter[1].ToString("X");
                        string second_str = massiv_last_parameter[2].ToString("X");

                        foreach (string a in bagList)
                        {
                            if (a == second_str)
                            {
                                second_str = second_str.Insert(0, "0");
                            }
                        }

                        string general_str = first_str + second_str + "0";
                        int k = Convert.ToInt32(general_str, 16);
                        string general_strBinary = Convert.ToString(k, 2);

                        if (general_strBinary.Length > 16)
                        {
                            int index_in_strBin = general_strBinary.Length - 17;

                            string n = Convert.ToString(general_strBinary[index_in_strBin]);

                            int m = Convert.ToInt32(n);

                            if (m == 0)
                            {
                                bit17for234 = 0x03;
                            }
                            else { bit17for234 = 0x83; }
                        }
                        else
                        {
                            bit17for234 = 0x03;
                        }
                    }
                    //проверка CRC
                    byte[] check_crc_massa = new byte[] { msg[0], msg[1], msg[2], msg[3], msg[4], msg[5], msg[6], msg[7], msg[8], msg[9] };
                    byte[] crc_out = CalculateCrc16Modbus(check_crc_massa);
                    if (crc_out[0] == msg[10] && crc_out[1] == msg[11])
                    {
                        crc_check_flag = true;
                    }
                    else { crc_check_flag = false; }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(
               ex.Message,
                "kzmp_energy notification",
                  MessageBoxButtons.OK,
                 MessageBoxIcon.Information,
                  MessageBoxDefaultButton.Button1,
                 MessageBoxOptions.ServiceNotification);
            }
        }
        private void read12_gateway()
        {
            try
            {
                Thread.Sleep(500);
                timeOver = 500;
                List<byte> msg = new List<byte>();
                int count = 0;
                int lng = 0;
                while (lng < 21)
                {
                    TimeoutTextBox2.Text = Convert.ToString(timeOver) + "(мс)";
                    if (!port.CDHolding)
                    {
                        StatusTextBox.Text = "NO CARRIER";
                        global_status_flag = false;

                        MessageBox.Show(
                        "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                        "kzmp_energy notification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                        Application.Restart();
                    }

                    count = port.BytesToRead;
                    if (count != 0)
                    {
                        byte[] ByteArray = new byte[count];
                        //string asd = port.ReadExisting();
                        //ByteArray = Encoding.Default.GetBytes(asd);
                        port.Read(ByteArray, 0, count);
                        foreach (byte f in ByteArray)
                        {
                            msg.Add(f);
                        }
                        lng = msg.Count;
                    }
                    Thread.Sleep(500);
                    timeOver = timeOver + 500;

                    if (timeOver >= 7000)
                    {
                        timeOverFlag = true;

                        richTextBox_conStatus2.AppendText("\n#Нет ответа от счетчика. Повтор запроса.\n");
                        richTextBox_conStatus2.ScrollToCaret();
                        break;
                    }

                    if (lng >= 21)
                    {
                        timeOverFlag = false;
                    }
                }

                if (!timeOverFlag)
                {
                    T = Convert.ToInt32(msg[17]);

                    int j = 0;

                    for (int i = 8; i < msg.Count; i++)
                    {
                        if (j < 11)
                        {
                            massiv_last_parameter[i - 8] = msg[i];
                        }
                        j++;
                    }


                    //если счетчик типа Меркурий 234, то нужно определить 3-ий байт в 06h запросе (определить 17 бит)
                    if (meter_type[selected_index].Contains("234"))
                    {
                        string first_str = massiv_last_parameter[1].ToString("X");
                        string second_str = massiv_last_parameter[2].ToString("X");

                        foreach (string a in bagList)
                        {
                            if (a == second_str)
                            {
                                second_str = second_str.Insert(0, "0");
                            }
                        }

                        string general_str = first_str + second_str + "0";
                        int k = Convert.ToInt32(general_str, 16);
                        string general_strBinary = Convert.ToString(k, 2);

                        if (general_strBinary.Length > 16)
                        {
                            int index_in_strBin = general_strBinary.Length - 17;

                            string n = Convert.ToString(general_strBinary[index_in_strBin]);

                            int m = Convert.ToInt32(n);

                            if (m == 0)
                            {
                                bit17for234 = 0x03;
                            }
                            else { bit17for234 = 0x83; }
                        }
                        else
                        {
                            bit17for234 = 0x03;
                        }
                    }

                    //проверка CRC
                    byte[] check_crc_massa = new byte[] { msg[8], msg[9], msg[10], msg[11], msg[12], msg[13], msg[14], msg[15], msg[16], msg[17] };
                    byte[] crc_out = CalculateCrc16Modbus(check_crc_massa);
                    if (crc_out[0] == msg[18] && crc_out[1] == msg[19])
                    {
                        crc_check_flag = true;
                    }
                    else { crc_check_flag = false; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
               ex.Message,
                "kzmp_energy notification",
                  MessageBoxButtons.OK,
                 MessageBoxIcon.Information,
                  MessageBoxDefaultButton.Button1,
                 MessageBoxOptions.ServiceNotification);
            }
        }
        private void  read30_find_true_address(ref byte older, ref byte young)
        {
            Thread.Sleep(500);
            List<byte> msg = new List<byte>();

            int count = 0;
            int lng = 0;
            while (lng < 30)
            {
                count = port.BytesToRead;
                if (count != 0)
                {
                    byte[] ByteArray = new byte[count];
                    port.Read(ByteArray, 0, count);
                    foreach (byte f in ByteArray)
                    {
                        msg.Add(f);
                    }
                    lng = msg.Count;
                }
                Thread.Sleep(1000);
            }
            byte hour = msg[2];
            int hour_b = Convert.ToInt32(hour);

            byte minute = msg[3];
            int minute_b = Convert.ToInt32(minute);

            byte day = msg[4];
            int day_b = Convert.ToInt32(day);

            byte month = msg[5];
            int month_b = Convert.ToInt32(month);

            byte year = msg[6];
            int year_b = Convert.ToInt32(year);

            string last_data = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X") + " " + hour_b.ToString("X") + ":" + minute_b.ToString("X");

            richTextBox_conStatus2.AppendText("\n"+last_data);
            richTextBox_conStatus2.ScrollToCaret();

            int fds = Count30(last_data);
            if (fds != 0)
            {
                if (young == 0xF0)
                {
                    if (older != 0xFF)
                    { older = Convert.ToByte(older + 0x01); }
                    else { older = 0x00; }
                    young = 0x00;
                }
                else
                {
                    young = Convert.ToByte(young + 0x10);
                }
            }
            else
            {
                power_profile_flag = false;
            }
        }
        private void read30_power_profile(int id_meter, int address, ref byte older, ref byte young, ref byte code_write_profile, ref int count_local)
        {
            Thread.Sleep(500);
            timeOver = 500;
            List<byte> msg = new List<byte>();
            int count = 0;
            int lng = 0;
            int timeCountIndex = 500;
            while (lng < 33)
            {
                TimeoutTextBox2.Text = Convert.ToString(timeCountIndex + " (мс)");
                if (!port.CDHolding)
                {
                    StatusTextBox.Text = "NO CARRIER";
                    global_status_flag = false;

                    MessageBox.Show(
                    "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                    "kzmp_energy notification",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                    Application.Restart();
                }
                count = port.BytesToRead;
                if (count != 0)
                {
                    byte[] ByteArray = new byte[count];
                    port.Read(ByteArray, 0, count);
                    foreach (byte f in ByteArray)
                    {
                        msg.Add(f);
                    }
                    lng = msg.Count;
                }
                Thread.Sleep(500);
                timeCountIndex = timeCountIndex + 500;
                timeOver = timeOver + 500;

                if (timeOver > 7000)
                {
                    timeOverFlag = true;

                    richTextBox_conStatus2.AppendText("\n#Нет ответа от счетчика. Повтор запроса.\n");
                    richTextBox_conStatus2.ScrollToCaret();
                    break;
                }

                if (lng >= 33)
                {
                    timeOverFlag = false;
                }
            }

            if (!timeOverFlag)
            {
                //проверка CRC
                byte[] check_crc_massa = new byte[] { msg[0], msg[1], msg[2], msg[3], msg[4], msg[5], msg[6],
                                                    msg[7], msg[8], msg[9], msg[10], msg[11], msg[12], msg[13],
                                                        msg[14], msg[15], msg[16], msg[17], msg[18], msg[19], msg[20],
                                                            msg[21], msg[22], msg[23], msg[24], msg[25], msg[26], msg[27],
                                                                msg[28], msg[29], msg[30] };
                byte[] crc_out = CalculateCrc16Modbus(check_crc_massa);
                if (crc_out[0] == msg[31] && crc_out[1] == msg[32])
                {
                    crc_check_flag = true;


                    byte hour = msg[2];
                    int hour_b = Convert.ToInt32(hour);

                    byte minute = msg[3];
                    int minute_b = Convert.ToInt32(minute);

                    byte day = msg[4];
                    int day_b = Convert.ToInt32(day);

                    byte month = msg[5];
                    int month_b = Convert.ToInt32(month);

                    byte year = msg[6];
                    int year_b = Convert.ToInt32(year);
                    string last_data = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X") + " " + hour_b.ToString("X") + ":" + minute_b.ToString("X");

                    richTextBox_conStatus2.AppendText("\t -->" + last_data);
                    richTextBox_conStatus2.ScrollToCaret();

                     textBox2.Text = last_data;
                     textBox1.Text = "ОК";



                    //1-ая часть ответа

                    string data = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X");
                    string time = hour_b.ToString("X") + ":" + minute_b.ToString("X");

                    if (data == "0.0.0") { data = "12.12.12"; };

                    string bagMsg = msg[8].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg)
                        {
                            bagMsg = bagMsg.Insert(0, "0");
                        }
                    }
                    string p_plus_str = msg[9].ToString("X") + bagMsg;
                    int p_plus = Convert.ToInt32(p_plus_str, 16);
                    //float p_plus_f = Convert.ToSingle(p_plus) / 1000;
                    float p_plus_f = Convert.ToSingle(p_plus);
                    calcRealPowerProfile(ref p_plus_f, T, A);
                    p_plus_str = Convert.ToString(p_plus_f);
                    if (p_plus_str.Contains(",")) p_plus_str = p_plus_str.Replace(",", ".");

                    string bagMsg2 = msg[10].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg2)
                        {
                            bagMsg2 = bagMsg2.Insert(0, "0");
                        }
                    }
                    string p_minus_str = msg[11].ToString("X") + bagMsg2;
                    int p_minus = Convert.ToInt32(p_minus_str, 16);
                    float p_minus_f = Convert.ToSingle(p_minus);
                    calcRealPowerProfile(ref p_minus_f, T, A);
                    //if (p_minus_f > 65) { p_minus_f = 0; }
                    if (p_minus_str == "FFFF") { p_minus_f = 0; }
                    p_minus_str = Convert.ToString(p_minus_f);
                    if (p_minus_str.Contains(",")) p_minus_str = p_minus_str.Replace(",", ".");

                    string bagMsg3 = msg[12].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg3)
                        {
                            bagMsg3 = bagMsg3.Insert(0, "0");
                        }
                    }
                    string q_plus_str = msg[13].ToString("X") + bagMsg3;
                    int q_plus = Convert.ToInt32(q_plus_str, 16);
                    float q_plus_f = Convert.ToSingle(q_plus);
                    calcRealPowerProfile(ref q_plus_f, T, A);
                    q_plus_str = Convert.ToString(q_plus_f);
                    if (q_plus_str.Contains(",")) q_plus_str = q_plus_str.Replace(",", ".");

                    string bagMsg4 = msg[14].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg4)
                        {
                            bagMsg4 = bagMsg4.Insert(0, "0");
                        }
                    }
                    string q_minus_str = msg[15].ToString("X") + bagMsg4;
                    int q_minus = Convert.ToInt32(q_minus_str, 16);
                    float q_minus_f = Convert.ToSingle(q_minus);
                    calcRealPowerProfile(ref q_minus_f, T, A);
                    //if (q_minus_f > 65) { q_minus_f = 0; }
                    if (q_minus_str == "FFFF") { q_minus_f = 0; }
                    q_minus_str = Convert.ToString(q_minus_f);
                    if (q_minus_str.Contains(",")) q_minus_str = q_minus_str.Replace(",", ".");

                    try
                    {
                        sql_cmd = "insert into dbo.power_profile_m values (" + Convert.ToString(id_meter) + "," + Convert.ToString(address) + ", '" + data + "', '" + time + "'," + p_plus_str + "," + p_minus_str + "," + q_plus_str + "," + q_minus_str + ");";
                        SqlCommand cmd = new SqlCommand(sql_cmd, connection);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        richTextBox_conStatus2.AppendText("\nERROR: " + ex.Message + "\n");
                        richTextBox_conStatus2.ScrollToCaret();
                    }

                    //2-ая часть ответа
                    hour = msg[17];
                    hour_b = Convert.ToInt32(hour);

                    minute = msg[18];
                    minute_b = Convert.ToInt32(minute);

                    day = msg[19];
                    day_b = Convert.ToInt32(day);

                    month = msg[20];
                    month_b = Convert.ToInt32(month);

                    year = msg[21];
                    year_b = Convert.ToInt32(year);

                    //string last_data2 = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X") + " " + hour_b.ToString("X") + ":" + minute_b.ToString("X");
                    //int count_end2 = Count30_end(last_data);

                    // if (count_end2 > 0)
                    // {
                    string data2 = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X");
                    string time2 = hour_b.ToString("X") + ":" + minute_b.ToString("X");

                    if (data2 == "0.0.0") { data2 = "12.12.12"; };

                    string bagMsg5 = msg[23].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg5)
                        {
                            bagMsg5 = bagMsg5.Insert(0, "0");
                        }
                    }
                    string p_plus_str2 = msg[24].ToString("X") + bagMsg5;
                    int p_plus2 = Convert.ToInt32(p_plus_str2, 16);
                    float p_plus_f2 = Convert.ToSingle(p_plus2);
                    calcRealPowerProfile(ref p_plus_f2, T, A);
                    p_plus_str2 = Convert.ToString(p_plus_f2);
                    if (p_plus_str2.Contains(",")) p_plus_str2 = p_plus_str2.Replace(",", ".");

                    string bagMsg6 = msg[25].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg6)
                        {
                            bagMsg6 = bagMsg6.Insert(0, "0");
                        }
                    }
                    string p_minus_str2 = msg[26].ToString("X") + bagMsg6;
                    int p_minus2 = Convert.ToInt32(p_minus_str2, 16);
                    float p_minus_f2 = Convert.ToSingle(p_minus2);
                    calcRealPowerProfile(ref p_minus_f2, T, A);
                    //if (p_minus_f2 > 65) { p_minus_f2 = 0; }
                    if (p_minus_str2 == "FFFF") { p_minus_f2 = 0; }
                    p_minus_str2 = Convert.ToString(p_minus_f2);
                    if (p_minus_str2.Contains(",")) p_minus_str2 = p_minus_str2.Replace(",", ".");

                    string bagMsg7 = msg[27].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg7)
                        {
                            bagMsg7 = bagMsg7.Insert(0, "0");
                        }
                    }
                    string q_plus_str2 = msg[28].ToString("X") + bagMsg7;
                    int q_plus2 = Convert.ToInt32(q_plus_str2, 16);
                    float q_plus_f2 = Convert.ToSingle(q_plus2);
                    calcRealPowerProfile(ref q_plus_f2, T, A);
                    q_plus_str2 = Convert.ToString(q_plus_f2);
                    if (q_plus_str2.Contains(",")) q_plus_str2 = q_plus_str2.Replace(",", ".");

                    string bagMsg8 = msg[29].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg8)
                        {
                            bagMsg8 = bagMsg8.Insert(0, "0");
                        }
                    }
                    string q_minus_str2 = msg[30].ToString("X") + bagMsg8;
                    int q_minus2 = Convert.ToInt32(q_minus_str2, 16);
                    float q_minus_f2 = Convert.ToSingle(q_minus2);
                    calcRealPowerProfile(ref q_minus_f2, T, A);
                    //if (q_minus_f2 > 65) { q_minus_f2 = 0; }
                    if (q_minus_str2 == "FFFF") { q_minus_f2 = 0; }
                    q_minus_str2 = Convert.ToString(q_minus_f2);
                    if (q_minus_str2.Contains(",")) q_minus_str2 = q_minus_str2.Replace(",", ".");

                    try
                    {
                        sql_cmd = "insert into dbo.power_profile_m values (" + Convert.ToString(id_meter) + "," + Convert.ToString(address) + ", '" + data2 + "', '" + time2 + "'," + p_plus_str2 + "," + p_minus_str2 + "," + q_plus_str2 + "," + q_minus_str2 + ");";
                        SqlCommand cmd2 = new SqlCommand(sql_cmd, connection);
                        cmd2.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        richTextBox_conStatus2.AppendText("\nERROR: " + ex.Message + "\n");
                        richTextBox_conStatus2.ScrollToCaret();
                    }


                    if (young != 0xE0 & young != 0xF0)
                    {
                        young = Convert.ToByte(young + 0x20);
                    }
                    else
                    {
                        if (older != 0xFF)
                        { older = Convert.ToByte(older + 0x01); }
                        else
                        {
                            older = 0x00;
                            //смена 17-го бита для Меркурия 234 происходит при переходе старшего байта с 0x00 в 0xff и наоборот
                            if (bit17for234 == 0x03)
                            {
                                bit17for234 = 0x83;
                            }
                            else
                            {
                                bit17for234 = 0x03;
                            }
                            code_write_profile = bit17for234;
                        }
                        young = 0x00;
                    }
                    count_local = count_local - 1;
                }
                else
                {
                    crc_check_flag = false;

                    richTextBox_conStatus2.AppendText("\n###Повтор запроса - не совпадение CRC-значений!###");
                    richTextBox_conStatus2.ScrollToCaret();

                    textBox2.Text = "######################";
                    textBox1.Text = "Не совпадение хэш-сумм";

                }
            }
        }
        private void read30_power_profile_gateway(int id_meter, int address, ref byte older, ref byte young, ref byte code_write_profile, ref int count_local)
        {
            Thread.Sleep(500);
            timeOver = 500;
            List<byte> msg = new List<byte>();
            int count = 0;
            int lng = 0;
            int timeCountIndex = 500;
            while (lng < 42)
            {
                TimeoutTextBox2.Text = Convert.ToString(timeCountIndex + " (мс)");
                if (!port.CDHolding)
                {
                    StatusTextBox.Text = "NO CARRIER";
                    global_status_flag = false;

                    MessageBox.Show(
                    "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                    "kzmp_energy notification",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                    Application.Restart();
                }

                count = port.BytesToRead;
                if (count != 0)
                {
                    byte[] ByteArray = new byte[count];
                    port.Read(ByteArray, 0, count);
                    foreach (byte f in ByteArray)
                    {
                        msg.Add(f);
                    }
                    lng = msg.Count;
                }
                Thread.Sleep(500);
                timeCountIndex = timeCountIndex + 500;
                timeOver = timeOver + 500;

                if (timeOver > 7000)
                {
                    timeOverFlag = true;

                    richTextBox_conStatus2.AppendText("\n#Нет ответа от счетчика. Повтор запроса.\n");
                    richTextBox_conStatus2.ScrollToCaret();
                    break;
                }

                if (lng >= 42)
                {
                    timeOverFlag = false;
                }
            }

            if (!timeOverFlag)
            {
                //проверка CRC
                byte[] check_crc_massa = new byte[] { msg[8], msg[9], msg[10], msg[11], msg[12], msg[13], msg[14],
                                                    msg[15], msg[16], msg[17], msg[18], msg[19], msg[20], msg[21],
                                                        msg[22], msg[23], msg[24], msg[25], msg[26], msg[27], msg[28],
                                                            msg[29], msg[30], msg[31], msg[32], msg[33], msg[34], msg[35],
                                                                msg[36], msg[37], msg[38] };
                byte[] crc_out = CalculateCrc16Modbus(check_crc_massa);
                if (crc_out[0] == msg[39] && crc_out[1] == msg[40])
                {
                    crc_check_flag = true;


                    for (int i = 0; i < 8; i++)
                    {
                        msg.RemoveAt(0);
                    }

                    byte hour = msg[2];
                    int hour_b = Convert.ToInt32(hour);

                    byte minute = msg[3];
                    int minute_b = Convert.ToInt32(minute);

                    byte day = msg[4];
                    int day_b = Convert.ToInt32(day);

                    byte month = msg[5];
                    int month_b = Convert.ToInt32(month);

                    byte year = msg[6];
                    int year_b = Convert.ToInt32(year);
                    string last_data = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X") + " " + hour_b.ToString("X") + ":" + minute_b.ToString("X");

                    richTextBox_conStatus2.AppendText("\t -->" + last_data);
                    richTextBox_conStatus2.ScrollToCaret();

                    textBox2.Text = last_data;
                    textBox1.Text = "OK";


                    //1-ая часть ответа

                    string data = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X");
                    string time = hour_b.ToString("X") + ":" + minute_b.ToString("X");

                    if (data == "0.0.0") { data = "12.12.12"; };

                    string bagMsg = msg[8].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg)
                        {
                            bagMsg = bagMsg.Insert(0, "0");
                        }
                    }
                    string p_plus_str = msg[9].ToString("X") + bagMsg;
                    int p_plus = Convert.ToInt32(p_plus_str, 16);
                    //float p_plus_f = Convert.ToSingle(p_plus) / 1000;
                    float p_plus_f = Convert.ToSingle(p_plus);
                    calcRealPowerProfile(ref p_plus_f, T, A);
                    p_plus_str = Convert.ToString(p_plus_f);
                    if (p_plus_str.Contains(",")) p_plus_str = p_plus_str.Replace(",", ".");

                    string bagMsg2 = msg[10].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg2)
                        {
                            bagMsg2 = bagMsg2.Insert(0, "0");
                        }
                    }
                    string p_minus_str = msg[11].ToString("X") + bagMsg2;
                    int p_minus = Convert.ToInt32(p_minus_str, 16);
                    float p_minus_f = Convert.ToSingle(p_minus);
                    calcRealPowerProfile(ref p_minus_f, T, A);
                  
                    if (p_minus_str == "FFFF") { p_minus_f = 0; }
                    p_minus_str = Convert.ToString(p_minus_f);
                    if (p_minus_str.Contains(",")) p_minus_str = p_minus_str.Replace(",", ".");

                    string bagMsg3 = msg[12].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg3)
                        {
                            bagMsg3 = bagMsg3.Insert(0, "0");
                        }
                    }
                    string q_plus_str = msg[13].ToString("X") + bagMsg3;
                    int q_plus = Convert.ToInt32(q_plus_str, 16);
                    float q_plus_f = Convert.ToSingle(q_plus);
                    calcRealPowerProfile(ref q_plus_f, T, A);
                    q_plus_str = Convert.ToString(q_plus_f);
                    if (q_plus_str.Contains(",")) q_plus_str = q_plus_str.Replace(",", ".");

                    string bagMsg4 = msg[14].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg4)
                        {
                            bagMsg4 = bagMsg4.Insert(0, "0");
                        }
                    }
                    string q_minus_str = msg[15].ToString("X") + bagMsg4;
                    int q_minus = Convert.ToInt32(q_minus_str, 16);
                    float q_minus_f = Convert.ToSingle(q_minus);
                    calcRealPowerProfile(ref q_minus_f, T, A);
                    
                    if (q_minus_str == "FFFF") { q_minus_f = 0; }
                    q_minus_str = Convert.ToString(q_minus_f);
                    if (q_minus_str.Contains(",")) q_minus_str = q_minus_str.Replace(",", ".");

                    try
                    {
                        sql_cmd = "insert into dbo.power_profile_m values (" + Convert.ToString(id_meter) + "," + Convert.ToString(address) + ", '" + data + "', '" + time + "'," + p_plus_str + "," + p_minus_str + "," + q_plus_str + "," + q_minus_str + ");";
                        SqlCommand cmd = new SqlCommand(sql_cmd, connection);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        richTextBox_conStatus2.AppendText("\nERROR: " + ex.Message + "\n");
                        richTextBox_conStatus2.ScrollToCaret();
                    }

                    //2-ая часть ответа
                    hour = msg[17];
                    hour_b = Convert.ToInt32(hour);

                    minute = msg[18];
                    minute_b = Convert.ToInt32(minute);

                    day = msg[19];
                    day_b = Convert.ToInt32(day);

                    month = msg[20];
                    month_b = Convert.ToInt32(month);

                    year = msg[21];
                    year_b = Convert.ToInt32(year);

                    //string last_data2 = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X") + " " + hour_b.ToString("X") + ":" + minute_b.ToString("X");
                    //int count_end2 = Count30_end(last_data);

                    // if (count_end2 > 0)
                    // {
                    string data2 = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X");
                    string time2 = hour_b.ToString("X") + ":" + minute_b.ToString("X");

                    if (data2 == "0.0.0") { data2 = "12.12.12"; };

                    string bagMsg5 = msg[23].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg5)
                        {
                            bagMsg5 = bagMsg5.Insert(0, "0");
                        }
                    }
                    string p_plus_str2 = msg[24].ToString("X") + bagMsg5;
                    int p_plus2 = Convert.ToInt32(p_plus_str2, 16);
                    float p_plus_f2 = Convert.ToSingle(p_plus2);
                    calcRealPowerProfile(ref p_plus_f2, T, A);
                    p_plus_str2 = Convert.ToString(p_plus_f2);
                    if (p_plus_str2.Contains(",")) p_plus_str2 = p_plus_str2.Replace(",", ".");

                    string bagMsg6 = msg[25].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg6)
                        {
                            bagMsg6 = bagMsg6.Insert(0, "0");
                        }
                    }
                    string p_minus_str2 = msg[26].ToString("X") + bagMsg6;
                    int p_minus2 = Convert.ToInt32(p_minus_str2, 16);
                    float p_minus_f2 = Convert.ToSingle(p_minus2);
                    calcRealPowerProfile(ref p_minus_f2, T, A);
                   
                    if (p_minus_str2 == "FFFF") { p_minus_f2 = 0; }
                    p_minus_str2 = Convert.ToString(p_minus_f2);
                    if (p_minus_str2.Contains(",")) p_minus_str2 = p_minus_str2.Replace(",", ".");

                    string bagMsg7 = msg[27].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg7)
                        {
                            bagMsg7 = bagMsg7.Insert(0, "0");
                        }
                    }
                    string q_plus_str2 = msg[28].ToString("X") + bagMsg7;
                    int q_plus2 = Convert.ToInt32(q_plus_str2, 16);
                    float q_plus_f2 = Convert.ToSingle(q_plus2);
                    calcRealPowerProfile(ref q_plus_f2, T, A);
                    q_plus_str2 = Convert.ToString(q_plus_f2);
                    if (q_plus_str2.Contains(",")) q_plus_str2 = q_plus_str2.Replace(",", ".");

                    string bagMsg8 = msg[29].ToString("X");
                    foreach (string a in bagList)
                    {
                        if (a == bagMsg8)
                        {
                            bagMsg8 = bagMsg8.Insert(0, "0");
                        }
                    }
                    string q_minus_str2 = msg[30].ToString("X") + bagMsg8;
                    int q_minus2 = Convert.ToInt32(q_minus_str2, 16);
                    float q_minus_f2 = Convert.ToSingle(q_minus2);
                    calcRealPowerProfile(ref q_minus_f2, T, A);
                    
                    if (q_minus_str2 == "FFFF") { q_minus_f2 = 0; }
                    q_minus_str2 = Convert.ToString(q_minus_f2);
                    if (q_minus_str2.Contains(",")) q_minus_str2 = q_minus_str2.Replace(",", ".");

                    try
                    {
                        sql_cmd = "insert into dbo.power_profile_m values (" + Convert.ToString(id_meter) + "," + Convert.ToString(address) + ", '" + data2 + "', '" + time2 + "'," + p_plus_str2 + "," + p_minus_str2 + "," + q_plus_str2 + "," + q_minus_str2 + ");";
                        SqlCommand cmd2 = new SqlCommand(sql_cmd, connection);
                        cmd2.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        richTextBox_conStatus2.AppendText("\nERROR: " + ex.Message + "\n");
                        richTextBox_conStatus2.ScrollToCaret();
                    }


                    if (young != 0xE0 & young != 0xF0)
                    {
                        young = Convert.ToByte(young + 0x20);
                    }
                    else
                    {
                        if (older != 0xFF)
                        { older = Convert.ToByte(older + 0x01); }
                        else
                        {
                            older = 0x00;
                            //смена 17-го бита для Меркурия 234 происходит при переходе старшего байта с 0x00 в 0xff и наоборот
                            if (bit17for234 == 0x03)
                            {
                                bit17for234 = 0x83;
                            }
                            else
                            {
                                bit17for234 = 0x03;
                            }
                            code_write_profile = bit17for234;
                        }
                        young = 0x00;
                    }
                    count_local = count_local - 1;
                }
                else
                {
                    crc_check_flag = false;

                    richTextBox_conStatus2.AppendText("\n###Повтор запроса - не совпадение CRC-значений!###");
                    richTextBox_conStatus2.ScrollToCaret();

                     textBox2.Text = "######################";
                     textBox1.Text = "Не совпадение хэш-сумм";
                }
            }
        }
        private void read_for_auth2()
        {
            Thread.Sleep(2000);
            List<byte> msg = new List<byte>();
            int i = 0;
            int count = 0;
            int lng = 0;
            while (lng < 4)
            {
                string data = port.ReadExisting();
                if (data == "") { Thread.Sleep(1000); }
                else { lng = 10; }
            }

        }
        private void write_to_box()
        {
            richTextBox_conStatus2.AppendText("\nПрофиль мощности загружен.");
            richTextBox_conStatus2.ScrollToCaret();

            TimeoutTextBox2.Text = "(.......)";
        }
        private void write_disconnect()
        {
            richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nОТКЛЮЧЕНИЕ\n-------------------------------------------------------------------------------------------------");
            richTextBox_conStatus2.ScrollToCaret();

            if (!port.CDHolding)
            {
                StatusTextBox.Text = "NO CARRIER";
                global_status_flag = false;

                MessageBox.Show(
                "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                "kzmp_energy notification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

                Application.Restart();
            }

            port.DiscardInBuffer();

            Thread.Sleep(1000);
            port.Write("+++");
            Thread.Sleep(2000);
            port.Write("ATH" + "\r");

            port.Close();

            richTextBox_conStatus2.AppendText("\nСоединение разорвано.");
            richTextBox_conStatus2.ScrollToCaret();

            //разблокировка кнопок
            g.btnHome.Enabled = true;
            g.iconButton1.Enabled = true;
            g.iconButton2.Enabled = true;
            g.iconButton3.Enabled = true;

            g.iconButton5.Enabled = true;
            g.iconButton6.Enabled = true;
            g.iconButton4.Enabled = true;

            iconButton1.Enabled = true;
            dataGridView1.Enabled = true;
            datePickerStart.Enabled = true;
            datePickerEnd.Enabled = true;
            timePickerStart.Enabled = true;
            timePickerEnd.Enabled = true;


        }
        private async void iconButton2_Click(object sender, EventArgs e)
        {
            

            MessageBox.Show(
                "Отключение. Будет выполнен рестарт программы.",
                 "kzmp_energy notification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.ServiceNotification);
            //await Task.Run(() => restartApp());
            Application.Restart();
        }

        private void calcRealPowerProfile(ref float x, float t, float a )
        {
            x = (x * 60 / t) / (2 * a);
        }

        private void iconButton3_Click_1(object sender, EventArgs e)
        {
            Application.Restart();
        }



        //----------------------------------------------------------------------------------------------------------------------------------------------------------
        //CODE FOR GSM-GATEWAY
        //----------------------------------------------------------------------------------------------------------------------------------------------------------
        
        
        //функция установления соединения с счётчиком 
        public async void MeterConnectionGateway(int address)
        {
            try
            {
                //УСТАНОВКА ПАРАМЕТРОВ НА ШЛЮЗЕ
                byte[] set_gateway_param = new byte[] { 0x27, 0xb7, 0xfc, 0x01, 0x00, 0x04, 0x00, 0x00, 0x01, 0x16, 0x33, 0x64, 0xad };

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nУСТАНОВКА ПАРАМЕТРОВ НА GSM-ШЛЮЗЕ\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                timeOver = 0;
                timeOverFlag = true;
                if (timeOverFlag)
                {
                    await Task.Run(() => write(set_gateway_param, set_gateway_param.Length));
                    await Task.Run(() => read11_gateway());
                }

                //ТЕСТ СВЯЗИ СО СЧЁТЧИКОМ
                //формирование запроса
                // msg345 = new List<byte>();

                byte hex = Convert.ToByte(address);

                byte[] meter_connection_test = new byte[13];
                byte[] meter_connection_test_crc = new byte[] { hex, 0x00 };
                byte[] crc = CalculateCrc16Modbus(meter_connection_test_crc);

                //num
                meter_connection_test[3] = 0x01;
                meter_connection_test[4] = 0x00;
                //len
                meter_connection_test[5] = 0x04;
                meter_connection_test[6] = 0x00;
                //port
                meter_connection_test[7] = 0x01;

                //crc24
                byte[] local_mas = new byte[] { meter_connection_test[3], meter_connection_test[4], meter_connection_test[5], meter_connection_test[6], meter_connection_test[7] };
                byte[] crc24 = CalculateCrc24(local_mas);
                meter_connection_test[0] = crc24[0];
                meter_connection_test[1] = crc24[1];
                meter_connection_test[2] = crc24[2];
                
                //payload
                meter_connection_test[8] = hex;
                meter_connection_test[9] = 0x00;
                meter_connection_test[10] = crc[0];
                meter_connection_test[11] = crc[1];

                //checksum
                meter_connection_test[12] = Convert.ToByte((meter_connection_test[8]+ meter_connection_test[9] + meter_connection_test[10] + meter_connection_test[11]+0xff)&0xff);

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nТЕСТИРОВАНИЕ СОЕДИНЕНИЯ СО СЧЁТЧИКОМ (адрес: " + Convert.ToString(address) + ")\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                timeOver = 0;
                timeOverFlag = true;
                if (timeOverFlag)
                {
                    await Task.Run(() => write(meter_connection_test, meter_connection_test.Length));
                    await Task.Run(() => read11_gateway());
                }

                //АУТЕНТИФИКАЦИЯ И АВТОРИЗАЦИЯ НА СЧЁТЧИКЕ
                byte[] ath_meter_crc = new byte[] { hex, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
                byte[] crc2 = CalculateCrc16Modbus(ath_meter_crc);
                byte[] ath_meter = new byte[11] { hex, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, crc2[0], crc2[1] };

                byte[] ath_meter_gateway = new byte[20];

                //payload
                ath_meter_gateway[8] = ath_meter[0];
                ath_meter_gateway[9] = ath_meter[1];
                ath_meter_gateway[10] = ath_meter[2];
                ath_meter_gateway[11] = ath_meter[3];
                ath_meter_gateway[12] = ath_meter[4];
                ath_meter_gateway[13] = ath_meter[5];
                ath_meter_gateway[14] = ath_meter[6];
                ath_meter_gateway[15] = ath_meter[7];
                ath_meter_gateway[16] = ath_meter[8];
                ath_meter_gateway[17] = ath_meter[9];
                ath_meter_gateway[18] = ath_meter[10];
                //num
                ath_meter_gateway[3] = 0x01;
                ath_meter_gateway[4] = 0x00;
                //len
                ath_meter_gateway[5] = 0x0b;
                ath_meter_gateway[6] = 0x00;
                //port
                ath_meter_gateway[7] = 0x01;
                //crc24
                local_mas[0] = ath_meter_gateway[3];
                local_mas[1] = ath_meter_gateway[4];
                local_mas[2] = ath_meter_gateway[5];
                local_mas[3] = ath_meter_gateway[6];
                local_mas[4] = ath_meter_gateway[7];
                crc24 = CalculateCrc24(local_mas);
                ath_meter_gateway[0] = crc24[0];
                ath_meter_gateway[1] = crc24[1];
                ath_meter_gateway[2] = crc24[2];
                //checksum
                ath_meter_gateway[19] = Convert.ToByte((ath_meter_gateway[8]+ ath_meter_gateway[9] + ath_meter_gateway[10] + 
                                            ath_meter_gateway[11] + ath_meter_gateway[12] + ath_meter_gateway[13] + ath_meter_gateway[14] + 
                                            ath_meter_gateway[15] + ath_meter_gateway[16] + ath_meter_gateway[17] + ath_meter_gateway[18]+0xff)&0xff);

                richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nАУТЕНТИФИКАЦИЯ И АВТОРИЗАЦИЯ НА СЧЁТЧИКЕ(адрес: " + Convert.ToString(address) + ")\n-------------------------------------------------------------------------------------------------");
                richTextBox_conStatus2.ScrollToCaret();

                await Task.Run(() => write(ath_meter_gateway, ath_meter_gateway.Length));
                //await Task.Run(() => read4(ath_meter, ath_meter.Length));
                await Task.Run(() => read_for_auth_gateway(ath_meter_gateway, ath_meter_gateway.Length, hex, address));
            }
            catch (Exception ex)
            {
                richTextBox_conStatus2.AppendText("\n" + ex.Message);
                richTextBox_conStatus2.ScrollToCaret();
                //MessageBox.Show(ex.Message);
               
                
                MessageBox.Show(
                ex.Message,
                 "kzmp_energy notification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification);

            }
        }

        private void datePickerStart_ValueChanged(object sender, EventArgs e)
        {
            DateTime date = datePickerStart.Value;
            int lastMonth = date.Month - 1;

            switch(lastMonth)
            {
                //case 0: {comboBox_Months.SelectedItem = comboBox_Months.Items[comboBox_Months.inde] break; }
            }
        }

        private void FormPowerProfile_Load(object sender, EventArgs e)
        {
            try
            {
                int currentMonth = DateTime.Now.Month;

                switch (currentMonth)
                {
                    case 1: { comboBox_Months.SelectedIndex = 11; break; }
                    case 2: { comboBox_Months.SelectedIndex = 0; break; }
                    case 3: { comboBox_Months.SelectedIndex = 1; break; }
                    case 4: { comboBox_Months.SelectedIndex = 2; break; }
                    case 5: { comboBox_Months.SelectedIndex = 3; break; }
                    case 6: { comboBox_Months.SelectedIndex = 4; break; }
                    case 7: { comboBox_Months.SelectedIndex = 5; break; }
                    case 8: { comboBox_Months.SelectedIndex = 6; break; }
                    case 9: { comboBox_Months.SelectedIndex = 7; break; }
                    case 10: { comboBox_Months.SelectedIndex = 8; break; }
                    case 11: { comboBox_Months.SelectedIndex = 9; break; }
                    case 12: { comboBox_Months.SelectedIndex = 10; break; }
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class meter_date_time_class
    {
        public string date;
        public string time;
    }

    public class EndInfoAboutSession
    {
        public string meter_address;
        public string state;
    }

}

