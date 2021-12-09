using AirlineInvoice.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace AirlineInvoice.Controllers
{
    public class TicketController : BaseController
    {
        //
        // GET: /Ticket/
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var isAccounting = Roles.IsUserInRole(Utils.CommonFunction._Accounting);
            if (isAccounting && Models.userprofile.CurrentUser.AgentBranchID > 0 && !Roles.IsUserInRole(Utils.CommonFunction._AdminRole))
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary {
                { "Controller", "account" },
                { "Action", "login" },
                {"returnUrl",filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery}
            });
            }
        }
        public ActionResult Index()
        {
            using (var db = new Models.InvoiceContext())
            {
                return View(GetListTicket(1, 20));
            }
        }
        public ActionResult AllTicket()
        {
            using (var db = new Models.InvoiceContext())
            {
                return View(GetListTicket(1, 20, "", "", (int)Utils.TicketType.All));
            }
        }
        public ActionResult Dometic()
        {
            using (var db = new Models.InvoiceContext())
            {
                return View(GetListTicket(1, 20, "", "", (int)Utils.TicketType.Dometic));
            }
        }

        public ActionResult Global()
        {
            using (var db = new Models.InvoiceContext())
            {
                return View(GetListTicket(1, 20, "", "", (int)Utils.TicketType.Global));
            }
        }

        public ActionResult Changed()
        {
            using (var db = new Models.InvoiceContext())
            {
                return View(GetListTicket(1, 20, "", "", (int)Utils.TicketType.Changed));
            }
        }

        public ActionResult Return()
        {
            using (var db = new Models.InvoiceContext())
            {
                return View(GetListTicket(1, 20, "", "", (int)Utils.TicketType.Return));
            }
        }
        private List<InvoiceDetail> GetListTicket(int currentPage, int pageSize, string ticketNo = "", string invoiceNo = "", int ticketType = 0, string fromDate = "", string toDate = "", string tripCode = "")
        {
            var branchID = Models.userprofile.CurrentUser.AgentBranchID;
            var agentID = Models.userprofile.CurrentUser.AgentID;
            DateTime dFromDate = new DateTime();
            DateTime dToDate = new DateTime();
            DateTime dnow = DateTime.Now;
            if (fromDate != "")
            {
                dFromDate = fromDate.ConvertToDateTime();
            }
            else
            {
                dFromDate = new DateTime(dnow.Year, dnow.Month, 1);
            }
            if (toDate != "")
            {
                dToDate = toDate.ConvertToDateTime();
            }
            else
            {                             
                if(dnow.Month + 1 > 12) { dToDate = new DateTime(dnow.Year +1, 1, 1).AddDays(-1); }
                else { dToDate = new DateTime(dnow.Year, dnow.Month + 1, 1).AddDays(-1); }
                
            }
            



            InvoiceDetail invoiceDetail = new InvoiceDetail();
            var ListInvoiceDetail = invoiceDetail.GetListPaged(currentPage, pageSize, agentID, branchID,0, "", ticketType, dFromDate, dToDate, tripCode);
            using (var db = new Models.InvoiceContext())
            {
                var query = db.InvoiceDetails.Where(x => x.AgentID == agentID && (x.AgentBranchID == branchID || branchID == 0 || branchID == null));
                if (!string.IsNullOrEmpty(ticketNo))
                {
                    query = query.Where(x => x.TicketNo == ticketNo);
                }
                if (!string.IsNullOrEmpty(tripCode))
                {
                    query = query.Where(x => x.TripCode == tripCode);
                }
                if (ticketType != 5)
                {
                    query = query.Where(x => x.TicketType == ticketType || ticketType == 0);
                }
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    query = query.Where(x => x.SoldDate >= dFromDate && x.SoldDate <= dToDate);
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

        public PartialViewResult GetTicketData(int currentPage, int pageSize, string ticketNo, string invoiceNo, int ticketType, string fromDate, string toDate, string tripCode)
        {
            switch (ticketType)
            {
                case (int)Utils.TicketType.All:
                    return PartialView("AllTicketData", GetListTicket(currentPage, pageSize, ticketNo, invoiceNo, ticketType, fromDate, toDate, tripCode));
                case (int)Utils.TicketType.Dometic:
                    return PartialView("DometicData", GetListTicket(currentPage, pageSize, ticketNo, invoiceNo, ticketType, fromDate, toDate, tripCode));
                case (int)Utils.TicketType.Global:
                    return PartialView("GlobalData", GetListTicket(currentPage, pageSize, ticketNo, invoiceNo, ticketType, fromDate, toDate, tripCode));
                case (int)Utils.TicketType.Changed:
                    return PartialView("ChangedData", GetListTicket(currentPage, pageSize, ticketNo, invoiceNo, ticketType, fromDate, toDate, tripCode));
                case (int)Utils.TicketType.Return:
                    return PartialView("ReturnData", GetListTicket(currentPage, pageSize, ticketNo, invoiceNo, ticketType, fromDate, toDate, tripCode));
                default:
                    break;
            }
            return PartialView("TicketData", GetListTicket(currentPage, pageSize, ticketNo, invoiceNo, ticketType, fromDate, toDate, tripCode));
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
        [HttpPost]
        public string CAddTicket(string TicketNo, string TicketType, string TripCode, string TicketFare, string TicketVAT, string TicketTax, string TicketTaxGlobal, string TicketPrice, string VCRDisplay, string PNRCode, string PaxName, string OtherFees, string ReturnFees, string ChangeTicket, string ChangeLevelFee, string ChangeLevelFeeVAT, string AgentFee, string AgentFeeVAT, string SoldDate)
        {
            try
            {
                TicketNo = TicketNo.Length > 0 ? TicketNo : " ";
                TicketType = TicketType.Length > 0 ? TicketType : " ";
                TripCode = TripCode.Length > 0 ? TripCode : " ";
                TicketFare = TicketFare.Length > 0 ? TicketFare : "0";
                TicketVAT = TicketVAT.Length > 0 ? TicketVAT : "0";
                TicketTax = TicketTax.Length > 0 ? TicketTax : "0";
                TicketTaxGlobal = TicketTaxGlobal.Length > 0 ? TicketTaxGlobal : "0";
                TicketPrice = TicketPrice.Length > 0 ? TicketPrice : "0";
                VCRDisplay = VCRDisplay.Length > 0 ? VCRDisplay : " ";
                PNRCode = PNRCode.Length > 0 ? PNRCode : " ";
                PaxName = PaxName.Length > 0 ? PaxName : " ";
                OtherFees = OtherFees.Length > 0 ? OtherFees : "0";
                ReturnFees = ReturnFees.Length > 0 ? ReturnFees : "0";
                ChangeTicket = ChangeTicket.Length > 0 ? ChangeTicket : " ";
                ChangeLevelFee = ChangeLevelFee.Length > 0 ? ChangeLevelFee : "0";
                ChangeLevelFeeVAT = ChangeLevelFeeVAT.Length > 0 ? ChangeLevelFeeVAT : "0";
                AgentFee = AgentFee.Length > 0 ? AgentFee : "0";
                AgentFeeVAT = AgentFeeVAT.Length > 0 ? AgentFeeVAT : "0";
                SoldDate = SoldDate.Length > 0 ? SoldDate : " ";
                string data = "";
                InvoiceDetail iv = new InvoiceDetail();
                iv = iv.GetByTicketNo(TicketNo.Trim());
                if (iv.InvoiceDetailID > 0)
                {
                    if (iv.InvoiceID > 0)
                    {
                        data = "Số vé " + iv.TicketNo + " đã xuất trong hóa đơn mã " + iv.InvoiceID;
                    }
                    else
                    {
                        data = "Số vé " + iv.TicketNo + " đã có trong cơ sở dữ liệu";
                    }
                }
                else
                {
                    iv.TicketNo = TicketNo.Trim();
                    switch (TicketType)
                    {
                        case "Vé nội địa":
                            iv.TicketType = 1;
                            break;
                        case "Vé quôc tế":
                            iv.TicketType = 2;
                            break;
                        case "Vé đổi":
                            iv.TicketType = 3;
                            break;
                        case "Vé hoàn":
                            iv.TicketType = 4;
                            break;
                        default:
                            iv.TicketType = 1;
                            break;
                    }

                    iv.TripCode = TripCode.Trim();
                    iv.TicketFare = Convert.ToInt32(TicketFare.Trim());
                    iv.TicketVAT = Convert.ToInt32(TicketVAT.Trim());
                    iv.TicketTax = Convert.ToInt32(TicketTax.Trim());
                    iv.TicketTaxGlobal = Convert.ToInt32(TicketTaxGlobal.Trim());
                    iv.TicketPrice = Convert.ToInt32(TicketPrice.Trim());
                    iv.VCRDisplay = VCRDisplay.Trim();
                    iv.PNRCode = PNRCode.Trim();
                    iv.PaxName = PaxName.Trim();
                    iv.OtherFees = Convert.ToInt32(OtherFees.Trim());
                    iv.ReturnFees = Convert.ToInt32(ReturnFees.Trim());
                    iv.ChangeTicket = ChangeTicket.Trim();
                    iv.ChangeLevelFee = Convert.ToInt32(ChangeLevelFee.Trim());
                    iv.ChangeLevelFeeVAT = Convert.ToInt32(ChangeLevelFeeVAT.Trim());
                    iv.AgentFee = Convert.ToInt32(AgentFee.Trim());
                    iv.AgentFeeVAT = Convert.ToInt32(AgentFeeVAT.Trim());
                    DateTime exp = new DateTime();
                    exp = DateTime.ParseExact(SoldDate.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    iv.SoldDate = exp;

                    iv.RealPay = 0;
                    iv.UpdateSystem = true;
                    iv.Airline = "VN";
                    iv.InvoiceID = 0;
                    iv.AgentID = userprofile.CurrentUser.AgentID;
                    iv.AgentBranchID = userprofile.CurrentUser.AgentBranchID;
                    iv.UpdateTime = DateTime.Now;
                    iv.Note = " ";
                    int i = 0;
                    i = iv.Insert();
                    if (i > 0)
                    {
                        data = "Thêm thành công số vé: " + TicketNo;
                    }
                    else
                    {
                        data = "Thêm số vé " + TicketNo + " lỗi!";
                    }
                }
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
