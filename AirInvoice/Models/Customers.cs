using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class Customers
    {

        private DBAccess db;
        //--------------------------------------------------------------------------
        public Customers()
        {
            db = new DBAccess();
        }
        //--------------------------------------------------------------------------
        ~Customers()
        {

        }
        //--------------------------------------------------------------------------
        public virtual void Dispose()
        {

        }
        //--------------------------------------------------------------------------
        public int CustomerID { get; set; }
        public int AgentID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTax { get; set; }
        public string CustomerAddress { get; set; }
        //--------------------------------------------------------------------------
        public List<Customers> init(SqlCommand cmd)
        {
            SqlConnection con = db.getConnection();
            cmd.Connection = con;
            List<Customers> l_Customers = new List<Customers>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                SmartDataReader smartReader = new SmartDataReader(reader);
                while (smartReader.Read())
                {
                    Customers m_Customers = new Customers();
                    m_Customers.CustomerID = smartReader.GetInt32("CustomerID");
                    m_Customers.AgentID = smartReader.GetInt32("AgentID");
                    m_Customers.CustomerName = smartReader.GetString("CustomerName");
                    m_Customers.CustomerTax = smartReader.GetString("CustomerTax");
                    m_Customers.CustomerAddress = smartReader.GetString("CustomerAddress");
                    l_Customers.Add(m_Customers);
                }
                smartReader.disposeReader(reader);
                return l_Customers;
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
                SqlCommand cmd = new SqlCommand("CustomersInsert");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AgentID", this.AgentID));
                cmd.Parameters.Add(new SqlParameter("@CustomerName", this.CustomerName));
                cmd.Parameters.Add(new SqlParameter("@CustomerTax", this.CustomerTax));
                cmd.Parameters.Add(new SqlParameter("@CustomerAddress", this.CustomerAddress));
                cmd.Parameters.Add("@CustomerID", SqlDbType.Int).Direction = ParameterDirection.Output;
                db.ExecuteSQL(cmd);
                this.CustomerID = (cmd.Parameters["@CustomerID"].Value == null) ? 0 : Convert.ToInt32(cmd.Parameters["@CustomerID"].Value);
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
                SqlCommand cmd = new SqlCommand("CustomersUpdate");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AgentID", this.AgentID));
                cmd.Parameters.Add(new SqlParameter("@CustomerName", this.CustomerName));
                cmd.Parameters.Add(new SqlParameter("@CustomerTax", this.CustomerTax));
                cmd.Parameters.Add(new SqlParameter("@CustomerAddress", this.CustomerAddress));
                cmd.Parameters.Add(new SqlParameter("@CustomerID", this.CustomerID));
                db.ExecuteSQL(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //--------------------------------------------------------------------------
        public void Delete()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("CustomersDelete");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CustomerID", this.CustomerID));
                db.ExecuteSQL(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //---------------------------------------------------------------------------------------------
        public List<Customers> GetByAgentID(int id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("CustomersGetByAgentID");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AgentID", id));
                List<Customers> lAr = init(cmd);
                return lAr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Customers GetByCustomerID(int id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("CustomersGetByCustomerID");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CustomerID", id));
                List<Customers> lAr = init(cmd);
                if (lAr.Count == 1) return lAr[0] as Customers;
                else return new Customers();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}