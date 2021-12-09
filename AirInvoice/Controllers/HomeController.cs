using AirlineInvoice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace AirlineInvoice.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            using (var db = new Models.InvoiceContext())
            {
                
                
                if (Models.userprofile.CurrentUser!=null)
                {
                    int Dometic = 0;
                    int Global = 0;
                    int Changed = 0;
                    int Return = 0;
                    Dometic = (from d in db.InvoiceDetails
                               where d.TicketType == 1 && d.AgentID == Models.userprofile.CurrentUser.AgentID
                               select d).Count();
                    Global = (from g in db.InvoiceDetails
                              where g.TicketType == 2 && g.AgentID == Models.userprofile.CurrentUser.AgentID
                              select g).Count();
                    Changed = (from c in db.InvoiceDetails
                               where c.TicketType == 3 && c.AgentID == Models.userprofile.CurrentUser.AgentID
                               select c).Count();
                    Return = (from r in db.InvoiceDetails
                              where r.TicketType == 4 && r.AgentID == Models.userprofile.CurrentUser.AgentID
                              select r).Count();
                    ViewBag.Dometic = Dometic == null ? 0 : Dometic;
                    ViewBag.Global = Global == null ? 0 : Global;
                    ViewBag.Changed = Changed == null ? 0 : Changed;
                    ViewBag.Return = Return == null ? 0 : Return;
                }
                return View();
            }
        }
        
        
    }
}
