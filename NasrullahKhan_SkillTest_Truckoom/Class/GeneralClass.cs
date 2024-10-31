using System.Data;
using System.Data.SqlClient;

namespace NasrullahKhan_SkillTest_PenaltyCalculation.Class
{
    public class GeneralClass
    {
        private static string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=TMS;Integrated Security=True;Connect Timeout=0;";
        private SqlConnection sqlConn = new SqlConnection(connectionString);
        private static readonly int commandTimeout = 30;


        public DataTable GetDataTable(SqlCommand Comm)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                Comm.Connection = conn;
                Comm.CommandTimeout = commandTimeout;

                using (SqlDataAdapter sqlAdpt = new SqlDataAdapter(Comm))
                {
                    sqlAdpt.Fill(dt);
                }
            }
            return dt;
        }

        public bool ExecuteNonQuery(SqlCommand Comm)
        {
            bool isExecuted = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                Comm.Connection = conn;
                Comm.CommandTimeout = commandTimeout;
                Comm.ExecuteNonQuery();
                //** Comm.Dispose(); //**
                isExecuted = true;
            }
            return isExecuted;
        }
    }
}
