using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class Buyers
    {

        private DBAccess db;
        //--------------------------------------------------------------------------
        public Buyers()
        {
            db = new DBAccess();
        }
        //--------------------------------------------------------------------------
        ~Buyers()
        {

        }
        //--------------------------------------------------------------------------
        public virtual void Dispose()
        {

        }
        //--------------------------------------------------------------------------
        public int InvoiceID { get; set; }
        public string Buyer { get; set; }
        
        //--------------------------------------------------------------------------
        public List<Buyers> init(SqlCommand cmd)
        {
            SqlConnection con = db.getConnection();
            cmd.Connection = con;
            List<Buyers> l_Buyers = new List<Buyers>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                SmartDataReader smartReader = new SmartDataReader(reader);
                while (smartReader.Read())
                {
                    Buyers m_Buyers = new Buyers();
                    m_Buyers.InvoiceID = smartReader.GetInt32("InvoiceID");
                    m_Buyers.Buyer = smartReader.GetString("Buyer");                    
                    l_Buyers.Add(m_Buyers);
                }
                smartReader.disposeReader(reader);
                return l_Buyers;
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
                SqlCommand cmd = new SqlCommand("BuyersInsert");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", this.InvoiceID));
                cmd.Parameters.Add(new SqlParameter("@Buyer", this.Buyer));                
                db.ExecuteSQL(cmd);                
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
                SqlCommand cmd = new SqlCommand("BuyersUpdate");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", this.InvoiceID));
                cmd.Parameters.Add(new SqlParameter("@Buyer", this.Buyer));
                db.ExecuteSQL(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //---------------------------------------------------------------------------------------------
        
        public Buyers GetByInvoiceID(int ID)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("BuyersGetByInvoiceID");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", ID));
                List<Buyers> lAr = init(cmd);
                if (lAr.Count == 1) return lAr[0] as Buyers;
                else return new Buyers();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}