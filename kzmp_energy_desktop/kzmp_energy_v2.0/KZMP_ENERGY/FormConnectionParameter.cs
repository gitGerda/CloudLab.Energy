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
using System.Reflection;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;
using System.Runtime.InteropServices;

namespace KZMP_ENERGY
{
    public partial class FormConnectionParameter : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        public static string[] com_ports;
        public static SerialPort port;
        public static bool conn = false;
        byte[] buf = new byte[64];

        public static string sim_number;

        List<NumbersInfoClass> NumbersInfoList = new List<NumbersInfoClass>();

        //static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DBkzmp_energy;Integrated Security=True";
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        string sql_cmd = "select distinct SIM from dbo.meter";
        SqlCommand cmd;

        FormMainMenu b;

        //флаги 
        public static bool GSM_modem_flag;
        public static bool GSM_gateway_flag;


        public FormConnectionParameter(FormMainMenu h)
        {
            InitializeComponent();

            GSM_gateway_flag = false;
            GSM_modem_flag = false;

            b = h;

            com_ports = SerialPort.GetPortNames();
            foreach (string com_name in com_ports)
            { 
                comboBox_comPorts.Items.Add(com_name);
            }

            //comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox_BaudRate.SelectedIndex = 0;
            comboBox_ReadTimeout.SelectedIndex = 0;
            comboBox_WriteTimeout.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;


            //чтение информации о номерах
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
                cmd = new SqlCommand(sql_cmd, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //string sim = Convert.ToString(reader.GetValue(0));
                        //comboBox5.Items.Add(sim);

                        NumbersInfoClass localClass = new NumbersInfoClass();

                        localClass.number = Convert.ToString(reader.GetValue(0));

                        NumbersInfoList.Add(localClass);
                    }
                }
                reader.Close();

                for (int i = 0; i < NumbersInfoList.Count; i++)
                {
                    if (!(connection.State == ConnectionState.Open))
                    {
                        connection.Open();
                    }
                    string local_sql_cmd = "select CompanyName, Interface from dbo.meter where SIM='" + NumbersInfoList[i].number + "'";
                    cmd = new SqlCommand(local_sql_cmd, connection);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            NumbersInfoList[i].company = Convert.ToString(reader.GetValue(0));
                            NumbersInfoList[i].Interface = Convert.ToString(reader.GetValue(1));
                        }
                    }
                    reader.Close();
                }

                //вывод общей информации о номере в comboBox5
                for(int i=0;i<NumbersInfoList.Count;i++)
                {
                    string item = NumbersInfoList[i].company + " -> " + NumbersInfoList[i].number+" -> "+NumbersInfoList[i].Interface;
                    comboBox5.Items.Add(item);

                }
            
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
           SerialPort sp = (SerialPort)sender;
           string indata = sp.ReadExisting();
           indata = indata.Replace(" ", "");
           indata = indata.Replace("\r", "");
           indata = indata.Replace("\n", "");
          // richTextBox_conStatus.AppendText("_");

           port.DiscardInBuffer();
           //port.DiscardOutBuffer();
           //port.BaseStream.Flush();

           /*indata = sp.ReadExisting();
           indata = indata.Replace(" ", "");
           indata = indata.Replace("\r", "");
           indata = indata.Replace("\n", "");*/

           /*richTextBox_conStatus.AppendText("\nОтвет:" + indata );
           richTextBox_conStatus.ScrollToCaret();*/

            // Thread.Sleep(1000);
            // port.Write("+++");
            // Thread.Sleep(1000);
            port.DataReceived -= DataReceivedHandler;
            //port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler2);
            Thread.Sleep(200);

            //b.btnHome.Enabled = true;

            //  b.iconButton1.Enabled = true;
            //  iconButton1.Enabled = true;


            //b.iconButton2.Enabled = true;
            //b.iconButton3.Enabled = true;
            // g.iconButton4.Enabled = false;
            //b.iconButton5.Enabled = true;
            //b.iconButton6.Enabled = true;
            //b.iconButton4.Enabled = true;
        }
        private void DataReceivedHandler2(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            indata = indata.Replace(" ", "");
            indata = indata.Replace("\r", "");
            indata = indata.Replace("\n", "");
            //richTextBox_conStatus.AppendText("\nОтвет:"+indata+" \n");
            port.DataReceived -= DataReceivedHandler2;
        }
        private async void iconButton1_MouseDown(object sender, MouseEventArgs e)
        {
            if (NumbersInfoList[comboBox5.SelectedIndex].Interface == "GSM-шлюз") { GSM_gateway_flag = true; }
            else if (NumbersInfoList[comboBox5.SelectedIndex].Interface == "GSM") { GSM_modem_flag = true; }

            iconButton1.Enabled = false;
            b.btnHome.Enabled = false;
            b.iconButton1.Enabled = false;
            b.iconButton2.Enabled = false;
            b.iconButton3.Enabled = false;

            b.iconButton5.Enabled = false;
            b.iconButton6.Enabled = false;
            b.iconButton4.Enabled = false;

            sim_number = Convert.ToString(comboBox5.SelectedItem);
            try
            {

                richTextBox_conStatus.AppendText("\n----------------------------------------------------------------------------------------------------\nИНИЦИАЛИЗАЦИЯ СОЕДИНЕНИЯ\n----------------------------------------------------------------------------------------------------");

                port = new SerialPort();

                port.PortName = Convert.ToString(comboBox_comPorts.SelectedItem);

                port.BaudRate = 9600;

                port.ReadTimeout = 5000;

                port.Parity = Parity.None;
                port.DtrEnable = true;
                port.RtsEnable = true;

                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Handshake = Handshake.None;

                port.Open();

                port.DiscardInBuffer();

               richTextBox_conStatus.AppendText("\r\nВыполнить: ATZ");
                port.Write("ATZ" + "\r");
                Thread.Sleep(100);
                port.ReadLine();
              richTextBox_conStatus.AppendText("\nОтвет:" + port.ReadExisting());

                port.DiscardInBuffer();
               // port.DiscardOutBuffer();
               // port.BaseStream.Flush();

                richTextBox_conStatus.AppendText("\r\nВыполнить: ATE0");
                port.Write("ATE0" + "\r");
                Thread.Sleep(100);
                port.ReadLine();
                richTextBox_conStatus.AppendText("\nОтвет:" + port.ReadExisting());

                port.DiscardInBuffer();
                //port.DiscardOutBuffer();
               // port.BaseStream.Flush();

               richTextBox_conStatus.AppendText("\nВыполнить: AT+CBST=" + Convert.ToString(comboBox4.SelectedItem));
                port.Write("AT+CBST=" + Convert.ToString(comboBox4.SelectedItem) + "\r");
                Thread.Sleep(100);
                port.ReadLine();
                richTextBox_conStatus.AppendText("\nОтвет:" + port.ReadExisting());

                port.DiscardInBuffer();
                //port.DiscardOutBuffer();
                //port.BaseStream.Flush();

                //port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                richTextBox_conStatus.AppendText("\nВыполнить: ATD" + Convert.ToString(NumbersInfoList[comboBox5.SelectedIndex].number));
                port.Write("ATD" + Convert.ToString(NumbersInfoList[comboBox5.SelectedIndex].number) + "\r");
                sim_number = NumbersInfoList[comboBox5.SelectedIndex].number;
                //read_atd();
                await Task.Run(()=>read_atd());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void read_atd()
        {
            Thread.Sleep(500);
            List<byte> msg = new List<byte>();
            int i = 0;
            int count = 0;
            int lng = 0;
            while (lng < 5)
            {
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
                //Thread.Sleep(100);
            }

            byte[] g = new byte[msg.Count];
            for (int h = 0; h < msg.Count; h++)
            {
                g[h] = msg[h];
            }
            string str = Encoding.ASCII.GetString(g) ;
            

            if (str.Contains("CO") || str.Contains("ON"))
            {
                richTextBox_conStatus.AppendText("\nОтвет: CONNECT - соединение установлено.");

               
                b.iconButton1.Enabled = true;
                iconButton1.Enabled = true;
                
            }

            if (str.Contains("BU") || str.Contains("SY")||str.Contains("US"))
            {
                richTextBox_conStatus.AppendText("\nОтвет: BUSY - линия занята.");

                
               b.iconButton1.Enabled = true;
               iconButton1.Enabled = true;
                

                MessageBox.Show("Линия занята. Будет выполнен рестарт программы");
                Application.Restart();
            }

            if (str.Contains("NO") || str.Contains("CA") || str.Contains("AR"))
            {
                richTextBox_conStatus.AppendText("\nОтвет: NO CARRIER - потеря несущей.");

                
                b.iconButton1.Enabled = true;
                iconButton1.Enabled = true;
                

                MessageBox.Show("Потеря несущей. Будет выполнен рестарт программы");
                Application.Restart();
            }

        }
        private void iconButton2_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                richTextBox_conStatus.AppendText("\n----------------------------------------------------------------------------------------------------\nОТКЛЮЧЕНИЕ\n----------------------------------------------------------------------------------------------------");
                richTextBox_conStatus.ScrollToCaret();
                port.DiscardInBuffer();

                Thread.Sleep(1000);
                port.Write("+++");
                Thread.Sleep(2000);
                port.Write("ATH" + "\r");
                Thread.Sleep(200);

                port.Close();

               richTextBox_conStatus.AppendText("\nОтвет: Соединение разорвано.");
               richTextBox_conStatus.ScrollToCaret();

                MessageBox.Show("Соединение разорвано. Будет выполнен рестарт программы.");
                //Thread.Sleep(5000);
                Application.Restart();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Restart();
            }

        }

        private void comboBox5_Click(object sender, EventArgs e)
        {
            /*
            if (comboBox1.Text == "Интерфейс")
            {
                comboBox5.Enabled = false;
                MessageBox.Show("Выберите интерфейс.");
            }
            else 
            {
                comboBox5.ForeColor = Color.Black;
                comboBox5.Enabled = true;
            }*/
            comboBox5.ForeColor = Color.Black;
        }

        private void comboBox_comPorts_Click(object sender, EventArgs e)
        {
            comboBox_comPorts.ForeColor = Color.Black;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.ForeColor = Color.Black;
        }

        
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            /*
            comboBox5.Items.Clear();
            
            if (comboBox1.Text != "Интерфейс")
            {
                comboBox5.Enabled = true;

                string sql_cmd2 = "select distinct SIM from dbo.meter where Interface like '"+comboBox1.Text+"'";

                //чтение информации о номерах
                try
                {
                    if (!(connection.State == ConnectionState.Open))
                    {
                        connection.Open();
                    }
                    cmd = new SqlCommand(sql_cmd2, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            //string sim = Convert.ToString(reader.GetValue(0));
                            //comboBox5.Items.Add(sim);

                            NumbersInfoClass localClass = new NumbersInfoClass();

                            localClass.number = Convert.ToString(reader.GetValue(0));

                            NumbersInfoList.Add(localClass);
                        }
                    }
                    reader.Close();

                    for (int i = 0; i < NumbersInfoList.Count; i++)
                    {
                        if (!(connection.State == ConnectionState.Open))
                        {
                            connection.Open();
                        }
                        string local_sql_cmd = "select CompanyName from dbo.meter where SIM='" + NumbersInfoList[i].number + "'";
                        cmd = new SqlCommand(local_sql_cmd, connection);
                        reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                NumbersInfoList[i].company = Convert.ToString(reader.GetValue(0));
                            }
                        }
                        reader.Close();
                    }

                    //вывод общей информации о номере в comboBox5
                    for (int i = 0; i < NumbersInfoList.Count; i++)
                    {
                        string item = NumbersInfoList[i].company + " -> " + NumbersInfoList[i].number;
                        comboBox5.Items.Add(item);

                    }

                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }*/
        }

      
    }

    public class NumbersInfoClass
    {
        public string number;
        public string company;
        public string Interface;
    }
}   
