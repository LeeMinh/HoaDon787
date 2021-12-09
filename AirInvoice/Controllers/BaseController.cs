using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace AirlineInvoice.Controllers
{
    //[RequireHttps]
    public class BaseController : Controller
    {
        
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // redirect tu https ve http
            //if (!filterContext.RouteData.Values["controller"].ToString().ToLower().Equals("account"))
            //{
            //    if (filterContext.HttpContext.Request.Url.ToString().ToLower().Contains("https"))
            //    {
            //        filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString().Replace("https:", "http:")); // Go on, bugger off "s"!
            //        filterContext.Result.ExecuteResult(filterContext);
            //        return;
            //    }
            //}
            var isValid = true;
            var isAdmin = Utils.CommonFunction.CheckAgentUserPermission();
            // Chua dang nhap thi bat dang nhap
            if (!filterContext.ActionDescriptor.ActionName.ToLower().Equals("login") && Membership.GetUser() == null)
            {
                isValid = false;
            }
           
                // check quyen monitor
            else if (Utils.CommonFunction.ListModulesMonitor.Contains(filterContext.RouteData.Values["controller"].ToString().ToLower())
                && (!Roles.RoleExists(Utils.CommonFunction._Accounting) || !Roles.IsUserInRole(Utils.CommonFunction._Accounting)))
            {
                isValid = false;
            }
                // check quyen xem bao cao
            else if (filterContext.RouteData.Values["controller"].ToString().ToLower().Equals("reports")
                            && (!Roles.RoleExists(Utils.CommonFunction._OperatorRole) || !Roles.IsUserInRole(Utils.CommonFunction._OperatorRole)))
            {
                isValid = false;
            }
            if (!isValid)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                { "Controller", "account" },
                { "Action", "login" },
                {"returnUrl",filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery}
            });
            }
            else
            {
                base.OnActionExecuted(filterContext);
            }
        }
    }
}