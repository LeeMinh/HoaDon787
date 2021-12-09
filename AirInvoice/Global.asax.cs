using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AirlineInvoice
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();
            //(new AirlineInvoice.Filters.InitializeSimpleMembershipAttribute()).OnActionExecuting(null);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
        protected void Application_PreRequestHandlerExecute()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("vi-VN");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("vi-VN");
        }

        protected void Application_Error()
        { 
        StackTrace stackTrace = new StackTrace();
            ILog logger = LogManager.GetLogger(stackTrace.GetFrame(1).GetMethod().DeclaringType);
            var ex = Server.GetLastError();
            logger.Error(ex);// Right
            logger.Error(ex.Message);
        }
    }
}