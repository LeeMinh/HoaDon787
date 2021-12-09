using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class Logs
    {

        private DBAccess db;
        //--------------------------------------------------------------------------
        public Logs()
        {
            db = new DBAccess();
        }
        //--------------------------------------------------------------------------
        ~Logs()
        {

        }
        //--------------------------------------------------------------------------
        public virtual void Dispose()
        {

        }
        //--------------------------------------------------------------------------
        public int LogID { get; set; }
        public int InvoiceID { get; set; }
        public string Log { get; set; }
        
        //--------------------------------------------------------------------------
        public List<Logs> init(SqlCommand cmd)
        {
            SqlConnection con = db.getConnection();
            cmd.Connection = con;
            List<Logs> l_Logs = new List<Logs>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                SmartDataReader smartReader = new SmartDataReader(reader);
                while (smartReader.Read())
                {
                    Logs m_Logs = new Logs();
                    m_Logs.LogID = smartReader.GetInt32("LogID");
                    m_Logs.InvoiceID = smartReader.GetInt32("InvoiceID");
                    m_Logs.Log = smartReader.GetString("Log");                    
                    l_Logs.Add(m_Logs);
                }
                smartReader.disposeReader(reader);
                return l_Logs;
            }
            catch (SqlException err)
            {
                throw err;
            }
            finally
            {
                db.closeConnection(con);
            }
        }
        //--------------------------------------------------------------------------
        public void Insert()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("LogsInsert");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", this.InvoiceID));
                cmd.Parameters.Add(new SqlParameter("@Log", this.Log));
                cmd.Parameters.Add("@LogID", SqlDbType.Int).Direction = ParameterDirection.Output;
                db.ExecuteSQL(cmd);
                this.LogID = (cmd.Parameters["@LogID"].Value == null) ? 0 : Convert.ToInt32(cmd.Parameters["@LogID"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //--------------------------------------------------------------------------
        public void Update()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("LogsUpdate");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", this.InvoiceID));
                cmd.Parameters.Add(new SqlParameter("@Log", this.Log));
                cmd.Parameters.Add(new SqlParameter("@LogID", this.LogID));
                db.ExecuteSQL(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        //---------------------------------------------------------------------------------------------
        public Logs GetByInvoiceID(int id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("LogsGetByInvoiceID");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", id));
                List<Logs> lAr = init(cmd);
                if (lAr.Count == 1) return lAr[0] as Logs;
                else return new Logs();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}