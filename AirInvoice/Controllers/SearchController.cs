using AirlineInvoice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AirlineInvoice.Controllers
{
    public class SearchController : BaseController
    {


        #region Customers
        public PartialViewResult Customers()
        {
            return PartialView(GetListCustomer(1, 20));
        }

        public PartialViewResult GetCustomerData(int currentPage, int pageSize, string searchText)
        {
            return PartialView("CustomerData", GetListCustomer(currentPage, pageSize, searchText));
        }

        private List<Models.Customer> GetListCustomer(int currentPage, int pageSize, string searchText = "")
        {
            using (var db = new Models.InvoiceContext())
            {
                var query = db.Customers.Where(x => x.IdentityNumber.Contains(searchText) || x.FullName.Contains(searchText)).OrderBy(x => x.FullName);
                ViewBag.CurrentPage = currentPage;
                ViewBag.PageSize = pageSize;
                ViewBag.EntityName = "CustomerSearch";
                ViewBag.TotalRow = query.Count();
                return query.ToPagedList(currentPage, pageSize);
            }
        }


        public JsonResult SearchCustomer(string identityNumber)
        {
            using (var db = new InvoiceContext())
            {
                var obj = db.Customers.FirstOrDefault(x => x.IdentityNumber.Equals(identityNumber));
                if (obj != null)
                {
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CustomerID = -1 }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region Ticket
        public PartialViewResult Ticket(int invoiceType)
        {
            var model = GetListTicket(1, 20, invoiceType);
            return PartialView(model);
        }

        public PartialViewResult GetTicketData(int currentPage, int pageSize, int invoiceType, string ticketNo, int ticketType, string fromDate, string toDate, string tripCode)
        {
            var model = GetListTicket(currentPage, pageSize, invoiceType, ticketNo, ticketType, fromDate, toDate, tripCode);
            return PartialView("TicketData", model);
        }

        private List<InvoiceDetail> GetListTicket(int currentPage, int pageSize, int invoiceType = 0, string ticketNo = "", int ticketType = 0, string fromDate = "", string toDate = "", string tripCode = "")
        {
            ViewBag.InvoiceType = invoiceType;
            var branchID = Models.userprofile.CurrentUser.AgentBranchID;
            var agentID = Models.userprofile.CurrentUser.AgentID;
            DateTime dFromDate = fromDate.ConvertToDateTime();
            DateTime dToDate = toDate.ConvertToDateTime();
            ViewBag.EntityName = "TicketSearch";
            using (var db = new Models.InvoiceContext())
            {
                var query = db.InvoiceDetails.Where(x => x.AgentID == agentID && (x.AgentBranchID == branchID || branchID == 0)
                    && x.TicketNo.Contains(ticketNo) && x.TripCode.Contains(tripCode)
                    && (x.TicketType == ticketType || ticketType == 0) && (x.InvoiceID == null || x.InvoiceID == 0));
                
                if (invoiceType == (int)Utils.InvoiceType.Normal)
                {
                    query = query.Where(x => x.TicketType != (int)Utils.TicketType.Return);
                }
                else if (invoiceType == (int)Utils.InvoiceType.Return)
                {
                    query = query.Where(x => x.TicketType == (int)Utils.TicketType.Return);
                }
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    query = query.Where(x => x.SoldDate >= dFromDate && x.SoldDate <= dToDate);
                }
                else
                {
                    dFromDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
                    dToDate = DateTime.Now;
                    query = query.Where(x => x.SoldDate >= dFromDate && x.SoldDate <= dToDate);
                }
                ViewBag.CurrentPage = currentPage;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRow = query.Count();
                query = query.OrderBy(x => x.TicketNo);
                var model = query.ToPagedList(currentPage, pageSize);
                if (model != null)
                {
                    model.ForEach(x => x.VCRDisplay = string.Empty);
                }
                return model;
            }
        }
        #endregion
    }
}
