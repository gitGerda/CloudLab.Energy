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

namespace KZMP_ENERGY
{
    public partial class FormListOfMeters : Form
    {
        static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DBkzmp_energy;Integrated Security=True";
        SqlConnection connection = new SqlConnection(connectionString);
        string sql = "SELECT * FROM dbo.meter";
        public FormListOfMeters()
        {
            InitializeComponent();
            //dataGridView1 some parameters
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            //create dataset and datable for datagridview1 to upload data from DB
            DataSet meters = new DataSet("meters");
            DataTable meters_table = new DataTable("meters_table");
            DataTable transit = new DataTable("transit");
            meters.Tables.Add(meters_table);
            meters.Tables.Add(transit);

            //create columns for meters_table
            DataColumn id_column = new DataColumn("ID", Type.GetType("System.Int32"));
            id_column.Unique = true; // столбец будет иметь уникальное значение
            id_column.AllowDBNull = false; // не может принимать null
            id_column.AutoIncrement = true; // будет автоинкрементироваться
            id_column.AutoIncrementSeed = 1; // начальное значение
            id_column.AutoIncrementStep = 1; // приращении при добавлении новой строки

            DataColumn type_column = new DataColumn("Тип счетчика", Type.GetType("System.String"));
            type_column.ReadOnly = true;

            DataColumn address_column = new DataColumn("Сетевой адрес", Type.GetType("System.Int32"));
            address_column.ReadOnly = true;

            DataColumn desc_column = new DataColumn("Описание счетчика", Type.GetType("System.String"));
            desc_column.ReadOnly = true;

            meters_table.Columns.Add(id_column);
            meters_table.Columns.Add(type_column);
            meters_table.Columns.Add(address_column);
            meters_table.Columns.Add(desc_column);

            meters_table.PrimaryKey = new DataColumn[] { meters_table.Columns["ID"]};
            dataGridView1.DataSource = meters.Tables[0];
            dataGridView1.Columns["Id"].ReadOnly = true;

            //устанавливаю размеры колонок в dataGridView1
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = 200;
           // dataGridView1.Columns[2].Width = 100;
            //dataGridView1.Columns[3].Width = 2000;*/
            //dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // dataGridView1.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 14);
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            //dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 10);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(91,202,113);
            //dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.RowsDefaultCellStyle.Font= new Font("MS Reference Sans Serif", 10);

            try
            {
                // Открываем подключение
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                // Заполняем Dataset
                adapter.Fill(meters.Tables[1]);
                DataTable dt = meters.Tables[1];
                foreach (DataRow rows in dt.Rows)
                {
                    var cells = rows.ItemArray;
                    int id = Convert.ToInt32(cells[0]);
                    string type = Convert.ToString(cells[1]);
                    int address = Convert.ToInt32(cells[2]);
                    string desc = Convert.ToString(cells[3]);
                    meters_table.Rows.Add(new object[] { null, type, address, desc });
                }
                for (int i = 0; i < 50; i++)
                {
                    meters_table.Rows.Add();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally 
            {
                connection.Close();

            }
        }
    }
}
