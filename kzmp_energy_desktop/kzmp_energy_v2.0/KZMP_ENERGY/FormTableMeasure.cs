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
using System.Configuration;

namespace KZMP_ENERGY
{
    public partial class FormTableMeasure : Form
    {
        //static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DBkzmp_energy;Integrated Security=True";
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        

        SqlCommand cmd;

        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;

        DataTable tbl;

        List<MetersInfoClass> MetersInfoList = new List<MetersInfoClass>();
        public FormTableMeasure()
        {
            InitializeComponent();

            
            tbl = new DataTable();

            DataColumn idColumn = new DataColumn("ID", Type.GetType("System.Int32"));
            idColumn.Unique = true; // столбец будет иметь уникальное значение
            idColumn.AllowDBNull = false; // не может принимать null
            idColumn.AutoIncrement = true; // будет автоинкрементироваться
            idColumn.AutoIncrementSeed = 1; // начальное значение
            idColumn.AutoIncrementStep = 1; // приращении при добавлении новой строки

            DataColumn address = new DataColumn("Сетевой адрес", Type.GetType("System.Int32"));
            DataColumn date = new DataColumn("Дата", Type.GetType("System.String"));
            DataColumn time = new DataColumn("Время", Type.GetType("System.String"));
            DataColumn Pplus = new DataColumn("P+", Type.GetType("System.String"));
            DataColumn Pminus = new DataColumn("P-", Type.GetType("System.String"));
            DataColumn Qplus = new DataColumn("Q+", Type.GetType("System.String"));
            DataColumn Qminus = new DataColumn("Q-", Type.GetType("System.String"));

            tbl.Columns.Add(idColumn);
            tbl.Columns.Add(address);
            tbl.Columns.Add(date);
            tbl.Columns.Add(time);
            tbl.Columns.Add(Pplus);
            tbl.Columns.Add(Pminus);
            tbl.Columns.Add(Qplus);
            tbl.Columns.Add(Qminus);

            tbl.PrimaryKey = new DataColumn[] { tbl.Columns["ID"] };

            dataGridView1.DataSource = tbl;

            /*
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Width = 100;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Width = 140;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;*/

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 8);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(91, 202, 113);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.RowsDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 8);

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            //FillDataGridView();

            //загрузка информации о счетчиках из dbo.meter
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }

                string local_cmd = "select id_meter, address, SIM, CompanyName from dbo.meter;";
                cmd = new SqlCommand(local_cmd, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MetersInfoClass local_class = new MetersInfoClass();

                        local_class.id_meter = Convert.ToString(reader.GetValue(0));
                        local_class.address = Convert.ToString(reader.GetValue(1));
                        local_class.SIM = Convert.ToString(reader.GetValue(2));
                        local_class.CompanyName = Convert.ToString(reader.GetValue(3));

                        MetersInfoList.Add(local_class);
                    }
                }
                reader.Close();

                //заполнение comboBox1
                for (int i = 0; i < MetersInfoList.Count; i++)
                {
                    string str = MetersInfoList[i].SIM + "->" + MetersInfoList[i].CompanyName + "->" + MetersInfoList[i].address;
                    comboBox1.Items.Add(str);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillDataGridView()
        {
            tbl.Clear();

            string start = datePickerStart.Value.ToString();
            string end = datePickerEnd.Value.ToString();

            string sql = "SELECT * FROM dbo.power_profile_m where id ="+MetersInfoList[comboBox1.SelectedIndex].id_meter+" and date>='"+start+"' and date<='"+end+"'";
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
            cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Measures measuresData = new Measures();

                        measuresData.address = Convert.ToInt32(reader.GetValue(1));
                        measuresData.date = Convert.ToString(reader.GetValue(2));
                        measuresData.time = Convert.ToString(reader.GetValue(3));
                        measuresData.Pplus = Convert.ToString(reader.GetValue(4));
                        measuresData.Pminus = Convert.ToString(reader.GetValue(5));
                        measuresData.Qplus = Convert.ToString(reader.GetValue(6));
                        measuresData.Qminus = Convert.ToString(reader.GetValue(7));

                        tbl.Rows.Add(new object[] {null,measuresData.address,measuresData.date,measuresData.time,measuresData.Pplus,measuresData.Pminus,
                    measuresData.Qplus,measuresData.Qminus});
                    }
                    
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            FillDataGridView();
        }
    }
    public class Measures
    {
        public int id_meter;
        public int address;
        public string date;
        public string time;
        public string Pplus;
        public string Pminus;
        public string Qplus;
        public string Qminus;
    }

    public class MetersInfoClass
    {
        public string id_meter;
        public string address;
        public string CompanyName;
        public string SIM;
    }
}
