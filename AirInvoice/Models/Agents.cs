using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AirlineInvoice.Models;
namespace AirlineInvoice.Models
{
    public class Agents
    {
        
        private DBAccess db;
        //--------------------------------------------------------------------------
        public Agents()
        {
            db = new DBAccess();
        }
        //--------------------------------------------------------------------------
        ~Agents()
        {

        }
        //--------------------------------------------------------------------------
        public virtual void Dispose()
        {

        }
        //--------------------------------------------------------------------------
        public int AgentID { get; set; }
        public string AgentName { get; set; }
        public string AgentCompanyName { get; set; }
        public string AgentCompanyAddress { get; set; }
        public string AgentTaxCode { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string SigninSabre { get; set; }
        public string PasscodeSabre { get; set; }
        public string SuffixSabre { get; set; }
        public string HardcopyPrinterAddress { get; set; }
        public string TicketPrinterAddress { get; set; }
        public string TicketPrintRoutine { get; set; }
        public string StationNumber { get; set; }
        public string AgentLogo { get; set; }
        public int ParentID { get; set; }
        //--------------------------------------------------------------------------
        public List<Agents> init(SqlCommand cmd)
        {
            SqlConnection con = db.getConnection();
            cmd.Connection = con;
            List<Agents> l_Agents = new List<Agents>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                SmartDataReader smartReader = new SmartDataReader(reader);
                while (smartReader.Read())
                {
                    Agents m_Agents = new Agents();
                    m_Agents.AgentID = smartReader.GetInt32("AgentID");
                    m_Agents.AgentName = smartReader.GetString("AgentName");
                    m_Agents.AgentCompanyName = smartReader.GetString("AgentCompanyName");
                    m_Agents.AgentCompanyAddress = smartReader.GetString("AgentCompanyAddress");
                    m_Agents.AgentTaxCode = smartReader.GetString("AgentTaxCode");
                    m_Agents.Tel = smartReader.GetString("Tel");
                    m_Agents.Email = smartReader.GetString("Email");
                    m_Agents.IsActive = smartReader.GetBoolean("IsActive");
                    m_Agents.SigninSabre = smartReader.GetString("SigninSabre");
                    m_Agents.PasscodeSabre = smartReader.GetString("PasscodeSabre");
                    m_Agents.SuffixSabre = smartReader.GetString("SuffixSabre");
                    m_Agents.HardcopyPrinterAddress = smartReader.GetString("HardcopyPrinterAddress");
                    m_Agents.TicketPrinterAddress = smartReader.GetString("TicketPrinterAddress");
                    m_Agents.TicketPrintRoutine = smartReader.GetString("TicketPrintRoutine");
                    m_Agents.StationNumber = smartReader.GetString("StationNumber");
                    m_Agents.AgentLogo = smartReader.GetString("AgentLogo");
                    m_Agents.ParentID = smartReader.GetInt32("ParentID");
                    l_Agents.Add(m_Agents);
                }
                smartReader.disposeReader(reader);
                return l_Agents;
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
        public List<Agents> GetAll()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("AgentGetAll");
                cmd.CommandType = CommandType.StoredProcedure;
                List<Agents> lAr = init(cmd);
                return lAr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Agents GetByID(int ID)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("AgentGetByID");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AgentID", ID));
                List<Agents> lAr = init(cmd);
                if (lAr.Count == 1) return lAr[0] as Agents;
                else return new Agents();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}