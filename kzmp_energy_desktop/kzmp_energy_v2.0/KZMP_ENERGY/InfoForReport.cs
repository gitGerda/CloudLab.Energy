using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;

namespace KZMP_ENERGY
{
    public partial class InfoForReport : Form
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        string sql_cmd = "SELECT * FROM dbo.msg_number";
        SqlCommand cmd;

        BDInfoReport selectedRow = new BDInfoReport();
        public InfoForReport()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            DownBD();
        }

        private void DownBD()
        {
            DataTable tbl = new DataTable();

            DataColumn idColumn = new DataColumn("ID", Type.GetType("System.Int32"));
            idColumn.Unique = true; // столбец будет иметь уникальное значение
            idColumn.AllowDBNull = false; // не может принимать null
            idColumn.AutoIncrement = true; // будет автоинкрементироваться
            idColumn.AutoIncrementSeed = 1; // начальное значение
            idColumn.AutoIncrementStep = 1; // приращении при добавлении новой строки

            //DataColumn idMeterColumn = new DataColumn("ID счётчика", Type.GetType("System.Int32"));
            DataColumn CompanyINN = new DataColumn("ИНН организации", Type.GetType("System.String"));
            DataColumn CompanyName = new DataColumn("Организация", Type.GetType("System.String"));
            DataColumn Contract = new DataColumn("Номер договора", Type.GetType("System.String"));
            DataColumn MsgNumber = new DataColumn("MsgNumber", Type.GetType("System.String"));
            DataColumn Date = new DataColumn("Дата", Type.GetType("System.String"));


            tbl.Columns.Add(idColumn);//0
            //tbl.Columns.Add(idMeterColumn);
            tbl.Columns.Add(CompanyINN);    //1
            tbl.Columns.Add(CompanyName);//2
            tbl.Columns.Add(Contract);//3
            tbl.Columns.Add(MsgNumber);//4
            tbl.Columns.Add(Date);//5

            tbl.PrimaryKey = new DataColumn[] { tbl.Columns["ID"] };


            dataGridView1.DataSource = tbl;

            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 8);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(91, 202, 113);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.RowsDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 8);

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
                        /*BDmeter localMeter = new BDmeter();

                        localMeter.id_meter = Convert.ToInt32(reader.GetValue(0));
                        localMeter.type = Convert.ToString(reader.GetValue(1));
                        localMeter.address = Convert.ToInt32(reader.GetValue(2));
                        localMeter.sim = Convert.ToString(reader.GetValue(3));
                        localMeter.inn = Convert.ToString(reader.GetValue(4));
                        localMeter.companyName = Convert.ToString(reader.GetValue(5));
                        localMeter.INNcenter = Convert.ToString(reader.GetValue(6));
                        localMeter.CenterName = Convert.ToString(reader.GetValue(7));
                        localMeter.MeasuringpointName = Convert.ToString(reader.GetValue(8));
                        localMeter.MeasuringchannelA = Convert.ToString(reader.GetValue(9));
                        localMeter.MeasuringchannelR = Convert.ToString(reader.GetValue(10));
                        localMeter.xml80020code = Convert.ToString(reader.GetValue(11));
                        localMeter.Transformation_ratio = Convert.ToString(reader.GetValue(12));
                        localMeter.Interface = Convert.ToString(reader.GetValue(13));

                        tbl.Rows.Add(new object[] {null,localMeter.type,localMeter.address,localMeter.sim,localMeter.inn,localMeter.companyName,
                                                   localMeter.INNcenter, localMeter.CenterName, localMeter.xml80020code, localMeter.MeasuringpointName,
                                                   localMeter.MeasuringchannelA, localMeter.MeasuringchannelR,localMeter.Transformation_ratio, localMeter.Interface});*/
                        BDInfoReport localClass = new BDInfoReport();
                        // localClass.id = Convert.ToString(reader.GetValue(0));
                        localClass.inn = Convert.ToString(reader.GetValue(0));
                        localClass.name = Convert.ToString(reader.GetValue(1));
                        localClass.contract = Convert.ToString(reader.GetValue(2));
                        localClass.msg_number = Convert.ToString(reader.GetValue(3));
                        localClass.date = Convert.ToString(reader.GetValue(4));

                        tbl.Rows.Add(new object[] { null, localClass.inn, localClass.name, localClass.contract, localClass.msg_number, localClass.date });
                    }

                }
                reader.Close();

                int i = 0;
                while (i != 10)
                {
                    tbl.Rows.Add();
                    i++;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    /*
                    //textBox1.Text = row.Cells[1].Value.ToString();
                    comboBox2.Text = row.Cells[1].Value.ToString();
                    textBox2.Text = row.Cells[2].Value.ToString();
                    textBox3.Text = row.Cells[3].Value.ToString();
                    textBox4.Text = row.Cells[4].Value.ToString();
                    textBox5.Text = row.Cells[5].Value.ToString();
                    textBox8.Text = row.Cells[6].Value.ToString();
                    textBox7.Text = row.Cells[7].Value.ToString();
                    textBox6.Text = row.Cells[8].Value.ToString();
                    textBox9.Text = row.Cells[9].Value.ToString();
                    textBox10.Text = row.Cells[10].Value.ToString();
                    textBox11.Text = row.Cells[11].Value.ToString();
                    textBox12.Text = row.Cells[12].Value.ToString();
                    comboBox1.Text = row.Cells[13].Value.ToString();


                    selectedRow.type = row.Cells[1].Value.ToString();
                    selectedRow.address = Convert.ToInt32(row.Cells[2].Value);
                    selectedRow.sim = row.Cells[3].Value.ToString();
                    selectedRow.inn = row.Cells[4].Value.ToString();
                    selectedRow.companyName = row.Cells[5].Value.ToString();
                    selectedRow.xml80020code = row.Cells[8].Value.ToString();
                    selectedRow.INNcenter = row.Cells[6].Value.ToString();
                    selectedRow.CenterName = row.Cells[7].Value.ToString();
                    selectedRow.MeasuringpointName = row.Cells[9].Value.ToString();
                    selectedRow.MeasuringchannelA = row.Cells[10].Value.ToString();
                    selectedRow.MeasuringchannelR = row.Cells[11].Value.ToString();
                    selectedRow.Transformation_ratio = row.Cells[12].Value.ToString();
                    selectedRow.Interface = row.Cells[13].Value.ToString();*/

                    textBox4.Text = row.Cells[1].Value.ToString();
                    textBox5.Text = row.Cells[2].Value.ToString();
                    textBox7.Text= row.Cells[3].Value.ToString();
                    textBox8.Text = row.Cells[4].Value.ToString();
                    var g = DateTime.Parse(row.Cells[5].Value.ToString());
                    datePickerStart.Value = g;

                    selectedRow.inn = row.Cells[1].Value.ToString();
                    selectedRow.name= row.Cells[2].Value.ToString();
                    selectedRow.contract= row.Cells[3].Value.ToString();
                    selectedRow.msg_number = row.Cells[4].Value.ToString();
                    selectedRow.date = datePickerStart.Value.ToString();
                }
            }
            catch (Exception ex) { }
        }

        //insert 
        private void iconButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }

                string local_date = datePickerStart.Value.ToString();
                local_date = local_date.Substring(0, 8);

                string sql_cmd = "insert into dbo.msg_number values('" + textBox4.Text + "','" + textBox5.Text + "','" + textBox7.Text + "','" + textBox8.Text + "','" + local_date + "');";
                cmd = new SqlCommand(sql_cmd, connection);
                cmd.ExecuteNonQuery();
                DownBD();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //update
        private void iconButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }

                string local_date = datePickerStart.Value.ToString();
                local_date = local_date.Substring(0, 8);

                string sql_cmd = "update dbo.msg_number set CompanyINN ='" + textBox4.Text + "', CompanyName = '" + textBox5.Text + "', Contract = '" + textBox7.Text + "', MsgNumber = '" + textBox8.Text + "', Date = '" + local_date + "' where CompanyINN = '" + selectedRow.inn + "';";
                cmd = new SqlCommand(sql_cmd, connection);
                cmd.ExecuteNonQuery();
                DownBD();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //delete
        private void iconButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
                string sql_cmd = "delete from dbo.msg_number where CompanyINN='" + selectedRow.inn + "';";
                cmd = new SqlCommand(sql_cmd, connection);
                cmd.ExecuteNonQuery();
                DownBD();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    class BDInfoReport
    {
        //public string id;
        public string inn;
        public string name;
        public string contract;
        public string msg_number;
        public string date;
    }
}
