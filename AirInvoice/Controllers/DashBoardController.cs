using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirlineInvoice.Controllers
{
    public class DashBoardController : BaseController
    {
        //
        // GET: /DashBoard/
        public ActionResult Index()
        {
            return View();
        }
	}
}