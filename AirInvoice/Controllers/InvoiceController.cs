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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;
using SignTokenCore;
using System.Runtime.Serialization.Formatters.Soap;
using System.Net;
using Newtonsoft.Json.Linq;

namespace AirlineInvoice.Controllers
{

    public class InvoiceController : BaseController
    {
        #region Invoice
        //
        // GET: /Invoice/
        [HttpPost]
        public string deleteTicket(string InvoiceDetailID)
        {
            try
            {
                string retval = "";
                int id = 0;
                id = Convert.ToInt32(InvoiceDetailID);
                InvoiceDetail iv = new InvoiceDetail();
                iv.deletebyInvoiceDetailId(id);
                retval = "OK";
                return retval;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public ActionResult Index()
        {
            using (var db = new Models.InvoiceContext())
            {
                var isAccounting = Roles.IsUserInRole(Utils.CommonFunction._Accounting);
                //Get list AgentBrach by AgenId
                var query = db.AgentBranches.Where(x => x.AgentID == Models.userprofile.CurrentUser.AgentID);
                List<AgentBranch> listAgenBranches = new List<AgentBranch>();
                listAgenBranches = query.ToList<AgentBranch>();
                TempData["ListAgenBranches"] = listAgenBranches;
                //
                if (isAccounting)
                {
                    return View(GetListInvoice(1, 20, 0, -1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, Membership.GetUser().ProviderUserKey.ToString()));
                }
                return View(GetListInvoice(1, 20, 0, -1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, Membership.GetUser().ProviderUserKey.ToString()));

            }
        }
        public ActionResult ListCustomer()
        {
            List<Customers> listCus = new List<Customers>();
            Customers cus = new Customers();
            listCus = cus.GetByAgentID(userprofile.CurrentUser.AgentID);
            return View(listCus);
        }
        public ActionResult AddCustomer()
        {
            return View();
        }
        public void UpdateEInvoiceNumber  (string InvoiceID="0",string Enumber="")
        {
            try {
                int invoiceID = Convert.ToInt32(InvoiceID);
                using (var db = new Models.InvoiceContext())
                {
                    var invoice = db.Invoices.FirstOrDefault(x => x.InvoiceID == invoiceID);
                    invoice.ENumber = Enumber;
                    db.SaveChanges();
                }
            }
            catch { }
        }
        [HttpPost]
        public ActionResult InsertCustomer(FormCollection fc)
        {
            Customers cus = new Customers();
            cus.CustomerName = (string.IsNullOrEmpty(fc["CustomerName"]) ? "" : fc["CustomerName"]);
            cus.CustomerAddress = (string.IsNullOrEmpty(fc["CustomerAddress"]) ? "" : fc["CustomerAddress"]);
            cus.CustomerTax = (string.IsNullOrEmpty(fc["CustomerTax"]) ? "" : fc["CustomerTax"]);
            cus.AgentID = userprofile.CurrentUser.AgentID;
            cus.Insert();
            return RedirectToAction("ListCustomer");
        }
        public ActionResult EditCustomer(int id)
        {
            Customers cus = new Customers();
            if (id > 0)
            {
                cus = cus.GetByCustomerID(id);
            }
            return View(cus);
        }
        [HttpPost]
        public ActionResult UpdateCustomer(FormCollection fc)
        {
            Customers cus = new Customers();
            cus = cus.GetByCustomerID(Convert.ToInt32(fc["CustomerID"]));
            cus.CustomerName = (string.IsNullOrEmpty(fc["CustomerName"]) ? "" : fc["CustomerName"]);
            cus.CustomerAddress = (string.IsNullOrEmpty(fc["CustomerAddress"]) ? "" : fc["CustomerAddress"]);
            cus.CustomerTax = (string.IsNullOrEmpty(fc["CustomerTax"]) ? "" : fc["CustomerTax"]);
            cus.AgentID = userprofile.CurrentUser.AgentID;
            cus.Update();
            return RedirectToAction("ListCustomer");
        }

        public ActionResult DeleteCustomer(int id)
        {
            if (id > 0)
            {
                Customers cus = new Customers();
                cus = cus.GetByCustomerID(id);
                cus.Delete();
            }
            return RedirectToAction("ListCustomer");
        }
        public ActionResult TagSearch(string term)
        {
            List<Customers> listCustomer = new List<Customers>();
            Customers customer = new Customers();
            listCustomer = customer.GetByAgentID(userprofile.CurrentUser.AgentID);

            return this.Json(listCustomer.Where(t => t.CustomerName.Contains(term)),
                            JsonRequestBehavior.AllowGet);
        }
        public ActionResult FixTicket()
        {
            try
            {
                if (Request.QueryString["ticketNo"] != null)
                {
                    InvoiceDetailFix ticket = new InvoiceDetailFix();
                    ticket = ticket.GetByTicketNo(Request.QueryString["ticketNo"]);
                    ViewData["ticket"] = ticket;
                    List<InvoiceDetailFixLog> listTicketlog = new List<InvoiceDetailFixLog>();
                    InvoiceDetailFixLog ticketlog = new InvoiceDetailFixLog();
                    listTicketlog = ticketlog.GetByTicketNo(Request.QueryString["ticketNo"]);
                    ViewData["listTicketlog"] = listTicketlog;
                    return View();
                }
                else
                {
                    return RedirectToAction("EditInvoice", new { id = Session["EditInvoiceID"] });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult EditTicket(FormCollection fc)
        {
            try
            {
                string ticketNo = fc["TicketNo"];
                int TicketVAT = Convert.ToInt32(fc["TicketVAT"]);
                int ChangeLevelFeeVAT = Convert.ToInt32(fc["ChangeLevelFeeVAT"]);
                int TicketTax = Convert.ToInt32(fc["TicketTax"]);
                int TicketTaxGlobal = Convert.ToInt32(fc["TicketTaxGlobal"]);
                int OtherFees = Convert.ToInt32(fc["OtherFees"]);
                int ChangeLevelFee = Convert.ToInt32(fc["ChangeLevelFee"]);
                int ReturnFees = Convert.ToInt32(fc["ReturnFees"]);
                int TicketFare = Convert.ToInt32(fc["TicketFare"]);
                int TicketPrice = Convert.ToInt32(fc["TicketPrice"]);
                string TripCode = fc["TripCode"];
                InvoiceDetailFix ticket = new InvoiceDetailFix();
                ticket = ticket.GetByTicketNo(ticketNo);
                ticket.TicketVAT = TicketVAT;
                ticket.ChangeLevelFeeVAT = ChangeLevelFeeVAT;
                ticket.TicketTax = TicketTax;
                ticket.TicketTaxGlobal = TicketTaxGlobal;
                ticket.OtherFees = OtherFees;
                ticket.ChangeLevelFee = ChangeLevelFee;
                ticket.ReturnFees = ReturnFees;
                ticket.TicketFare = TicketFare;
                ticket.TicketPrice = TicketPrice;
                ticket.TripCode = TripCode;
                int logId = 0;
                ticket.FixMoney(ref logId);
                if (logId > 0)
                {
                    return RedirectToAction("EditInvoice", new { id = Session["EditInvoiceID"] });
                }
                else
                {
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public string DeleteWhiteSpace(string s)
        {
            s = s.Trim();
            while (s.IndexOf("  ") >= 0)
            {
                s = s.Replace("  ", " ");
            }
            return s;
        }
        private List<Models.Invoice> GetListInvoice(int currentPage, int pageSize, int invoiceid = 0, int agentBranches = -1, string cusName = "", string invoiceno = "", string ticketNoSearch = "", string fromDate = "", string toDate = "",
                string userID = "", int invoiceType = 0, int invoiceStatus = -1, bool isFilter = false)
        {
            DateTime dFromDate = fromDate.ConvertToDateTime();
            DateTime dToDate = toDate.ConvertToDateTime();
            var isAccounting = Roles.IsUserInRole(Utils.CommonFunction._Accounting);
            using (var db = new Models.InvoiceContext())
            {
                var query = db.Invoices.Where(x => x.AgentID == Models.userprofile.CurrentUser.AgentID &&

                    (x.InvoiceStatus == invoiceStatus || invoiceStatus == -1));
                if (invoiceid > 0)
                {
                    query = query.Where(x => x.InvoiceID == invoiceid);
                }

                if (agentBranches != -1)
                {
                    query = query.Where(x => x.AgentBranchID == agentBranches);
                }
                if (!string.IsNullOrEmpty(cusName))
                {
                    query = query.Where(x => x.CustomerName.Contains(cusName.Trim()));
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


        public PartialViewResult GetInvoiceData(int currentPage, int pageSize, int invoiceid = 0, int agentBranches = -1, string cusName = "", string invoiceno = "", string ticketNoSearch = "", string fromDate = "", string toDate = "",
            string userID = "", int invoiceType = 0, int invoiceStatus = 0, bool isFilter = false)
        {
            return PartialView("InvoiceData", GetListInvoice(currentPage, pageSize, invoiceid, agentBranches, cusName, invoiceno, ticketNoSearch, fromDate, toDate, userID, invoiceType, invoiceStatus, isFilter));
        }


        public ActionResult AddInvoice()
        {
            var obj = new Invoice() { InvoiceDate = DateTime.Today, AgentBranchID = Models.userprofile.CurrentUser.AgentBranchID };
            return View(obj);
        }
        private bool CheckValidInvoice(Invoice model)
        {
            bool result = true;
            if (model.AgentBranchID <= 0)
                return false;
            if (model.InvoiceDate.ConvertToDateTime() < "1990-1-1".ConvertToDateTime())
                return false;
            return result;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInvoice(Invoice model, FormCollection fc)
        {

            if (!CheckValidInvoice(model)) return View(model);
            using (var db = new InvoiceContext())
            {
                model.UserCreate = Membership.GetUser().ProviderUserKey.ToString();
                model.DateCreate = DateTime.Now;
                model.AgentID = Models.userprofile.CurrentUser.AgentID;
                model.InvoiceStatus = (int)Utils.InvoiceStatus.Building;

                CreateInvoiceNo(db, model);
                if (ModelState.IsValid)
                {
                    db.Invoices.Add(model);
                    db.SaveChanges();
                    Buyers buyer = new Buyers();
                    buyer = buyer.GetByInvoiceID(model.InvoiceID);
                    string Buyer = "";
                    Buyer = fc["Buyer"];
                    if (buyer.InvoiceID != 0)
                    {
                        buyer.InvoiceID = model.InvoiceID;
                        buyer.Buyer = string.IsNullOrEmpty(Buyer) ? "" : Buyer;
                        buyer.Update();
                        model.Buyer = Buyer;
                    }
                    else
                    {
                        buyer.InvoiceID = model.InvoiceID;
                        buyer.Buyer = string.IsNullOrEmpty(Buyer) ? "" : Buyer;
                        buyer.Insert();
                        model.Buyer = Buyer;
                    }
                }
                else
                {
                    return View(model);
                }
                return RedirectToAction("EditInvoice", new { id = Utils.CommonFunction.Shrink(model.InvoiceID) });
            }
        }
        private void CreateInvoiceNo(InvoiceContext db, Invoice model)
        {

            var profile = userprofile.GetUser(new Guid(model.UserCreate));
            if (Roles.IsUserInRole(profile.UserName, Utils.CommonFunction._Accounting)
                && !Roles.IsUserInRole(profile.UserName, Utils.CommonFunction._AdminRole)
                && profile.AgentBranchID != 0)
            {
                var currentUser = userprofile.CurrentUser;
                if (Roles.IsUserInRole(currentUser.UserName, Utils.CommonFunction._Accounting)
                    && !Roles.IsUserInRole(currentUser.UserName, Utils.CommonFunction._AdminRole)
                    && currentUser.AgentBranchID != 0)
                {
                    return;
                }
                if (currentUser.UserName.Equals(profile.UserName))
                {
                    return;
                }
                if (model.ActionMode != (int)Utils.InvoiceAction.ExportInvoice)
                {
                    return;
                }
            }

            var lastestInvoice = db.Invoices.Where(x => x.AgentBranchID == model.AgentBranchID && x.InvoiceType == model.InvoiceType)
                                 .OrderByDescending(x => x.InvoiceNumber).FirstOrDefault();
            var branch = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == model.AgentBranchID);
            if (branch == null)
            {
                ModelState.AddModelError("AgentBranchID", "Bạn chưa chọn chi nhánh.");
                return;
            }
            var invoiceNumber = string.Empty;
            var numberPrefix = branch.InvoicePrefix;

            int seri = 0;

            int start;
            int end;
            if (model.InvoiceType == (int)Utils.InvoiceType.Normal)
            {
                start = branch.InvoiceStartNumber.ConvertToInt();
                end = branch.InvoiceEndNumber.ConvertToInt();
            }
            else
            {
                start = branch.InvoiceReturnStart.ConvertToInt();
                end = branch.InvoiceReturnEnd.ConvertToInt();
            }
            seri = start;
            if (seri >= end)
            {
                ModelState.AddModelError("InvoiceNumber", "Dải hóa đơn được cấp đã sử dụng hết.");
                return;
            }
            string seriFormat = "{0}{1:0000000}";
            if (lastestInvoice != null)
            {
                if (lastestInvoice.InvoiceNumber == null)
                {
                    seri = branch.InvoiceStartNumber.ConvertToInt();
                }
                else if (string.IsNullOrEmpty(numberPrefix))
                {
                    if (seri <= lastestInvoice.InvoiceNumber.ConvertToInt())
                    {
                        seri = lastestInvoice.InvoiceNumber.ConvertToInt() + 1;
                    }
                    else
                    {
                        seri = start;
                    }
                }
                else
                {
                    seri = lastestInvoice.InvoiceNumber.Replace(numberPrefix, "").ConvertToInt() + 1;
                }
            }
            if (seri > end)
            {
                ModelState.AddModelError("InvoiceNumber", "Dải hóa đơn được cấp đã sử dụng hết.");
                return;
            }
            model.InvoiceNumber = string.Format(seriFormat, numberPrefix, seri);

        }

        public ActionResult EditInvoice(string id)
        {
            Session["EditInvoiceID"] = id;
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.Invoices.FirstOrDefault(x => x.InvoiceID == key && x.AgentID == Models.userprofile.CurrentUser.AgentID);
                if (obj != null)
                {
                    obj.InvoiceDetails = db.InvoiceDetails.Where(x => x.InvoiceID == key).ToList();
                    if (obj.InvoiceDetails != null)
                    {
                        obj.InvoiceDetails.ForEach(item =>
                        {
                            item.VCRDisplay = string.Empty;
                        });
                    }
                    Buyers buyer = new Buyers();
                    buyer = buyer.GetByInvoiceID(obj.InvoiceID);
                    obj.Buyer = buyer.Buyer;
                    List<string> listold = new List<string>();
                    foreach (var item in obj.InvoiceDetails as List<InvoiceDetail>)
                    {
                        listold.Add(item.TicketNo);
                    }
                    Session["listold"] = listold;
                    return View(obj);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        private bool ChangeInvoiceInfo(Invoice model)
        {
            using (var db = new InvoiceContext())
            {
                var obj = db.Invoices.FirstOrDefault(x => x.InvoiceID == model.InvoiceID);
                if (obj != null)
                {
                    obj.AgentBranchID = model.AgentBranchID;
                    obj.AgentID = model.AgentID;
                    if (!string.IsNullOrEmpty(model.CustomerAddress))
                        obj.CustomerAddress = model.CustomerAddress.Trim();
                    obj.CustomerID = model.CustomerID;
                    if (!string.IsNullOrEmpty(model.CustomerName))
                        obj.CustomerName = model.CustomerName.Trim();
                    if (!string.IsNullOrEmpty(model.CustomerTax))
                        obj.CustomerTax = model.CustomerTax.Trim();
                    obj.InvoiceDate = model.InvoiceDate;
                    obj.InvoiceNumber = model.InvoiceNumber;
                    obj.InvoiceType = model.InvoiceType;
                    obj.Note = model.Note;
                    obj.Email = model.Email;
                    obj.PaymentType = model.PaymentType;
                    obj.Total = model.Total;
                    obj.TotalAgentFee = model.TotalAgentFee;
                    obj.Pattern = model.Pattern;
                    obj.EInvoiceNumber = model.EInvoiceNumber;
                    if (model.ActionMode == (int)Utils.InvoiceAction.ExportInvoice)
                    {
                        obj.InvoiceStatus = (int)Utils.InvoiceStatus.Exported;
                        model.InvoiceStatus = (int)Utils.InvoiceStatus.Exported;
                        if (string.IsNullOrEmpty(model.InvoiceNumber))
                        {
                            CreateInvoiceNo(db, model);
                            obj.InvoiceNumber = model.InvoiceNumber;
                        }
                        //obj.AgentBranchID = model.AgentBranchID;
                    }
                    var lstOldDetail = db.InvoiceDetails.Where(x => x.InvoiceID == model.InvoiceID).ToList();
                    // cap nhat nhung thang detail cu invoiceID ve ve more truoc
                    lstOldDetail.ForEach(item => item.InvoiceID = 0);

                    if (model.InvoiceDetails != null)
                    {
                        // kiem tra lai xem ve co thuoc hoa don nao chua
                        var arrID = model.InvoiceDetails.Select(x => x.InvoiceDetailID);
                        var check = db.InvoiceDetails.Where(x => x.InvoiceID != null && x.InvoiceID != 0 && x.InvoiceID != model.InvoiceID
                            && arrID.Contains(x.InvoiceDetailID)).Count();
                        if (check > 0)
                        {
                            ModelState.AddModelError("InvoiceID", "Tồn tại vé đã được xuất cho hóa đơn khác.");
                            return false;
                        }
                        var totalPayment = 0;
                        foreach (var item in model.InvoiceDetails)
                        {
                            InvoiceDetail detail = lstOldDetail.FirstOrDefault(x => x.InvoiceDetailID == item.InvoiceDetailID);
                            if (detail == null)
                            {
                                detail = db.InvoiceDetails.FirstOrDefault(x => x.InvoiceDetailID == item.InvoiceDetailID);
                            }
                            if (detail != null)
                            {
                                detail.InvoiceID = model.InvoiceID;
                                detail.AgentBranchID = model.AgentBranchID;
                                //detail.AgentFee = item.AgentFee;
                                //detail.AgentFeeVAT = item.AgentFeeVAT;
                            }
                            totalPayment += item.ChangeLevelFee.ConvertToInt()
                        + item.ChangeLevelFeeVAT.ConvertToInt()
                        + item.OtherFees.ConvertToInt() - item.ReturnFees.ConvertToInt() + item.TicketFare.ConvertToInt()
                        + item.TicketTax.ConvertToInt() + item.TicketVAT.ConvertToInt() + item.TicketTaxGlobal.ConvertToInt();
                            obj.Total = totalPayment;
                        }
                        obj.Total = obj.Total + obj.TotalAgentFee.ConvertToInt();
                        List<string> listold = new List<string>();
                        listold = Session["listold"] as List<string>;
                        if (listold == null)
                        {
                            listold = new List<string>();
                        }
                        List<string> listnew = new List<string>();
                        foreach (var item in model.InvoiceDetails as List<InvoiceDetail>)
                        {
                            listnew.Add(item.TicketNo);
                        }
                        if (listnew == null)
                        {
                            listnew = new List<string>();
                        }
                        List<string> listadd = new List<string>();
                        List<string> listremove = new List<string>();
                        listadd = listnew.Except(listold).ToList();
                        listremove = listold.Except(listnew).ToList();
                        Logs log = new Logs();
                        log = log.GetByInvoiceID(obj.InvoiceID);
                        if (log.InvoiceID <= 0)
                        {
                            log.InvoiceID = model.InvoiceID;
                            if (listadd.Count > 0)
                            {
                                foreach (var item in listadd)
                                {
                                    log.Log = log.Log + item + "-them ve-" + userprofile.CurrentUser.UserName + "-" + Convert.ToString(DateTime.Now) + " | ";
                                }
                            }
                            if (listremove.Count > 0)
                            {
                                foreach (var item in listremove)
                                {
                                    log.Log = log.Log + item + "-xoa ve-" + userprofile.CurrentUser.UserName + "-" + Convert.ToString(DateTime.Now) + " | ";
                                }
                            }
                            if (log.Log != null)
                            {
                                log.Insert();
                            }
                        }
                        else
                        {
                            log.InvoiceID = model.InvoiceID;
                            if (listadd.Count > 0)
                            {
                                foreach (var item in listadd)
                                {
                                    log.Log = log.Log + item + "-them ve-" + userprofile.CurrentUser.UserName + "-" + Convert.ToString(DateTime.Now) + " | ";
                                }
                            }
                            if (listremove.Count > 0)
                            {
                                foreach (var item in listremove)
                                {
                                    log.Log = log.Log + item + "-xoa ve-" + userprofile.CurrentUser.UserName + "-" + Convert.ToString(DateTime.Now) + " | ";
                                }
                            }
                            if (log.Log != null)
                            {
                                log.Update();
                            }
                        }
                    }
                }
                var i = db.SaveChanges();
                return i > 0;
            }
        }

        private bool CancelInvoiceExport(Invoice model)
        {
            using (var db = new InvoiceContext())
            {
                var obj = db.Invoices.FirstOrDefault(x => x.InvoiceID == model.InvoiceID);
                if (obj != null)
                {
                    obj.InvoiceStatus = 0;
                }
                var i = db.SaveChanges();
                return i > 0;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInvoice(Invoice model, FormCollection fc)
        {

            if (ModelState.IsValid)
            {
                var lstDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InvoiceDetail>>(model.InvoiceDetailJson);
                model.InvoiceDetails = lstDetail;
                Buyers buyer = new Buyers();
                buyer = buyer.GetByInvoiceID(model.InvoiceID);
                string Buyer = "";
                Buyer = fc["Buyer"];
                if (model.ActionMode == (int)Utils.InvoiceAction.CancelExport)
                {
                    model = new InvoiceContext().Invoices.FirstOrDefault(x => x.InvoiceID == model.InvoiceID);
                    model.InvoiceDetails = lstDetail;
                    CancelInvoiceExport(model);
                    return RedirectToAction("EditInvoice", new { id = Utils.CommonFunction.Shrink(model.InvoiceID) });
                }
                else if (model.ActionMode == (int)Utils.InvoiceAction.Update || model.ActionMode == (int)Utils.InvoiceAction.ExportInvoice)
                {
                    ChangeInvoiceInfo(model);
                    if (buyer.InvoiceID != 0)
                    {
                        buyer.InvoiceID = model.InvoiceID;
                        buyer.Buyer = string.IsNullOrEmpty(Buyer) ? "" : Buyer;
                        buyer.Update();
                        model.Buyer = Buyer;
                    }
                    else
                    {
                        buyer.InvoiceID = model.InvoiceID;
                        buyer.Buyer = string.IsNullOrEmpty(Buyer) ? "" : Buyer;
                        buyer.Insert();
                        model.Buyer = Buyer;
                    }
                    return RedirectToAction("EditInvoice", new { id = Utils.CommonFunction.Shrink(model.InvoiceID) });
                }
                else return View(model);
            }
            else
            {
                return View(model);
            }
        }

        public JsonResult GetInvoiceDetail(string ticketNo, int invoiceType)
        {
            using (var db = new InvoiceContext())
            {
                string mpatternValue = "";
                string suffixSabre = "";
                string thisSuffixSabre = "";
                bool check = false;
                bool soldtoday = false;
                bool t_void = false;
                bool t_arnk = false;
                var ticket = db.InvoiceDetails.FirstOrDefault(x => x.TicketNo.Equals(ticketNo.Trim()));
                // call service cua vna lấy thông tin vé
                var sv = new InvoiceService.VNAWSSoapClient();
                var agent = db.Agents.FirstOrDefault(x => x.AgentID == Models.userprofile.CurrentUser.AgentID);
                var response = sv.OpenFullTicketNumber(new InvoiceService.Authentication() { Username = "invoice", Password = "idb@@" },
                    ticketNo, agent.SigninSabre, agent.PasscodeSabre, agent.SuffixSabre);

                if (userprofile.CurrentUser.AgentID != 88)
                {
                    if (ticket == null || ticket.SoldDate != response.SoldDate)
                    {
                        ticket = new InvoiceDetail();

                        if (response != null && !string.IsNullOrEmpty(response.TicketNo))
                        {
                            ticket.AgentID = agent.AgentID;
                            ticket.Airline = response.Airline;
                            ticket.ChangeLevelFee = response.ChangeLevelFee;
                            ticket.ChangeLevelFeeVAT = response.ChangeLevelFeeVAT;
                            ticket.ChangeTicket = response.ChangeTicket;
                            ticket.Note = response.Note;
                            ticket.OtherFees = response.OtherFees;
                            ticket.PaxName = response.PaxName;
                            ticket.PNRCode = response.PNRCode;
                            ticket.RealPay = response.RealPay;
                            ticket.ReturnFees = response.ReturnFees;
                            ticket.SoldDate = response.SoldDate;
                            ticket.TicketFare = response.TicketFare;
                            ticket.TicketNo = response.TicketNo;
                            ticket.TicketPrice = response.TicketPrice;
                            ticket.TicketTax = response.TicketTax;
                            ticket.TicketTaxGlobal = response.TicketTaxGlobal;
                            ticket.TicketType = response.TicketType;
                            ticket.TicketVAT = response.TicketVAT;
                            ticket.TripCode = response.TripCode;
                            ticket.UpdateSystem = false;
                            ticket.UpdateTime = DateTime.Now;
                            ticket.VCRDisplay = response.VCRDisplay;

                            db.InvoiceDetails.Add(ticket);
                            db.SaveChanges();

                        }
                    }
                    //VietAn : vé bán trong tháng chỉ thêm được tối đa đến mùng 6 tháng sau
                    if ((Models.userprofile.CurrentUser.AgentID == 1 || Models.userprofile.CurrentUser.AgentID == 82 || Models.userprofile.CurrentUser.AgentID == 88) && !Models.userprofile.CurrentUser.UserName.Contains("admin") && !Models.userprofile.CurrentUser.UserName.Contains("TRANTHU") && !Models.userprofile.CurrentUser.UserName.Contains("VANANH") && !Models.userprofile.CurrentUser.UserName.Contains("TRANTHU2") && !Models.userprofile.CurrentUser.UserName.Contains("DIEUCHI") && !Models.userprofile.CurrentUser.UserName.Contains("HONGTRINH") && !Models.userprofile.CurrentUser.UserName.Contains("HONGTRINH2") && !Models.userprofile.CurrentUser.UserName.Contains("HONGTRINH-VN") && !Models.userprofile.CurrentUser.UserName.Contains("JJZ"))
                    {

                        DateTime ticketdate = new DateTime();
                        ticketdate = (DateTime)ticket.SoldDate;
                        int tmpMonth = 0;
                        int tmpYear = 0;
                        tmpMonth = ticketdate.Month + 1;
                        tmpYear = ticketdate.Year;
                        if (tmpMonth > 12)
                        {
                            tmpMonth = 1;
                            tmpYear = tmpYear + 1;
                        }
                        DateTime enddate = new DateTime(tmpYear, tmpMonth, 5);
                        if (DateTime.Now >= enddate && (Models.userprofile.CurrentUser.AgentID == 1 || Models.userprofile.CurrentUser.AgentID == 82 || Models.userprofile.CurrentUser.AgentID == 88) && (!Models.userprofile.CurrentUser.UserName.Contains("admin") && !Models.userprofile.CurrentUser.UserName.Contains("HONGTRINH-VN") && !Models.userprofile.CurrentUser.UserName.Contains("JJZ") && (!Models.userprofile.CurrentUser.UserName.Contains("lte"))))
                        {
                            check = true;
                        }
                        else
                        {
                            check = false;
                        }
                    }
                    //ve xuat trong ngay

                    DateTime date = Convert.ToDateTime(ticket.SoldDate);
                    if (date.Date == DateTime.Now.Date && (Models.userprofile.CurrentUser.AgentID == 1 || Models.userprofile.CurrentUser.AgentID == 82) && (!Models.userprofile.CurrentUser.UserName.Contains("admin") && (!Models.userprofile.CurrentUser.UserName.Contains("lte"))))
                    {
                        soldtoday = true;
                    }
                    else
                    {
                        soldtoday = false;
                    }

                    //=====================================================================
                    string vcrDisplay = "";
                    vcrDisplay = ticket.VCRDisplay;

                    //Lấy mã PCC trong VCR
                    string pattern = "ISSUED AT-\\w{3}";
                    Regex r_pattern = new Regex(pattern);
                    Match m_pattern = r_pattern.Match(vcrDisplay);
                    mpatternValue = m_pattern.Value;
                    if (ticket.TicketType != 4 && userprofile.CurrentUser.AgentID != 47)
                    {

                        if (m_pattern.Value.Length > 0)
                        {
                            suffixSabre = m_pattern.Value.Substring(10, 3);
                            //Có phải là vé void ?
                            string patternvoid = "VOID";
                            Regex regvoid = new Regex(patternvoid);
                            Match m_void = regvoid.Match(vcrDisplay);
                            //Có phải là vé ARNK ?
                            string patternnarnk = "ARNK";
                            Regex regarnk = new Regex(patternnarnk);
                            Match m_arnk = regarnk.Match(vcrDisplay);
                            t_void = m_void.Success;
                            t_arnk = m_arnk.Success;
                            int agentID = userprofile.CurrentUser.AgentID;
                            thisSuffixSabre = db.Agents.FirstOrDefault(x => x.AgentID == agentID).SuffixSabre;
                            if (!string.IsNullOrEmpty(suffixSabre))
                            {
                                suffixSabre = suffixSabre.Substring(1);
                                suffixSabre = string.Concat("A", suffixSabre);
                            }
                        }

                    }
                }
                ticket.VCRDisplay = string.Empty;
                var message = string.Empty;
                var status = "ok";
                if (ticket == null || string.IsNullOrEmpty(ticket.TicketNo))
                {
                    message = "Không tìm thấy vé này trong hệ thống.";
                    status = "none";
                }
                else if (ticket.InvoiceID > 0) //check
                {
                    message = "Vé đã được xuất tại hóa đơn " + db.Invoices.FirstOrDefault(x => x.InvoiceID == (int)ticket.InvoiceID).InvoiceID + "-" + db.Invoices.FirstOrDefault(x => x.InvoiceID == (int)ticket.InvoiceID).InvoiceNumber + ", xin mời chọn vé khác.";
                    status = "none";
                }
                // viet an chi admin moi xuat ve ban trong ngay
                else if (soldtoday)
                {
                    message = "Không thể xuất vé bán trong ngày";
                    status = "none";
                }

                //Nếu vé của việt an thì chỉ được thêm vào hóa đơn tối đa đến mùng 5 tháng sau
                else if (check)
                {
                    message = "Vé đã quá thời hạn được thêm vào hóa đơn, liên hệ F1 để được trợ giúp";
                    status = "none";
                }

                //Nếu không phải vé của đại lý thì không được thêm vào hóa đơn
                else if (ticket.TicketType != 4 && userprofile.CurrentUser.AgentID != 47 && (suffixSabre != thisSuffixSabre && !string.IsNullOrEmpty(suffixSabre) && mpatternValue.Length > 0))
                {
                    message = "Vé không thuộc đại lý bạn đang đăng nhập.";
                    status = "none";
                }
                //else if (ticket.AgentID != userprofile.CurrentUser.AgentID && userprofile.CurrentUser.AgentID != 0)
                //{
                //    message = "Vé không thuộc đại lý bạn đang đăng nhập.";
                //    status = "none";
                //}
                //Nếu trong VCR có VOID thì không cho thêm vé vào hóa đơn

                else if ((ticket.TicketType != 4) && (t_void && !t_arnk))
                {
                    message = "Không thể thêm vé VOID vào hóa đơn";
                    status = "none";
                }
                //nếu số vé từ năm 2015 thì không cho thêm vào hóa đơn
                else if (ticket.SoldDate < Convert.ToDateTime("2015-12-31") && ticket.AgentID != 74 && ticket.AgentID != 43 && ticket.AgentID != 1 && ticket.AgentID != 47)
                {
                    message = "Không thể thêm vé từ năm 2015.";
                    status = "none";
                }

                //====================================================
                else if (ticket != null)
                {
                    if (invoiceType == (int)Utils.InvoiceType.Normal)
                    {
                        if (ticket.TicketType == (int)Utils.TicketType.Return)
                        {
                            message = string.Format("Vé {0} không phải vé thuộc hóa đơn xuất.", ticket.TicketNo);
                            status = "none";
                        }
                    }
                    else if (invoiceType == (int)Utils.InvoiceType.Return)
                    {
                        if (ticket.TicketType != (int)Utils.TicketType.Return)
                        {
                            message = string.Format("Vé {0} không phải vé thuộc hóa đơn hoàn.", ticket.TicketNo);
                            status = "none";
                        }
                    }
                }
                return Json(new
                {
                    id = ticket != null ? ticket.InvoiceDetailID : 0,
                    result = Utils.CommonFunction.RenderViewToString(this.ControllerContext,
                    "AddNewDetailRow", ticket),
                    status = status,
                    message = message,
                    updatesystem = ticket.UpdateSystem.HasValue && ticket.UpdateSystem.Value ? 1 : 0,
                    jsontext = Newtonsoft.Json.JsonConvert.SerializeObject(ticket)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetInvoiceDetailMulti(List<InvoiceDetail> details)
        {
            if (details == null) details = new List<InvoiceDetail>();
            details.ForEach(x => x.VCRDisplay = string.Empty);
            var message = string.Empty;
            var status = "ok";
            if (details.Count == 0)
            {
                message = "Không tìm thấy vé trong hệ thống.";
                status = "none";
            }

            return Json(new
            {
                result = Utils.CommonFunction.RenderViewToString(this.ControllerContext,
                "AddNewDetailRow", details),
                status = status,
                message = message,
                jsontext = Newtonsoft.Json.JsonConvert.SerializeObject(details)
            }, JsonRequestBehavior.AllowGet);
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

        #endregion

        #region report
        public ActionResult ViewReport()
        {
            //test

            //
            return View();
        }

        private XtraReport CreateReport(int id, int reportcopy, int PrintAction)
        {
            using (var db = new Models.InvoiceContext())
            {
                XtraReport report = new XtraReport();
                //report.SaveLayout(Server.MapPath("~/AgentReport/") + "95_Report.REPX");
                var invoice = db.Invoices.FirstOrDefault(x => x.InvoiceID == id);
                if (invoice != null)
                {

                    if (reportcopy == 1)
                    {
                        invoice.PrintCopyString = "Liên 1 : Lưu (Copy 1 for Filing)";
                    }
                    else if (reportcopy == 2)
                    {
                        invoice.PrintCopyString = "Liên 2 : Giao cho người mua (Copy 2 for Buyer)";
                    }
                    else if (reportcopy == 3)
                    {
                        invoice.PrintCopyString = "Liên 3 : Nội bộ (Copy 3 for internal)";
                    }
                    //ngay hoa don in ra hien thi ngay trong thang, neu la mung 1 den mung 5 thang sau thi hien thi ngay cuoi cung cua thang truoc
                    //if ((Models.userprofile.CurrentUser.AgentID == 1 || Models.userprofile.CurrentUser.AgentID == 82))
                    //{
                    //    DateTime today = DateTime.Now;
                    //    DateTime finalday = new DateTime();
                    //    if (today.Day >= 1 && today.Day <=5)
                    //    {
                    //        finalday = today.AddMonths(-1);
                    //        finalday = new DateTime(finalday.Year,finalday.Month, DateTime.DaysInMonth(finalday.Year, finalday.Month));
                    //        invoice.InvoiceDate = finalday;
                    //    }
                    //    else
                    //    {
                    //        invoice.InvoiceDate = today;
                    //    }
                    //}

                    invoice.DetailListLabel = "(Kèm theo hóa đơn số (refer to the invoice no): " + invoice.InvoiceNumber + " ngày (date) " + invoice.InvoiceDate.Day + " tháng (month) " + invoice.InvoiceDate.Month + " năm (year) " + invoice.InvoiceDate.Year + " )";


                    invoice.InvoiceDetails = db.InvoiceDetails.Where(x => x.InvoiceID == id).ToList();
                    Agents agt = new Agents();
                    agt = agt.GetByID(Models.userprofile.CurrentUser.AgentID);
                    invoice.AgentTel = agt.Tel;
                    invoice.AgentEmail = agt.Email;

                    Buyers buyer = new Buyers();
                    buyer = buyer.GetByInvoiceID(invoice.InvoiceID);
                    invoice.Buyer = buyer.Buyer;
                    var branch = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == invoice.AgentBranchID);
                    if (branch != null)
                    {
                        invoice.BranchName = branch.BranchName;
                        invoice.BranchAddress = branch.Address;
                        if (invoice.InvoiceType == (int)Utils.InvoiceType.Normal)
                        {
                            invoice.FormNo = branch.FormNo;
                            invoice.Serial = branch.Serial;
                        }
                        else
                        {
                            invoice.FormNo = branch.FormNoReturn;
                            invoice.Serial = branch.SerialReturn;

                        }
                        invoice.BranchTax = branch.Tax;
                    }
                    var user = Models.userprofile.GetUser(new Guid(invoice.UserCreate));
                    if (user != null)
                    {
                        invoice.Seller = user.FullName;
                    }
                    var targetFolder = Server.MapPath("~/AgentReport");
                    var agentID = Models.userprofile.CurrentUser.AgentID;
                    var fileNameInvoice = "{0}_Report{1}.repx";
                    var reportTypeString = string.Empty;

                    if (PrintAction == (int)Utils.InvoiceAction.PrintInvoiceDetail)
                    {
                        fileNameInvoice = string.Format(fileNameInvoice, agentID, "Detail");
                    }
                    else if (invoice.InvoiceType == (int)Utils.InvoiceType.Return)
                    {
                        // ve hoan tra
                        fileNameInvoice = string.Format(fileNameInvoice, agentID, "Return");
                        if (invoice.AgentID == 1 || invoice.AgentID == 47 || invoice.AgentID == 6 || invoice.AgentID == 82 || invoice.AgentID == 88)
                        {
                            if (invoice.InvoiceDetails.Count == 1)
                            {
                                invoice.InvoiceDetails[0].STT = "1";
                                invoice.InvoiceDetails[0].SoLuong = "1";
                                invoice.InvoiceDetails[0].DonVi = "Vé";
                                invoice.InvoiceDetails[0].Currency = "đ";
                                invoice.InvoiceDetails.Add(new InvoiceDetail { STT = " ", SoLuong = "  ", DonVi = "  ", Currency = "  " });
                                invoice.InvoiceDetails.Add(new InvoiceDetail { STT = " ", SoLuong = "  ", DonVi = "  ", Currency = "  " });
                            }
                            else if (invoice.InvoiceDetails.Count == 2)
                            {
                                invoice.InvoiceDetails[0].STT = "1";
                                invoice.InvoiceDetails[0].SoLuong = "1";
                                invoice.InvoiceDetails[0].DonVi = "Vé";
                                invoice.InvoiceDetails[0].Currency = "đ";
                                invoice.InvoiceDetails[1].STT = "2";
                                invoice.InvoiceDetails[1].SoLuong = "1";
                                invoice.InvoiceDetails[1].DonVi = "Vé";
                                invoice.InvoiceDetails[1].Currency = "đ";
                                invoice.InvoiceDetails.Add(new InvoiceDetail { STT = " ", SoLuong = "  ", DonVi = "  ", Currency = "  " });
                            }
                            else if (invoice.InvoiceDetails.Count == 3)
                            {
                                invoice.InvoiceDetails[0].STT = "1";
                                invoice.InvoiceDetails[0].SoLuong = "1";
                                invoice.InvoiceDetails[0].DonVi = "Vé";
                                invoice.InvoiceDetails[0].Currency = "đ";
                                invoice.InvoiceDetails[1].STT = "2";
                                invoice.InvoiceDetails[1].SoLuong = "1";
                                invoice.InvoiceDetails[1].DonVi = "Vé";
                                invoice.InvoiceDetails[1].Currency = "đ";
                                invoice.InvoiceDetails[2].STT = "3";
                                invoice.InvoiceDetails[2].SoLuong = "1";
                                invoice.InvoiceDetails[2].DonVi = "Vé";
                                invoice.InvoiceDetails[2].Currency = "đ";
                            }
                            else if (invoice.InvoiceDetails.Count >= 4)
                            {
                                InvoiceDetail ids = new InvoiceDetail();
                                ids.TicketFare = invoice.InvoiceDetails.Sum(x => x.TicketFare);
                                ids.TicketVAT = invoice.InvoiceDetails.Sum(x => x.TicketVAT);
                                ids.TicketTax = invoice.InvoiceDetails.Sum(x => x.TicketTax);
                                ids.TicketTaxGlobal = invoice.InvoiceDetails.Sum(x => x.TicketTaxGlobal);
                                ids.ReturnFees = invoice.InvoiceDetails.Sum(x => x.ReturnFees);
                                ids.ChangeLevelFee = invoice.InvoiceDetails.Sum(x => x.ChangeLevelFee);
                                ids.ChangeLevelFeeVAT = invoice.InvoiceDetails.Sum(x => x.ChangeLevelFeeVAT);
                                ids.OtherFees = invoice.InvoiceDetails.Sum(x => x.OtherFees);
                                ids.SoLuong = "  ";
                                ids.DonVi = "  ";
                                ids.Currency = "đ";
                                ids.TicketNo = "Xem bảng kê kèm theo";
                                ids.ChangeTicket = "Xem bảng kê kèm theo";

                                invoice.InvoiceDetails = new List<InvoiceDetail>();

                                invoice.InvoiceDetails.Add(ids);
                                invoice.InvoiceDetails.Add(new InvoiceDetail { SoLuong = "  ", DonVi = "  ", Currency = "  " });
                                invoice.InvoiceDetails.Add(new InvoiceDetail { SoLuong = "  ", DonVi = "  ", Currency = "  " });
                            }
                        }
                    }
                    else
                    {
                        // ve noi dia, ve quoc te
                        fileNameInvoice = string.Format(fileNameInvoice, agentID, string.Empty);

                        if (invoice.AgentID == 1 || invoice.AgentID == 47 || invoice.AgentID == 6 || invoice.AgentID == 82 || invoice.AgentID == 88 || invoice.AgentID == 91)
                        {
                            if (invoice.InvoiceDetails.Count == 1)
                            {
                                invoice.InvoiceDetails[0].SoLuong = "1";
                                invoice.InvoiceDetails[0].STT = "1";
                                invoice.InvoiceDetails[0].DonVi = "Vé";
                                invoice.InvoiceDetails[0].Currency = "đ";
                                invoice.InvoiceDetails.Add(new InvoiceDetail { STT = "  ", SoLuong = "  ", DonVi = "  ", Currency = "  " });
                                invoice.InvoiceDetails.Add(new InvoiceDetail { STT = "  ", SoLuong = "  ", DonVi = "  ", Currency = "  " });
                            }
                            else if (invoice.InvoiceDetails.Count == 2)
                            {
                                invoice.InvoiceDetails[0].SoLuong = "1";
                                invoice.InvoiceDetails[0].STT = "1";
                                invoice.InvoiceDetails[0].DonVi = "Vé";
                                invoice.InvoiceDetails[0].Currency = "đ";

                                invoice.InvoiceDetails[1].STT = "2";
                                invoice.InvoiceDetails[1].SoLuong = "1";
                                invoice.InvoiceDetails[1].DonVi = "Vé";
                                invoice.InvoiceDetails[1].Currency = "đ";
                                invoice.InvoiceDetails.Add(new InvoiceDetail { STT = "  ", SoLuong = "  ", DonVi = "  ", Currency = "  " });
                            }
                            else if (invoice.InvoiceDetails.Count == 3)
                            {
                                invoice.InvoiceDetails[0].SoLuong = "1";
                                invoice.InvoiceDetails[0].STT = "1";
                                invoice.InvoiceDetails[0].DonVi = "Vé";
                                invoice.InvoiceDetails[0].Currency = "đ";
                                invoice.InvoiceDetails[1].SoLuong = "1";
                                invoice.InvoiceDetails[1].STT = "2";
                                invoice.InvoiceDetails[1].DonVi = "Vé";
                                invoice.InvoiceDetails[1].Currency = "đ";
                                invoice.InvoiceDetails[2].SoLuong = "1";
                                invoice.InvoiceDetails[2].STT = "3";
                                invoice.InvoiceDetails[2].DonVi = "Vé";
                                invoice.InvoiceDetails[2].Currency = "đ";
                            }
                            else if (invoice.InvoiceDetails.Count >= 4)
                            {
                                InvoiceDetail ids = new InvoiceDetail();
                                ids.TicketFare = invoice.InvoiceDetails.Sum(x => x.TicketFare);
                                ids.TicketVAT = invoice.InvoiceDetails.Sum(x => x.TicketVAT);
                                ids.TicketTax = invoice.InvoiceDetails.Sum(x => x.TicketTax);
                                ids.TicketTaxGlobal = invoice.InvoiceDetails.Sum(x => x.TicketTaxGlobal);
                                ids.ReturnFees = invoice.InvoiceDetails.Sum(x => x.ReturnFees);
                                ids.ChangeLevelFee = invoice.InvoiceDetails.Sum(x => x.ChangeLevelFee);
                                ids.ChangeLevelFeeVAT = invoice.InvoiceDetails.Sum(x => x.ChangeLevelFeeVAT);
                                ids.OtherFees = invoice.InvoiceDetails.Sum(x => x.OtherFees);
                                ids.SoLuong = "  ";
                                ids.DonVi = "  ";
                                ids.Currency = "đ";
                                ids.STT = "  ";
                                ids.TicketNo = "Xem bảng kê kèm theo";

                                invoice.InvoiceDetails = new List<InvoiceDetail>();

                                invoice.InvoiceDetails.Add(ids);
                                invoice.InvoiceDetails.Add(new InvoiceDetail { STT = "  ", SoLuong = "  ", DonVi = "  ", Currency = "  " });
                                invoice.InvoiceDetails.Add(new InvoiceDetail { STT = "  ", SoLuong = "  ", DonVi = "  ", Currency = "  " });
                            }
                        }

                    }
                    if (System.IO.File.Exists(Path.Combine(targetFolder, fileNameInvoice)))
                    {
                        report.LoadLayout(Path.Combine(targetFolder, fileNameInvoice));
                    }
                    else
                    {
                        if (PrintAction == (int)Utils.InvoiceAction.PrintInvoiceDetail)
                        {

                            List<InvoiceDetail> lid = db.InvoiceDetails.Where(x => x.InvoiceID == id).ToList();

                            lid.ForEach(x => x.TicketTax = (x.TicketTax == null ? (0 + (x.TicketTaxGlobal == null ? 0 : x.TicketTaxGlobal)) : (x.TicketTax + (x.TicketTaxGlobal == null ? 0 : x.TicketTaxGlobal))));

                            invoice.InvoiceDetails = lid;
                            report = new ReportFiles.ReportDetail(reportcopy);
                        }
                        else if (invoice.InvoiceType == (int)Utils.InvoiceType.Return)
                        {
                            // ve hoan tra
                            report = new ReportFiles.ReturnReport(reportcopy);
                        }
                        else
                        {
                            report = new ReportFiles.NormalReport(reportcopy);
                        }
                    }
                }


                if (report != null)
                {
                    report.Parameters.Add(new DevExpress.XtraReports.Parameters.Parameter()
                    {
                        Name = "Copy",
                        Value = reportcopy
                    });

                    report.DataSource = new System.Collections.Generic.List<Models.Invoice>() { invoice };
                }
                return report;
            }
        }

        //Xuất hóa đơn VNPT
        #region VNPT
        [Serializable]
        public class ImportInv {
            public xmlInvData xmlInvData { get; set; } = new xmlInvData();
            public string username { get; set; }
            public string password { get; set; }
            public int convert { get; set; } = 0;
        };
        [Serializable]
        public class xmlInvData { public Invoices Invoices { get; set; } = new Invoices(); };
        [Serializable]
        public class Invoices { public Inv Inv { get; set; } = new Inv(); };
        [Serializable]
        public class Inv { public string key { get; set; }
            public Models.EInvoiceVNPT.Invoice Invoice { get; set; } = new Models.EInvoiceVNPT.Invoice(); };
        public string CreateReportEInvoice(int InvoiceId)
        {
            using (var db = new Models.InvoiceContext())
            {
                string message = "";
                XtraReport report = new XtraReport();
                Models.EInvoiceVNPT.Invoice eInvoice = new Models.EInvoiceVNPT.Invoice();
                string messageRespone = "";
                var invoice = db.Invoices.FirstOrDefault(x => x.InvoiceID == InvoiceId);
                if (invoice != null)
                {
                    invoice.InvoiceDetails = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceId).ToList();
                    Agents agt = new Agents();
                    agt = agt.GetByID(Models.userprofile.CurrentUser.AgentID);
                    invoice.AgentTel = agt.Tel;
                    invoice.AgentEmail = agt.Email;
                    var branch = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == invoice.AgentBranchID);
                    string buyer = new Buyers().GetByInvoiceID(invoice.InvoiceID).Buyer;
                    if (branch != null)
                    {
                        invoice.BranchName = branch.BranchName;
                        invoice.BranchAddress = branch.Address;
                        if (invoice.InvoiceType == (int)Utils.InvoiceType.Normal)
                        {
                            invoice.FormNo = branch.FormNo;
                            invoice.Serial = branch.Serial;
                        }
                        else
                        {
                            invoice.FormNo = branch.FormNoReturn;
                            invoice.Serial = branch.SerialReturn;

                        }
                        invoice.BranchTax = branch.Tax;
                    }
                    var user = Models.userprofile.GetUser(new Guid(invoice.UserCreate));
                    if (user != null)
                    {
                        invoice.Seller = user.FullName;
                    }
                    //string ProductItem = "";
                    if (invoice.InvoiceDetails.Count > 0)
                    {
                        foreach (var item in invoice.InvoiceDetails)
                        {
                            Models.EInvoiceVNPT.Product newProducts = new Models.EInvoiceVNPT.Product();
                            newProducts.ProdName = item.TicketNo + "/" + item.TripCode;
                            newProducts.ProdPrice = (int)item.TicketFare;
                            newProducts.ProdUnit = "vé";
                            newProducts.ProdQuantity = 1;
                            newProducts.Total = (int)item.TicketFare;
                            newProducts.VATRate = ((int)((((double)item.TicketVAT / item.TicketFare)) * 100) > 0 ? 10 : 0);
                            newProducts.VATAmount = (int)item.TicketVAT;
                            newProducts.Extra2 = (int)item.TicketTax + (int)item.TicketTaxGlobal + (int)item.OtherFees + (int)item.ChangeLevelFee + ((int)item.ChangeLevelFee * (((int)item.ChangeLevelFeeVAT) / 100)) + (int)item.AgentFee + ((int)item.AgentFee * (((int)item.AgentFeeVAT) / 100));
                            newProducts.Amount = (double)item.TicketPrice;
                            eInvoice.Products.Insert(eInvoice.Products.Count, newProducts);
                        }
                        eInvoice.CusName = invoice.CustomerName != null ? invoice.CustomerName : branch.BranchName;
                        eInvoice.Buyer = invoice.Buyer != null ? invoice.Buyer : new Buyers().GetByInvoiceID(invoice.InvoiceID).Buyer;
                        eInvoice.CusPhone = branch.Tel != null ? branch.Tel : invoice.AgentTel;
                        eInvoice.EmailDeliver = invoice.Email;
                        eInvoice.CusCode = "KH" + branch.AgentID;
                        eInvoice.CusAddress = invoice.CustomerAddress != null ? invoice.CustomerAddress : (branch.Address != null ? branch.Address : "");
                        eInvoice.CusTaxCode = invoice.CustomerTax != null ? invoice.CustomerTax : (branch.Tax != null ? branch.Tax : "");
                        eInvoice.PaymentMethod = Convert.ToString(AirlineInvoice.Utils.DataComboSource.PaymentTypeList().Where(x => x.Value == "3").FirstOrDefault().Text);
                        eInvoice.KindOfService = "TM, CK";
                        eInvoice.DiscountAmount = 0;
                        eInvoice.PaymentStatus = "";
                        eInvoice.Extra1 = 0;
                        eInvoice.VATAmount = 0;
                        if (invoice.TotalAgentFee != null && invoice.TotalAgentFee != 0)
                        {
                            eInvoice.Extra7 = (double)invoice.Total - (double)invoice.TotalAgentFee;
                            eInvoice.Total = (double)invoice.Total - (double)invoice.TotalAgentFee;
                            eInvoice.Extra = (double)invoice.TotalAgentFee - Math.Round(Convert.ToDouble(invoice.TotalAgentFee / 11), 0);
                            eInvoice.Extra4 = (int)(((double)invoice.TotalAgentFee - Math.Round(Convert.ToDouble(invoice.TotalAgentFee / 11), 0)) / Math.Round(Convert.ToDouble(invoice.TotalAgentFee / 11), 0));
                            eInvoice.Extra5 = Math.Round(Convert.ToDouble(invoice.TotalAgentFee / 11), 0);
                            eInvoice.Extra6 = (double)invoice.TotalAgentFee;
                        }
                        else
                        {
                            eInvoice.Extra7 = (double)invoice.Total;
                            eInvoice.Total = (double)invoice.Total;
                            eInvoice.Extra = 0;
                            eInvoice.Extra4 = 0;
                            eInvoice.Extra5 = 0;
                            eInvoice.Extra6 = 0;
                        }
                        eInvoice.Amount = (double)(invoice.Total);
                        eInvoice.AmountInWords = Utils.CommonFunction.SayMoney(Convert.ToInt32(invoice.Total));
                        AgentElectronicBill InfoElecAgent = new AgentElectronicBill();
                        if (ViewBag.InfoElecAgent != null) { InfoElecAgent = ViewBag.InfoElecAgent; }
                        else
                        {
                            string readText = System.IO.File.ReadAllText(Server.MapPath("~/Config/AgentInfoVNPT.ini"));
                            List<AgentElectronicBill> L_InfoElecAgent = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AgentElectronicBill>>(readText);
                            InfoElecAgent = L_InfoElecAgent.Where(x => x.AgentId == invoice.AgentID).FirstOrDefault();
                        }
                        Invoices EImportInv = new Invoices();
                        EImportInv.Inv.Invoice = eInvoice;
                        EImportInv.Inv.key = invoice.InvoiceID.ToString();
                        XmlSerializer xsSubmit = new XmlSerializer(typeof(Invoices));
                        System.IO.StringWriter sww = new System.IO.StringWriter();
                        xsSubmit.Serialize(sww, EImportInv);
                        var xmlEInvoiceData = sww.ToString();
                        string result = Regex.Replace(xmlEInvoiceData, @"<(.*?)\s* />", @"<$1></$1>");
                        //result = result.Replace("\r\n", "");                                                  

                        //messageRespone = VNPTEInvoiceSignToken.PublishInvWithToken(InfoElecAgent.Account, InfoElecAgent.ACpass, result, InfoElecAgent.username, InfoElecAgent.password, InfoElecAgent.serialCert, InfoElecAgent.pattern, InfoElecAgent.serial, InfoElecAgent.linkWS);
                        //messageRespone = new VNPT_EInvoice.PublishServiceSoapClient().ImportInvWithPattern(result, InfoElecAgent.username, InfoElecAgent.password,InfoElecAgent.pattern ,InfoElecAgent.serial, 0);
                        messageRespone = new VNPT_EInvoice.PublishServiceSoapClient().getHashInvWithToken(InfoElecAgent.Account, InfoElecAgent.ACpass, result, InfoElecAgent.username, InfoElecAgent.password, InfoElecAgent.serialCert, 0, "", InfoElecAgent.pattern, InfoElecAgent.serial, 0);
                        messageRespone = messageRespone.Replace("<Invoices><Inv>", "<Invpublish>");                              //ImportInvWithPattern
                        messageRespone = messageRespone.Replace("</Inv></Invoices>", "</Invpublish>");
                         var invPublish = Deserialize<Invpublish>(messageRespone);
                        //messageRespone.Contains("OK:")
                        if (invPublish.idInv != null || invPublish.idInv != "")                                    
                        {
                            invoice.InvoiceStatus = 1;
                            invoice.Pattern = invPublish.pattern; 
                            invoice.EInvoiceNumber = invPublish.idInv;
                            // invoice.EInvoiceNumber = messageRespone.Substring
                            db.SaveChanges();
                            message = "Đã cập nhật trên hóa đơn điện tử: Mẫu: " + invoice.Pattern + " Số hóa đơn: " + invPublish.idInv;
                        }
                        else { string errmessage = AirlineInvoice.Utils.CommonFunction.ParseERREInvoiceVNPT(messageRespone); message = "Chưa cập nhật được hóa đơn trên hệ thông, vui lòng liên hệ kỹ thuật <br/>" + errmessage; }
                    }
                }

                return message;
            }
        }
        [Serializable]
        public class Invpublish
        { 
            public string key { get; set; }
            public string idInv { get; set; }
            public string hashValue { get; set; }
            public string pattern { get; set; }
        };
        #endregion

        //Xuất hóa đơn ONFINANCE
        #region ONFINANCE
        public string GetTokken()
        {
            AgentElectronicBill InfoElecAgent = new AgentElectronicBill();
            if (ViewBag.InfoElecAgent != null) { InfoElecAgent = ViewBag.InfoElecAgent; }
            else
            {
                string readText = System.IO.File.ReadAllText(Server.MapPath("~/Config/AgentInfoVNPT.ini"));
                List<AgentElectronicBill> L_InfoElecAgent = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AgentElectronicBill>>(readText);
                InfoElecAgent = L_InfoElecAgent.Where(x => x.AgentId == Models.userprofile.CurrentUser.AgentID).FirstOrDefault();
            }
            Models.EInvoiceONFINANCE.GetTokken tokkenInfo = new Models.EInvoiceONFINANCE.GetTokken();
            tokkenInfo.appId = InfoElecAgent.appId;
            tokkenInfo.taxCode = InfoElecAgent.taxCode;
            WebClient clientag = new WebClient();
            clientag.Headers[HttpRequestHeader.ContentType] = "application/json";
            clientag.Headers[HttpRequestHeader.Accept] = "*/*";
            clientag.Encoding = System.Text.Encoding.UTF8;
            string requestData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(tokkenInfo);
            string res = clientag.UploadString(InfoElecAgent.LinkONFINANCE + "gettoken", "POST", requestData);
            JObject jo = JObject.Parse(res);
            string RawData = "";
            JObject match = (JObject)jo["token"];
             if(match != null)
            {
                foreach (JProperty prop in match.Properties())
                {
                    if(prop.Name == "ValidFrom") {  ViewBag.OnfinanceStartTokken = prop.Value; }
                    if(prop.Name == "ValidTo") {  ViewBag.OnfinanceEndTokken = prop.Value; }
                    if(prop.Name == "RawData") { RawData = prop.Value.ToString(); }
                    Console.WriteLine(prop.Name + ": " + prop.Value);
                }
            }
            return RawData;
        }
        public string PostCreateListInvoice(int InvoiceId)
        {
            AgentElectronicBill InfoElecAgent = new AgentElectronicBill();
            if (ViewBag.InfoElecAgent != null) { InfoElecAgent = ViewBag.InfoElecAgent; }
            else
            {
                string readText = System.IO.File.ReadAllText(Server.MapPath("~/Config/AgentInfoVNPT.ini"));
                List<AgentElectronicBill> L_InfoElecAgent = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AgentElectronicBill>>(readText);
                InfoElecAgent = L_InfoElecAgent.Where(x => x.AgentId == Models.userprofile.CurrentUser.AgentID).FirstOrDefault();
            }
            string res = "";
            string resCode = "";
            using (var db = new Models.InvoiceContext())
            {
                string message = "";
                Models.EInvoiceONFINANCE.Invoice eInvoice = new Models.EInvoiceONFINANCE.Invoice();
                //string messageRespone = "";
                var invoice = db.Invoices.FirstOrDefault(x => x.InvoiceID == InvoiceId);
                if (invoice != null)
                {
                    invoice.InvoiceDetails = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceId).ToList();
                    Agents agt = new Agents();
                    agt = agt.GetByID(Models.userprofile.CurrentUser.AgentID);
                    invoice.AgentTel = agt.Tel;
                    invoice.AgentEmail = agt.Email;
                    var branch = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == invoice.AgentBranchID);
                    string buyer = new Buyers().GetByInvoiceID(invoice.InvoiceID).Buyer;
                    if (branch != null)
                    {
                        invoice.BranchName = branch.BranchName;
                        invoice.BranchAddress = branch.Address;
                        if (invoice.InvoiceType == (int)Utils.InvoiceType.Normal)
                        {
                            invoice.FormNo = branch.FormNo;
                            invoice.Serial = branch.Serial;
                        }
                        else
                        {
                            invoice.FormNo = branch.FormNoReturn;
                            invoice.Serial = branch.SerialReturn;

                        }
                        invoice.BranchTax = branch.Tax;
                    }
                    var user = Models.userprofile.GetUser(new Guid(invoice.UserCreate));
                    if (user != null)
                    {
                        invoice.Seller = user.FullName;
                    }

                    eInvoice.COMNAME = agt.AgentName;
                    eInvoice.COMTAXCODE = InfoElecAgent.taxCode;
                    eInvoice.COMADDRESS = agt.AgentCompanyAddress !=null ? agt.AgentCompanyAddress : "";
                    eInvoice.FORMCODE = "01GTKT0/004";
                    eInvoice.SYMBOLCODE = "VA/20E";
                    if(invoice.InvoiceType == 2)
                    {
                        eInvoice.FORMCODE = "01GTKT0/003";
                        eInvoice.SYMBOLCODE = "VA/20E";
                    }
                    eInvoice.CUSNAME = invoice.CustomerName != null ? invoice.CustomerName : branch.BranchName;
                    eInvoice.CUSADDRESS = invoice.CustomerAddress != null ? invoice.CustomerAddress : (branch.Address != null ? branch.Address : "");
                    eInvoice.CUSBUYER = invoice.Buyer != null ? invoice.Buyer : new Buyers().GetByInvoiceID(invoice.InvoiceID).Buyer;
                    eInvoice.CUSEMAIL = invoice.Email != null ? invoice.Email : "";
                    eInvoice.CUSPHONENUMBER = branch.Tel != null ? branch.Tel : (invoice.AgentTel != null ? invoice.AgentTel : "");
                    eInvoice.CUSTAXCODE = invoice.CustomerTax != null ? invoice.CustomerTax : (branch.Tax != null ? branch.Tax : "");
                    eInvoice.CUSPAYMENTMETHOD = Convert.ToString(AirlineInvoice.Utils.DataComboSource.PaymentTypeList().Where(x => x.Value == "3").FirstOrDefault().Text);
                    eInvoice.CUSACCOUNTNUMBER = "";
                    eInvoice.CUSBANKNAME = "";
                    eInvoice.DUEDATE = (DateTime.Now).ToString("yyyy-MM-dd");
                    eInvoice.DISCOUNTTYPE = "";
                    eInvoice.NOTE = "";
                    eInvoice.CREATEDBY = "apiuser";
                    eInvoice.IPADDRESS = Request.UserHostAddress;
                    eInvoice.SIGNEDTIME = (DateTime.Now).ToString("yyyy-MM-dd");
                    eInvoice.INITTIME = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                    eInvoice.INVOICECODE = "KH" + branch.AgentID;
                    if (invoice.InvoiceDetails.Count > 0)
                    {
                        foreach (var item in invoice.InvoiceDetails)
                        {
                            Models.EInvoiceONFINANCE.Product newProducts = new Models.EInvoiceONFINANCE.Product();
                            newProducts.SKU = "";
                            newProducts.PRODUCTNAME = item.TicketNo + "/" + item.TripCode;
                            newProducts.RETAILPRICE = (int)item.TicketFare;
                            newProducts.SALEPRICE = (int)item.TicketFare;
                            newProducts.QUANTITYUNIT = "vé";
                            newProducts.QUANTITY = 1;
                            newProducts.TAXRATE = ((int)((((double)item.TicketVAT / item.TicketFare)) * 100) > 0 ? 10 : 0);
                            newProducts.TOTALTAX = (int)item.TicketVAT;
                            newProducts.OTHERTAXFEE = (int)item.TicketTax + (int)item.TicketTaxGlobal + (int)item.OtherFees + (int)item.ChangeLevelFee + ((int)item.ChangeLevelFee * (((int)item.ChangeLevelFeeVAT) / 100)) + (int)item.AgentFee + ((int)item.AgentFee * (((int)item.AgentFeeVAT) / 100));
                            newProducts.ISPROMOTION = false;
                            newProducts.DISCOUNTRATE = 0;
                            newProducts.REFUNDFEE = item.ReturnFees != null ? (float)(item.ReturnFees) : 0;
                            eInvoice.LISTPRODUCT.Insert(eInvoice.LISTPRODUCT.Count, newProducts);
                        }
                    }
                    message = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(eInvoice);
                }
                message = "[" + message + "]";
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "*/*";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + GetTokken();
                client.Encoding = System.Text.Encoding.UTF8;
                client.Credentials = CredentialCache.DefaultCredentials;
                res = client.UploadString(InfoElecAgent.LinkONFINANCE + "/postcreatelistinvoice", "POST", message);

                var rss = JObject.Parse(res);

                var rssInvoiceNumber = rss["InvoiceCreated"];
                var rssCode = rss["Code"].ToString();

                if (rssCode != null && rssCode == "0")
                {
                    invoice.InvoiceStatus = 1;
                    invoice.Pattern = eInvoice.SYMBOLCODE; 
                    invoice.EInvoiceNumber = rssInvoiceNumber.ToString().Replace("[\r\n  ", "").Replace("\r\n]", "");
                    // invoice.EInvoiceNumber = messageRespone.Substring
                    db.SaveChanges();
                    message = "Đã cập nhật trên hóa đơn điện tử ONFINANCE: Mẫu: " + eInvoice.SYMBOLCODE + " Số hóa đơn: " + rssInvoiceNumber;
                }
            }
               
            return res;
        }
        public T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }
        #endregion
        public ActionResult NormalReportPartial(string invoiceid, int reportcopy, int PrintAction)
        {
            XtraReport report = null;
            var key = Utils.CommonFunction.Expand(invoiceid);
            report = CreateReport(key, reportcopy, PrintAction);
            return PartialView("ReportPartial", report);
        }

        public ActionResult PhieuCanXePartialExport(string invoiceid, int reportcopy, int PrintAction)
        {
            var key = Utils.CommonFunction.Expand(invoiceid);
            XtraReport report = CreateReport(key, reportcopy, PrintAction);
            return ReportViewerExtension.ExportTo(report);
        }
        #endregion

        #region DBOption
        public ActionResult Config()
        {
            using (var db = new InvoiceContext())
            {
                var lstOption = db.DBOptions.ToList();
                return View(lstOption);
            }
        }
        #endregion


    }
}
