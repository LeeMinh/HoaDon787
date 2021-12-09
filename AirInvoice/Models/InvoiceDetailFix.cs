using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace AirlineInvoice.Models
{
    public class InvoiceDetailFix
    {
        
        private DBAccess db;
        //--------------------------------------------------------------------------
        public InvoiceDetailFix()
        {
            db = new DBAccess();
        }
        //--------------------------------------------------------------------------
        ~InvoiceDetailFix()
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
        [AllowHtml]
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
        //--------------------------------------------------------------------------
        public List<InvoiceDetailFix> init(SqlCommand cmd)
        {
            SqlConnection con = db.getConnection();
            cmd.Connection = con;
            List<InvoiceDetailFix> l_InvoiceDetailFix = new List<InvoiceDetailFix>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                SmartDataReader smartReader = new SmartDataReader(reader);
                while (smartReader.Read())
                {
                    InvoiceDetailFix m_InvoiceDetailFix = new InvoiceDetailFix();
                    m_InvoiceDetailFix.InvoiceDetailID = smartReader.GetInt32("InvoiceDetailID");
                    m_InvoiceDetailFix.TicketNo = smartReader.GetString("TicketNo");
                    m_InvoiceDetailFix.TicketType = smartReader.GetInt32("TicketType");
                    m_InvoiceDetailFix.TripCode = smartReader.GetString("TripCode");
                    m_InvoiceDetailFix.TicketFare = smartReader.GetInt32("TicketFare");
                    m_InvoiceDetailFix.TicketVAT = smartReader.GetInt32("TicketVAT");
                    m_InvoiceDetailFix.TicketTax = smartReader.GetInt32("TicketTax");
                    m_InvoiceDetailFix.TicketTaxGlobal = smartReader.GetInt32("TicketTaxGlobal");
                    m_InvoiceDetailFix.RealPay = smartReader.GetInt32("RealPay");
                    m_InvoiceDetailFix.TicketPrice = smartReader.GetInt32("TicketPrice");
                    m_InvoiceDetailFix.VCRDisplay = smartReader.GetString("VCRDisplay");
                    m_InvoiceDetailFix.UpdateSystem = smartReader.GetBoolean("UpdateSystem");
                    m_InvoiceDetailFix.Airline = smartReader.GetString("Airline");
                    m_InvoiceDetailFix.InvoiceID = smartReader.GetInt32("InvoiceID");
                    m_InvoiceDetailFix.PNRCode = smartReader.GetString("PNRCode");
                    m_InvoiceDetailFix.PaxName = smartReader.GetString("PaxName");
                    m_InvoiceDetailFix.AgentID = smartReader.GetInt32("AgentID");
                    m_InvoiceDetailFix.AgentBranchID = smartReader.GetInt32("AgentBranchID");
                    m_InvoiceDetailFix.UpdateTime = smartReader.GetDateTime("UpdateTime");
                    m_InvoiceDetailFix.OtherFees = smartReader.GetInt32("OtherFees");
                    m_InvoiceDetailFix.ReturnFees = smartReader.GetInt32("ReturnFees");
                    m_InvoiceDetailFix.ChangeTicket = smartReader.GetString("ChangeTicket");
                    m_InvoiceDetailFix.ChangeLevelFee = smartReader.GetInt32("ChangeLevelFee");
                    m_InvoiceDetailFix.ChangeLevelFeeVAT = smartReader.GetInt32("ChangeLevelFeeVAT");
                    m_InvoiceDetailFix.Note = smartReader.GetString("Note");
                    m_InvoiceDetailFix.AgentFee = smartReader.GetInt32("AgentFee");
                    m_InvoiceDetailFix.AgentFeeVAT = smartReader.GetInt32("AgentFeeVAT");
                    m_InvoiceDetailFix.SoldDate = smartReader.GetDateTime("SoldDate");
                    l_InvoiceDetailFix.Add(m_InvoiceDetailFix);
                }
                smartReader.disposeReader(reader);
                return l_InvoiceDetailFix;
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
                SqlCommand cmd = new SqlCommand("InvoiceDetailFixInsert");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TicketNo",this.TicketNo));
                cmd.Parameters.Add(new SqlParameter("@TicketType",this.TicketType));
                cmd.Parameters.Add(new SqlParameter("@TripCode",this.TripCode));
                cmd.Parameters.Add(new SqlParameter("@TicketFare",this.TicketFare));
                cmd.Parameters.Add(new SqlParameter("@TicketVAT",this.TicketVAT));
                cmd.Parameters.Add(new SqlParameter("@TicketTax",this.TicketTax));
                cmd.Parameters.Add(new SqlParameter("@TicketTaxGlobal",this.TicketTaxGlobal));
                cmd.Parameters.Add(new SqlParameter("@RealPay",this.RealPay));
                cmd.Parameters.Add(new SqlParameter("@TicketPrice",this.TicketPrice));
                cmd.Parameters.Add(new SqlParameter("@VCRDisplay",this.VCRDisplay));
                cmd.Parameters.Add(new SqlParameter("@UpdateSystem",this.UpdateSystem));
                cmd.Parameters.Add(new SqlParameter("@Airline",this.Airline));
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", this.InvoiceID));
                cmd.Parameters.Add(new SqlParameter("@PNRCode",this.PNRCode));
                cmd.Parameters.Add(new SqlParameter("@PaxName",this.PaxName));
                cmd.Parameters.Add(new SqlParameter("@AgentID",this.AgentID));
                cmd.Parameters.Add(new SqlParameter("@AgentBranchID",this.AgentBranchID));
                cmd.Parameters.Add(new SqlParameter("@UpdateTime",this.UpdateTime));
                cmd.Parameters.Add(new SqlParameter("@OtherFees",this.OtherFees));
                cmd.Parameters.Add(new SqlParameter("@ReturnFees",this.ReturnFees));
                cmd.Parameters.Add(new SqlParameter("@ChangeTicket",this.ChangeTicket));
                cmd.Parameters.Add(new SqlParameter("@ChangeLevelFee",this.ChangeLevelFee));
                cmd.Parameters.Add(new SqlParameter("@ChangeLevelFeeVAT",this.ChangeLevelFeeVAT));
                cmd.Parameters.Add(new SqlParameter("@Note",this.Note));
                cmd.Parameters.Add(new SqlParameter("@AgentFee",this.AgentFee));
                cmd.Parameters.Add(new SqlParameter("@AgentFeeVAT",this.AgentFeeVAT));
                cmd.Parameters.Add(new SqlParameter("@SoldDate",this.SoldDate));
                cmd.Parameters.Add("@InvoiceDetailID", SqlDbType.Int).Direction = ParameterDirection.Output;
                db.ExecuteSQL(cmd);
                this.InvoiceDetailID = (cmd.Parameters["@InvoiceDetailID"].Value == null) ? 0 : Convert.ToInt32(cmd.Parameters["@InvoiceDetailID"].Value);
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
                SqlCommand cmd = new SqlCommand("InvoiceDetailFixUpdate");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@InvoiceDetailID",this.InvoiceDetailID));
                cmd.Parameters.Add(new SqlParameter("@TicketNo",this.TicketNo));
                cmd.Parameters.Add(new SqlParameter("@TicketType",this.TicketType));
                cmd.Parameters.Add(new SqlParameter("@TripCode",this.TripCode));
                cmd.Parameters.Add(new SqlParameter("@TicketFare",this.TicketFare));
                cmd.Parameters.Add(new SqlParameter("@TicketVAT",this.TicketVAT));
                cmd.Parameters.Add(new SqlParameter("@TicketTax",this.TicketTax));
                cmd.Parameters.Add(new SqlParameter("@TicketTaxGlobal",this.TicketTaxGlobal));
                cmd.Parameters.Add(new SqlParameter("@RealPay",this.RealPay));
                cmd.Parameters.Add(new SqlParameter("@TicketPrice",this.TicketPrice));
                cmd.Parameters.Add(new SqlParameter("@VCRDisplay",this.VCRDisplay));
                cmd.Parameters.Add(new SqlParameter("@UpdateSystem",this.UpdateSystem));
                cmd.Parameters.Add(new SqlParameter("@Airline",this.Airline));
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", this.InvoiceID));
                cmd.Parameters.Add(new SqlParameter("@PNRCode",this.PNRCode));
                cmd.Parameters.Add(new SqlParameter("@PaxName",this.PaxName));
                cmd.Parameters.Add(new SqlParameter("@AgentID",this.AgentID));
                cmd.Parameters.Add(new SqlParameter("@AgentBranchID",this.AgentBranchID));
                cmd.Parameters.Add(new SqlParameter("@UpdateTime",this.UpdateTime));
                cmd.Parameters.Add(new SqlParameter("@OtherFees",this.OtherFees));
                cmd.Parameters.Add(new SqlParameter("@ReturnFees",this.ReturnFees));
                cmd.Parameters.Add(new SqlParameter("@ChangeTicket",this.ChangeTicket));
                cmd.Parameters.Add(new SqlParameter("@ChangeLevelFee",this.ChangeLevelFee));
                cmd.Parameters.Add(new SqlParameter("@ChangeLevelFeeVAT",this.ChangeLevelFeeVAT));
                cmd.Parameters.Add(new SqlParameter("@Note",this.Note));
                cmd.Parameters.Add(new SqlParameter("@AgentFee",this.AgentFee));
                cmd.Parameters.Add(new SqlParameter("@AgentFeeVAT",this.AgentFeeVAT));
                cmd.Parameters.Add(new SqlParameter("@SoldDate",this.SoldDate));
                db.ExecuteSQL(cmd);                
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        //---------------------------------------------------------------------------------------------
        public void FixMoney(ref int InvoiceDetailLogId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("InvoiceDetailFixMoney");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TicketNo", this.TicketNo));
                cmd.Parameters.Add(new SqlParameter("@TicketFare",this.TicketFare));
                cmd.Parameters.Add(new SqlParameter("@TicketVAT",this.TicketVAT));
                cmd.Parameters.Add(new SqlParameter("@TicketTax",this.TicketTax));
                cmd.Parameters.Add(new SqlParameter("@TicketTaxGlobal",this.TicketTaxGlobal));
                cmd.Parameters.Add(new SqlParameter("@TicketPrice",this.TicketPrice));
                cmd.Parameters.Add(new SqlParameter("@OtherFees",this.OtherFees));
                cmd.Parameters.Add(new SqlParameter("@ReturnFees",this.ReturnFees));
                cmd.Parameters.Add(new SqlParameter("@ChangeLevelFee",this.ChangeLevelFee));
                cmd.Parameters.Add(new SqlParameter("@ChangeLevelFeeVAT",this.ChangeLevelFeeVAT));
                cmd.Parameters.Add(new SqlParameter("@TripCode", this.TripCode));
                cmd.Parameters.Add(new SqlParameter("@FixUser", userprofile.CurrentUser.UserName));
                cmd.Parameters.Add("@InvoiceDetailLogId", SqlDbType.Int).Direction = ParameterDirection.Output;
                db.ExecuteSQL(cmd);
                InvoiceDetailLogId = (cmd.Parameters["@InvoiceDetailLogId"].Value == null) ? 0 : Convert.ToInt32(cmd.Parameters["@InvoiceDetailLogId"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        
        //---------------------------------------------------------------------------------------------
        public InvoiceDetailFix GetByTicketNo(string ticketNo)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("InvoiceDetailFixGetByTicketNo");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TicketNo", ticketNo));
                List<InvoiceDetailFix> lAr = init(cmd);
                if (lAr.Count == 1) return lAr[0] as InvoiceDetailFix;
                else return new InvoiceDetailFix();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}