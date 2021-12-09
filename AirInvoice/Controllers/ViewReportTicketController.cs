using AirlineInvoice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI;
namespace AirlineInvoice.Controllers
{
    public class ViewReportTicketController : Controller
    {
        //
        // GET: /ViewReportTicket/

        public ActionResult Index()
        {
            using (var db = new Models.InvoiceContext())
            {
                //Get list AgentBrach by AgenId
                var query = db.AgentBranches.Where(x => x.AgentID == Models.userprofile.CurrentUser.AgentID);
                List<AgentBranch> listAgenBranches = new List<AgentBranch>();
                listAgenBranches = query.ToList<AgentBranch>();
                TempData["ListAgenBranches"] = listAgenBranches;
                //
                Session["ListTicketSearch"] = null;
                Session["ListTicketDefault"] = GetListTicket(1, 2147483647);
                return View(GetListTicket(1, 20));
            }
        }
        public PartialViewResult ViewReportTicketData(int currentPage, int pageSize, int agentBranches, int status, string ticketNo, string invoiceNo, int ticketType, string fromDate, string toDate, string tripCode)
        {
            Session["ListTicketDefault"] = null;
            Session["ListTicketSearch"] = GetListTicket(currentPage, 2147483647, agentBranches, status, ticketNo, invoiceNo, ticketType, fromDate, toDate, tripCode);
            return PartialView("ViewReportTicketData", GetListTicket(currentPage, pageSize, agentBranches, status, ticketNo, invoiceNo, ticketType, fromDate, toDate, tripCode));
        }
        private List<InvoiceDetail> GetListTicket(int currentPage, int pageSize, int agentBranches = -1, int status = -1, string ticketNo = "", string invoiceNo = "", int ticketType = 0, string fromDate = "", string toDate = "", string tripCode = "")
        {
            var branchID = Models.userprofile.CurrentUser.AgentBranchID;
            var agentID = Models.userprofile.CurrentUser.AgentID;
            DateTime dFromDate = fromDate.ConvertToDateTime();
            DateTime dToDate = toDate.ConvertToDateTime();

            using (var db = new Models.InvoiceContext())
            {
                var query = db.InvoiceDetails.Where(x => x.AgentID == agentID);
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    query = query.Where(x => x.SoldDate >= dFromDate && x.SoldDate <= dToDate);
                }
                if (ticketType != 0)
                {
                    query = query.Where(x => x.TicketType == ticketType);
                }
                if (!string.IsNullOrEmpty(ticketNo))
                {
                    query = query.Where(x => x.TicketNo.Contains(ticketNo));
                }
                if (!string.IsNullOrEmpty(invoiceNo))
                {
                    var querysplit = from iv in db.Invoices
                                     join id in query
                                     on iv.InvoiceID equals id.InvoiceID
                                     where iv.InvoiceNumber.Contains(invoiceNo)
                                     select id;
                    query = querysplit;
                }
                if (agentBranches != -1)
                {
                    query = query.Where(x => x.AgentBranchID == agentBranches);
                }
                if (status != -1)
                {
                    if (status == 1)
                    {
                        var querysplit = from iv in db.Invoices
                                         join id in query
                                         on iv.InvoiceID equals id.InvoiceID
                                         where iv.InvoiceNumber != null
                                         select id;
                        query = querysplit;
                    }
                    if (status == 0)
                    {
                        var querysplit = from iv in db.Invoices
                                         join id in query
                                         on iv.InvoiceID equals id.InvoiceID
                                         where iv.InvoiceNumber == null
                                         select id;
                        query = querysplit;
                    }

                }
                ViewBag.CurrentPage = currentPage;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRow = query.Count();
                query = query.OrderByDescending(x => x.SoldDate);
                var model = query.ToPagedList(currentPage, pageSize);
                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.VCRDisplay = string.Empty;
                        var invoice = db.Invoices.FirstOrDefault(y => y.InvoiceID == x.InvoiceID);
                        if (invoice != null)
                        {
                            x.InvoiceNumber = invoice.InvoiceNumber;
                        }
                    });
                }
                return model;
            }
        }
        public JsonResult GetVCR(string InvoiceDetailID)
        {
            using (var db = new InvoiceContext())
            {
                var key = Utils.CommonFunction.Expand(InvoiceDetailID);
                var ticket = db.InvoiceDetails.FirstOrDefault(x => x.InvoiceDetailID == key);
                // neu khong tim thay thi call service cua vna
                string vcr = string.Empty;
                if (ticket != null)
                {
                    vcr = ticket.VCRDisplay;
                }
                return Json(new
                {
                    result = vcr
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //Xuat file excel
        public ActionResult ExportExcel()
        {
            var listExcel = new System.Data.DataTable("listExcel");
            listExcel.Columns.Add("STT", typeof(int));
            listExcel.Columns.Add("SoVe", typeof(string));
            listExcel.Columns.Add("LoaiVe", typeof(string));
            listExcel.Columns.Add("NgayBan", typeof(DateTime));
            listExcel.Columns.Add("SoHoaDon", typeof(string));
            listExcel.Columns.Add("MaDatVe", typeof(string));
            listExcel.Columns.Add("HanhTrinh", typeof(string));
            listExcel.Columns.Add("GiaVe", typeof(int));
            listExcel.Columns.Add("VAT", typeof(int));
            listExcel.Columns.Add("PhiKhac", typeof(int));
            listExcel.Columns.Add("TongTien", typeof(int));
            listExcel.Columns.Add("MaHoaDon", typeof(string));
            List<InvoiceDetail> ListTicket = new List<InvoiceDetail>();
            ListTicket = (Session["ListTicketSearch"] == null ? Session["ListTicketDefault"] : Session["ListTicketSearch"]) as List<InvoiceDetail>;
            int count = 1;
            string ticketType = "";
            foreach (var item in ListTicket)
            {
                switch (item.TicketType)
                {
                    case 1:
                        ticketType = "Vé nội địa";
                        break;
                    case 2:
                        ticketType = "Vé quốc tế";
                        break;
                    case 3:
                        ticketType = "Vé đổi";
                        break;
                    case 4:
                        ticketType = "Vé hoàn";
                        break;
                    default:
                        ticketType = "";
                        break;
                }
                listExcel.Rows.Add(count, item.TicketNo, ticketType, item.SoldDate, item.InvoiceNumber, item.PNRCode, item.TripCode, item.TicketFare, item.TicketVAT, item.TicketTax, item.TicketPrice, item.InvoiceID);
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
