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
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Xml;

namespace AirlineInvoice.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }
    }
}
