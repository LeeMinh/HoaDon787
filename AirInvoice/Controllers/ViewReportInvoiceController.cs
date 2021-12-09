using AirlineInvoice.Models;
using DevExpress.Web.Mvc;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AirlineInvoice.ReportFiles;
using System.Web.UI;
using System.Globalization;

namespace AirlineInvoice.Controllers
{
    public class ViewReportInvoiceController : Controller
    {
        //
        // GET: /ViewReportInvoice/

        public ActionResult Index()
        {
            using (var db = new Models.InvoiceContext())
            {
                //Get list AgentBrach by AgenId
                var list = db.AgentBranches.Where(x => x.AgentID == Models.userprofile.CurrentUser.AgentID);
                List<AgentBranch> listAgenBranches = new List<AgentBranch>();
                listAgenBranches = list.ToList<AgentBranch>();
                TempData["ListAgenBranches"] = listAgenBranches;
                //
                var sumInvoice = 0;
                float sumTotal = 0;
                float sumTotalAgentFee = 0;
                var query = GetListInvoice(1, 2147483647, 0, -1, string.Empty, string.Empty, string.Empty, string.Empty, Membership.GetUser().ProviderUserKey.ToString());
                foreach (var item in query)
                {
                    sumInvoice += 1;
                    sumTotal += Convert.ToInt64(item.Total == null ? 0 : item.Total);
                    sumTotalAgentFee += Convert.ToInt64(item.TotalAgentFee == null ? 0 : item.TotalAgentFee);
                }
                ViewBag.sumInvoice = sumInvoice;
                ViewBag.sumTotal = sumTotal;
                ViewBag.sumTotalAgentFee = sumTotalAgentFee;
                Session["ListInvoiceSearch"] = null;
                Session["ListInvoiceDefault"] = GetListInvoice(1, 2147483647, 0, -1, string.Empty, string.Empty, string.Empty, string.Empty, Membership.GetUser().ProviderUserKey.ToString());
                return View(GetListInvoice(1, 20, 0, -1, string.Empty, string.Empty, string.Empty, string.Empty, Membership.GetUser().ProviderUserKey.ToString()));
                
            }
        }
        public PartialViewResult ViewReportInvoiceData(int currentPage, int pageSize, int invoiceid = 0, int agentBranches = -1, string invoiceno = "", string ticketNoSearch = "", string fromDate = "", string toDate = "",
            string userID = "", int invoiceType = 0, int invoiceStatus = 0, bool isFilter = false)
        {
            int sumInvoice = 0;
            float sumTotal = 0;
            float sumTotalAgentFee = 0;
            var query = GetListInvoice(currentPage, 2147483647, invoiceid, agentBranches, invoiceno, ticketNoSearch, fromDate, toDate, userID, invoiceType, invoiceStatus, isFilter);
            foreach (var item in query)
            {
                sumInvoice += 1;
                sumTotal += Convert.ToInt64(item.Total == null ? 0 : item.Total);
                sumTotalAgentFee += Convert.ToInt64(item.TotalAgentFee == null ? 0 : item.TotalAgentFee);
            }
            ViewBag.sumInvoice = sumInvoice;
            ViewBag.sumTotal = sumTotal;
            ViewBag.sumTotalAgentFee = sumTotalAgentFee;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            Session["ListInvoiceDefault"] = null;
            Session["ListInvoiceSearch"] = GetListInvoice(currentPage, 2147483647, invoiceid, agentBranches, invoiceno, ticketNoSearch, fromDate, toDate, userID, invoiceType, invoiceStatus, isFilter);
            return PartialView("ViewReportInvoiceData", GetListInvoice(currentPage, pageSize, invoiceid, agentBranches, invoiceno, ticketNoSearch, fromDate, toDate, userID, invoiceType, invoiceStatus, isFilter));
        }
        private List<Models.Invoice> GetListInvoice(int currentPage, int pageSize, int invoiceid = 0, int agentBranches = -1, string invoiceno = "", string ticketNoSearch = "", string fromDate = "", string toDate = "",
                string userID = "", int invoiceType = 0, int invoiceStatus = -1, bool isFilter = false)
        {
            DateTime dFromDate = fromDate.ConvertToDateTime();
            DateTime dToDate = toDate.ConvertToDateTime();
            var isAccounting = Roles.IsUserInRole(Utils.CommonFunction._Accounting);
            using (var db = new Models.InvoiceContext())
            {
                var query = db.Invoices.Where(x => x.AgentID == Models.userprofile.CurrentUser.AgentID &&

                    (x.InvoiceStatus == 1));
                if (invoiceid > 0)
                {
                    query = query.Where(x => x.InvoiceID == invoiceid);
                }
                if (invoiceType != 0)
                {
                    query = query.Where(x => x.InvoiceType == invoiceType);
                }
                if (agentBranches != -1)
                {
                    query = query.Where(x => x.AgentBranchID == agentBranches);
                }
                if (agentBranches != -1)
                {
                    query = query.Where(x => x.AgentBranchID == agentBranches);
                }

                if (!string.IsNullOrEmpty(invoiceno))
                {
                    query = query.Where(x => x.InvoiceNumber.Contains(invoiceno.Trim()));
                }
                if (!string.IsNullOrEmpty(ticketNoSearch))
                {
                    var querysplit = from id in db.InvoiceDetails
                            join iv in query
                            on id.InvoiceID equals iv.InvoiceID
                            where id.TicketNo.Contains(ticketNoSearch.Trim())
                            select iv;
                    query = querysplit;
                }
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    query = query.Where(x => x.InvoiceDate >= dFromDate && x.InvoiceDate <= dToDate);
                }
                if (invoiceType != 0)
                {
                    query = query.Where(x => x.InvoiceType == invoiceType);
                }
                if (isFilter)
                {
                    if (!string.IsNullOrEmpty(userID))
                        query = query.Where(x => x.UserCreate.Equals(userID));
                }
                else
                {
                    if (Models.userprofile.CurrentUser.AgentBranchID != 0)
                    {
                        if (isAccounting == false)
                        {
                            query = query.Where(x => x.AgentBranchID == Models.userprofile.CurrentUser.AgentBranchID);
                        }
                        else
                        {
                            query = query.Where(x => x.UserCreate.Equals(userID));
                        }


                    }
                }

                query = query.OrderByDescending(x => x.InvoiceNumber);
                ViewBag.CurrentPage = currentPage;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRow = query.Count();
                Session["TotalRow"] = query.Count();
                var lst = query.ToPagedList(currentPage, pageSize);
                lst.ForEach(x =>
                {
                    var branch = db.AgentBranches.FirstOrDefault(m => m.AgentBranchID == x.AgentBranchID);
                    if (branch != null)
                    {
                        x.BranchName = branch.BranchName;
                        x.BranchAddress = branch.Address;
                        x.BranchTax = branch.Tax;
                    }
                });
                return lst;
            }
        }
        //Xem danh sach ve
        public JsonResult GetTicketNo(string InvoiceID)
        {
            using (var db = new InvoiceContext())
            {
                var key = Convert.ToInt32(InvoiceID);
                List<InvoiceDetail> listInvoiceDetails = new List<InvoiceDetail>();
                var query = db.InvoiceDetails.Where(x => x.InvoiceID == key);
                listInvoiceDetails = query.ToList<InvoiceDetail>();
                string listTickets = string.Empty;
                if (listInvoiceDetails.Count > 0)
                {
                    foreach (var item in listInvoiceDetails)
                    {
                        listTickets += item.TicketNo + " | ";
                    }
                }
                return Json(new
                {
                    result = listTickets
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public string GetTicketNumber(int InvoiceID)
        {
            using (var db = new InvoiceContext())
            {
                var key = Convert.ToInt32(InvoiceID);
                List<InvoiceDetail> listInvoiceDetails = new List<InvoiceDetail>();
                var query = db.InvoiceDetails.Where(x => x.InvoiceID == key);
                listInvoiceDetails = query.ToList<InvoiceDetail>();
                string listTickets = string.Empty;
                if (listInvoiceDetails.Count > 0)
                {
                    foreach (var item in listInvoiceDetails)
                    {
                        listTickets += item.TicketNo + " | ";
                    }
                }
                return listTickets;
            }
        }
        //Xuat file excel
        public ActionResult ExportExcel()
        {
            var listExcel = new System.Data.DataTable("listExcel");
            listExcel.Columns.Add("STT", typeof(int));
            listExcel.Columns.Add("MaHD", typeof(int));
            listExcel.Columns.Add("SoHD", typeof(int));
            listExcel.Columns.Add("TongTien", typeof(int));
            listExcel.Columns.Add("PhiDichVu", typeof(int));
            listExcel.Columns.Add("ChiNhanh", typeof(string));
            listExcel.Columns.Add("KhachHang", typeof(string));
            listExcel.Columns.Add("NgayHD", typeof(DateTime));
            listExcel.Columns.Add("NguoiTao", typeof(string));
            listExcel.Columns.Add("MaSoThue", typeof(string));
            listExcel.Columns.Add("NgayHoaDon", typeof(string));
            listExcel.Columns.Add("SoVe", typeof(string));
            
            List<Invoice> ListInvoice = new List<Invoice>();
            ListInvoice = (Session["ListInvoiceSearch"] == null ? Session["ListInvoiceDefault"] : Session["ListInvoiceSearch"]) as List<Invoice>;
            int count = 1;
            string listTicketNo;
            foreach (var item in ListInvoice)
            {
                listTicketNo = GetTicketNumber(Convert.ToInt32(item.InvoiceID));

                listExcel.Rows.Add(count, item.InvoiceID, item.InvoiceNumber, item.Total, item.TotalAgentFee, item.BranchName, item.CustomerName, item.InvoiceDate, AirlineInvoice.Utils.CommonFunction.GetUsernameByID(item.UserCreate),"MST: " + item.CustomerTax, item.InvoiceDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture), listTicketNo);
                count++;
            }
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = listExcel;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Export_Excel.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
            return RedirectToAction("Index");

        }
    }
}
