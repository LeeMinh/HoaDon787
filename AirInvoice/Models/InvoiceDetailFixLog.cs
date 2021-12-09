using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class InvoiceDetailFixLog
    {

        private DBAccess db;
        //--------------------------------------------------------------------------
        public InvoiceDetailFixLog()
        {
            db = new DBAccess();
        }
        //--------------------------------------------------------------------------
        ~InvoiceDetailFixLog()
        {

        }
        //--------------------------------------------------------------------------
        public virtual void Dispose()
        {

        }
        //--------------------------------------------------------------------------
        public int InvoiceDetailID { get; set; }
        public string TicketNo { get; set; }
        public int TicketType { get; set; }
        public string TripCode { get; set; }
        public int TicketFare { get; set; }
        public int TicketVAT { get; set; }
        public int TicketTax { get; set; }
        public int TicketTaxGlobal { get; set; }
        public int RealPay { get; set; }
        public int TicketPrice { get; set; }
        public string VCRDisplay { get; set; }
        public bool? UpdateSystem { get; set; }
        public string Airline { get; set; }
        public int InvoiceID { get; set; }
        public string PNRCode { get; set; }
        public string PaxName { get; set; }
        public int AgentID { get; set; }
        public int AgentBranchID { get; set; }
        public DateTime UpdateTime { get; set; }
        public int OtherFees { get; set; }
        public int ReturnFees { get; set; }
        public string ChangeTicket { get; set; }
        public int ChangeLevelFee { get; set; }
        public int ChangeLevelFeeVAT { get; set; }
        public string Note { get; set; }
        public int AgentFee { get; set; }
        public int AgentFeeVAT { get; set; }
        public DateTime SoldDate { get; set; }
        public int TicketFareNew { get; set; }
        public int TicketVATNew { get; set; }
        public int TicketTaxNew { get; set; }
        public int TicketTaxGlobalNew { get; set; }
        public int TicketPriceNew { get; set; }
        public int OtherFeesNew { get; set; }
        public int ReturnFeesNew { get; set; }
        public int ChangeLevelFeeNew { get; set; }
        public int ChangeLevelFeeVATNew { get; set; }
        public string TripCodeNew { get; set; }
        public string FixUser { get; set; }
        public DateTime FixDatetime { get; set; }

        //--------------------------------------------------------------------------
        public List<InvoiceDetailFixLog> init(SqlCommand cmd)
        {
            SqlConnection con = db.getConnection();
            cmd.Connection = con;
            List<InvoiceDetailFixLog> l_InvoiceDetailFixLog = new List<InvoiceDetailFixLog>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                SmartDataReader smartReader = new SmartDataReader(reader);
                while (smartReader.Read())
                {
                    InvoiceDetailFixLog m_InvoiceDetailFixLog = new InvoiceDetailFixLog();
                    m_InvoiceDetailFixLog.InvoiceDetailID = smartReader.GetInt32("InvoiceDetailID");
                    m_InvoiceDetailFixLog.TicketNo = smartReader.GetString("TicketNo");
                    m_InvoiceDetailFixLog.TicketType = smartReader.GetInt32("TicketType");
                    m_InvoiceDetailFixLog.TripCode = smartReader.GetString("TripCode");
                    m_InvoiceDetailFixLog.TicketFare = smartReader.GetInt32("TicketFare");
                    m_InvoiceDetailFixLog.TicketVAT = smartReader.GetInt32("TicketVAT");
                    m_InvoiceDetailFixLog.TicketTax = smartReader.GetInt32("TicketTax");
                    m_InvoiceDetailFixLog.TicketTaxGlobal = smartReader.GetInt32("TicketTaxGlobal");
                    m_InvoiceDetailFixLog.RealPay = smartReader.GetInt32("RealPay");
                    m_InvoiceDetailFixLog.TicketPrice = smartReader.GetInt32("TicketPrice");
                    m_InvoiceDetailFixLog.VCRDisplay = smartReader.GetString("VCRDisplay");
                    m_InvoiceDetailFixLog.UpdateSystem = smartReader.GetBoolean("UpdateSystem");
                    m_InvoiceDetailFixLog.Airline = smartReader.GetString("Airline");
                    m_InvoiceDetailFixLog.InvoiceID = smartReader.GetInt32("InvoiceID");
                    m_InvoiceDetailFixLog.PNRCode = smartReader.GetString("PNRCode");
                    m_InvoiceDetailFixLog.PaxName = smartReader.GetString("PaxName");
                    m_InvoiceDetailFixLog.AgentID = smartReader.GetInt32("AgentID");
                    m_InvoiceDetailFixLog.AgentBranchID = smartReader.GetInt32("AgentBranchID");
                    m_InvoiceDetailFixLog.UpdateTime = smartReader.GetDateTime("UpdateTime");
                    m_InvoiceDetailFixLog.OtherFees = smartReader.GetInt32("OtherFees");
                    m_InvoiceDetailFixLog.ReturnFees = smartReader.GetInt32("ReturnFees");
                    m_InvoiceDetailFixLog.ChangeTicket = smartReader.GetString("ChangeTicket");
                    m_InvoiceDetailFixLog.ChangeLevelFee = smartReader.GetInt32("ChangeLevelFee");
                    m_InvoiceDetailFixLog.ChangeLevelFeeVAT = smartReader.GetInt32("ChangeLevelFeeVAT");
                    m_InvoiceDetailFixLog.Note = smartReader.GetString("Note");
                    m_InvoiceDetailFixLog.AgentFee = smartReader.GetInt32("AgentFee");
                    m_InvoiceDetailFixLog.AgentFeeVAT = smartReader.GetInt32("AgentFeeVAT");
                    m_InvoiceDetailFixLog.SoldDate = smartReader.GetDateTime("SoldDate");
                    m_InvoiceDetailFixLog.TicketFareNew = smartReader.GetInt32("TicketFareNew");
                    m_InvoiceDetailFixLog.TicketVATNew = smartReader.GetInt32("TicketVATNew");
                    m_InvoiceDetailFixLog.TicketTaxNew = smartReader.GetInt32("TicketTaxNew");
                    m_InvoiceDetailFixLog.TicketTaxGlobalNew = smartReader.GetInt32("TicketTaxGlobalNew");
                    m_InvoiceDetailFixLog.TicketPriceNew = smartReader.GetInt32("TicketPriceNew");
                    m_InvoiceDetailFixLog.OtherFeesNew = smartReader.GetInt32("OtherFeesNew");
                    m_InvoiceDetailFixLog.ReturnFeesNew = smartReader.GetInt32("ReturnFeesNew");
                    m_InvoiceDetailFixLog.ChangeLevelFeeNew = smartReader.GetInt32("ChangeLevelFeeNew");
                    m_InvoiceDetailFixLog.ChangeLevelFeeVATNew = smartReader.GetInt32("ChangeLevelFeeVATNew");
                    m_InvoiceDetailFixLog.TripCodeNew = smartReader.GetString("TripCodeNew");
                    m_InvoiceDetailFixLog.FixUser = smartReader.GetString("FixUser");
                    m_InvoiceDetailFixLog.FixDatetime = smartReader.GetDateTime("FixDatetime");
                    l_InvoiceDetailFixLog.Add(m_InvoiceDetailFixLog);
                }
                smartReader.disposeReader(reader);
                return l_InvoiceDetailFixLog;
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
        
        //---------------------------------------------------------------------------------------------
        public List<InvoiceDetailFixLog> GetByTicketNo(string ticketNo)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("InvoiceDetailFixLogGetByTicketNo");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TicketNo", ticketNo));
                List<InvoiceDetailFixLog> lAr = init(cmd);
                return lAr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}