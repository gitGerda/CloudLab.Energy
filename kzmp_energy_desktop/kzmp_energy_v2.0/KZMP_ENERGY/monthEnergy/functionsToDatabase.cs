using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace KZMP_ENERGY.monthEnergy
{
    public static class functionsToDatabase
    {
        public static bool checkExistence(ref SqlConnection connection, string meterId, string year, string month)
        {
            checkConnection(connection);

            string sql_cmd = "select count(*) " +
                             "from dbo.EnergyTable " +
                             "where MeterID = " + meterId + " and year= " + year + " and Month='" + month + "';";
            SqlCommand cmd = new SqlCommand(sql_cmd, connection);
            int linesCount = Convert.ToInt32(cmd.ExecuteScalar());

            if(linesCount==0)
            { 
                return false;
            }
            else
            {
                sql_cmd = "update dbo.EnergyTable " +
                          "set StartValue='NotDefined', EndValue='NotDefined', Total='NotDefined', Date='NotDefined' " +
                          "where MeterID = " + meterId + " and year= " + year + " and Month='" + month + "';";
                cmd = new SqlCommand(sql_cmd, connection);
                checkConnection(connection);
                cmd.ExecuteNonQuery();

                return true;
            }
        }

        public static void insertFunc(ref SqlConnection connection, string meterId, string year, string month,string StartValue,string EndValue)
        {
            checkConnection(connection);

            //string total = Convert.ToString(Convert.ToSingle(EndValue)-Convert.ToSingle(StartValue));

            string sql_cmd = "insert into dbo.EnergyTable(MeterID,Year,Month,StartValue,EndValue,Total,Date) " +
                             "values (" + meterId + ", " + year + ", '" + month + "', '" + StartValue + "', '" + EndValue + "', '0', '"+DateTime.Now.ToString()+"');";
            SqlCommand cmd = new SqlCommand(sql_cmd, connection);
            cmd.ExecuteNonQuery();
        }

        public static void updateFunc(ref SqlConnection connection, string meterId, string year, string month, string StartValue, string EndValue, bool startValueFlag)
        {
            checkConnection(connection);

            //string total = Convert.ToString(Convert.ToSingle(EndValue) - Convert.ToSingle(StartValue));
            string sql_cmd = "";

            if (startValueFlag)
            {
                sql_cmd = "update dbo.EnergyTable " +
                                 "set StartValue='" + StartValue + "' , Total='0', Date='" +
                                 DateTime.Now.ToString() + "' " +
                                 "where MeterID=" + meterId + " and Year=" + year + " and Month='" + month + "';";
            }
            else 
            {
                sql_cmd = "update dbo.EnergyTable " +
                                 "set EndValue='" + EndValue + "' , Total='0', Date='" +
                                 DateTime.Now.ToString() + "' " +
                                 "where MeterID=" + meterId + " and Year=" + year + " and Month='" + month + "';";
            }

            SqlCommand cmd = new SqlCommand(sql_cmd, connection);
            cmd.ExecuteNonQuery();
        }

        static void checkConnection(SqlConnection connection)
        {
            if (!(connection.State == ConnectionState.Open))
            {
                connection.Open();
            }
        }

        public static void calculateTotal(ref SqlConnection connection, string meterId, string year, string month, string total)
        {
            checkConnection(connection);

            string sql_cmd = "update dbo.EnergyTable " +
                             "set Total='"+total+"' " +
                             "where MeterID=" + meterId + " and Year=" + year + " and Month='" + month + "';";
            SqlCommand cmd = new SqlCommand(sql_cmd, connection);
            cmd.ExecuteNonQuery();
        }
    }
}
