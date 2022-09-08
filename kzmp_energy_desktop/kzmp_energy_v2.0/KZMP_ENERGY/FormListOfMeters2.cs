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
    public partial class FormListOfMeters2 : Form
    {
        //static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DBkzmp_energy;Integrated Security=True";
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        string sql_cmd = "SELECT * FROM dbo.meter";
        //SqlDataAdapter adapter;
        SqlCommand cmd;

        FormMainMenu g;

        BDmeter selectedRow = new BDmeter();
        
        public FormListOfMeters2(FormMainMenu f)
        {
            InitializeComponent();

           // g = f;
            


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
            DataColumn typeColumn = new DataColumn("Тип", Type.GetType("System.String"));
            DataColumn addressColumn = new DataColumn("Сетевой адрес", Type.GetType("System.Int32"));
            DataColumn simColumn = new DataColumn("Номер модема", Type.GetType("System.String"));
            DataColumn innColumn = new DataColumn("ИНН организации", Type.GetType("System.String"));
            DataColumn companyNameColumn = new DataColumn("Организация", Type.GetType("System.String"));
            DataColumn xml80020codeColumn = new DataColumn("XML80020 код", Type.GetType("System.String"));
            DataColumn INNcenter = new DataColumn("ИНН центра", Type.GetType("System.String"));
            DataColumn CenterName = new DataColumn("Центр услуг", Type.GetType("System.String"));
            DataColumn MeasuringpointName = new DataColumn("MeasuringpointName", Type.GetType("System.String"));
            DataColumn MeasuringchannelA = new DataColumn("MeasuringchannelA", Type.GetType("System.String"));
            DataColumn MeasuringchannelR = new DataColumn("MeasuringchannelR", Type.GetType("System.String"));
            DataColumn Transformation_ratio = new DataColumn("Transformation", Type.GetType("System.String"));
            DataColumn Interface = new DataColumn("Интерфейс", Type.GetType("System.String"));

            tbl.Columns.Add(idColumn);//0
            //tbl.Columns.Add(idMeterColumn);
            tbl.Columns.Add(typeColumn);    //1
            tbl.Columns.Add(addressColumn);//2
            tbl.Columns.Add(simColumn);//3
            tbl.Columns.Add(innColumn);//4
            tbl.Columns.Add(companyNameColumn);//5
            tbl.Columns.Add(INNcenter);//6
            tbl.Columns.Add(CenterName);//7
            tbl.Columns.Add(xml80020codeColumn);//8
            tbl.Columns.Add(MeasuringpointName);//9
            tbl.Columns.Add(MeasuringchannelA);//10
            tbl.Columns.Add(MeasuringchannelR);//11
            tbl.Columns.Add(Transformation_ratio);//12
            tbl.Columns.Add(Interface);//13
            

            tbl.PrimaryKey = new DataColumn[] { tbl.Columns["ID"] };


            dataGridView1.DataSource = tbl;

            dataGridView1.Columns[0].Width = 25;
            dataGridView1.Columns[1].Width = 110;
            dataGridView1.Columns[2].Width = 70;
            dataGridView1.Columns[3].Width = 100;
            dataGridView1.Columns[4].Width = 90;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[11].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[12].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[13].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

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
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            cmd = new SqlCommand(sql_cmd, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    BDmeter localMeter = new BDmeter();

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
                                               localMeter.MeasuringchannelA, localMeter.MeasuringchannelR,localMeter.Transformation_ratio, localMeter.Interface});
                }
                
            }
            reader.Close();

            int i = 0;
            while (i != 40)
            {
                tbl.Rows.Add();
                i++;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
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
                    selectedRow.INNcenter =row.Cells[6].Value.ToString();
                    selectedRow.CenterName= row.Cells[7].Value.ToString();
                    selectedRow.MeasuringpointName= row.Cells[9].Value.ToString();
                    selectedRow.MeasuringchannelA= row.Cells[10].Value.ToString();
                    selectedRow.MeasuringchannelR= row.Cells[11].Value.ToString();
                    selectedRow.Transformation_ratio = row.Cells[12].Value.ToString();
                    selectedRow.Interface = row.Cells[13].Value.ToString();
                }
            }
            catch (Exception ex) { }
        }
        //insert button
        private void iconButton1_Click(object sender, EventArgs e)
        {
            BDmeter insertMeterDate = new BDmeter();

            try
            {
                //insertMeterDate.type = textBox1.Text;
                insertMeterDate.type = comboBox2.Text;
                insertMeterDate.address = Convert.ToInt32(textBox2.Text);
                insertMeterDate.sim = textBox3.Text;
                insertMeterDate.inn = textBox4.Text;
                insertMeterDate.companyName = textBox5.Text;
                insertMeterDate.xml80020code = textBox6.Text;
                insertMeterDate.INNcenter = textBox8.Text;
                insertMeterDate.CenterName = textBox7.Text;
                insertMeterDate.MeasuringchannelA = textBox10.Text;
                insertMeterDate.MeasuringchannelR = textBox11.Text;
                insertMeterDate.MeasuringpointName = textBox9.Text;
                insertMeterDate.Transformation_ratio = textBox12.Text;
                insertMeterDate.Interface = comboBox1.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            string insert_sql_cmd = "insert into dbo.meter values('" + insertMeterDate.type + "'," + insertMeterDate.address +
                ",'" + insertMeterDate.sim + "','" + insertMeterDate.inn + "','" + insertMeterDate.companyName + "','" + insertMeterDate.INNcenter + "','"
                + insertMeterDate.CenterName + "','" + insertMeterDate.MeasuringpointName + "','" + insertMeterDate.MeasuringchannelA + "','"
                + insertMeterDate.MeasuringchannelR + "','" + insertMeterDate.xml80020code + "','"+insertMeterDate.Transformation_ratio+"','"+insertMeterDate.Interface+"');";

            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            cmd = new SqlCommand(insert_sql_cmd, connection);
            try
            {
                cmd.ExecuteNonQuery();
                DownBD();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //update button
        private void iconButton2_Click(object sender, EventArgs e)
        {
            string sql_cmd_id = "select id_meter from dbo.meter where address="+selectedRow.address + "and SIM = "+selectedRow.sim;
            int id_meter = 0;
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            cmd = new SqlCommand(sql_cmd_id, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    id_meter = Convert.ToInt32(reader.GetValue(0));
                }
            }
            reader.Close();

            BDmeter updateMeter = new BDmeter();

            updateMeter.id_meter = id_meter;
            //updateMeter.type = textBox1.Text;
            updateMeter.type = comboBox2.Text;
            updateMeter.address = Convert.ToInt32(textBox2.Text);
            updateMeter.sim = textBox3.Text;
            updateMeter.inn = textBox4.Text;
            updateMeter.companyName = textBox5.Text;
            updateMeter.xml80020code = textBox6.Text;
            updateMeter.CenterName = textBox7.Text;
            updateMeter.INNcenter = textBox8.Text;
            updateMeter.MeasuringpointName = textBox9.Text;
            updateMeter.MeasuringchannelA = textBox10.Text;
            updateMeter.MeasuringchannelR = textBox11.Text;
            updateMeter.Transformation_ratio = textBox12.Text;
            updateMeter.Interface = comboBox1.Text;

            try
            {
                string update_sql_cmd = "update dbo.meter set type='" + updateMeter.type + "', address=" + Convert.ToString(updateMeter.address) + ",SIM='" +
                    updateMeter.sim + "',INN='" + updateMeter.inn + "',CompanyName='" + updateMeter.companyName + "',INNcenter='" + updateMeter.INNcenter + 
                    "', CenterName = '"+updateMeter.CenterName+"', MeasuringpointName = '"+updateMeter.MeasuringpointName+"', MeasuringchannelA = '"+
                    updateMeter.MeasuringchannelA + "', MeasuringchannelR = '"+ updateMeter.MeasuringchannelR+"', XML80020code = '"+updateMeter.xml80020code+"', Transformation_ratio = '"+updateMeter.Transformation_ratio+"', Interface = '"+updateMeter.Interface+"'"+
                    "where id_meter=" + Convert.ToString(updateMeter.id_meter) + ";";

                cmd = new SqlCommand(update_sql_cmd, connection);
                cmd.ExecuteNonQuery();
                DownBD();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //delete button
        private void iconButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            string delete_sql_cmd = "delete from dbo.meter where address=" + textBox2.Text+"and SIM='"+textBox3.Text+"'";
            cmd = new SqlCommand(delete_sql_cmd, connection);
            try
            {
                cmd.ExecuteNonQuery();
                DownBD();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       

    }

    class BDmeter
    {
        public int id_meter;
        public string type;
        public int address;
        public string sim;
        public string inn;
        public string companyName;
        public string xml80020code;
        public string INNcenter;
        public string  CenterName;
        public string MeasuringpointName ;
        public string MeasuringchannelA ;
        public string MeasuringchannelR ;
        public string Transformation_ratio;
        public string Interface;
    }
}
