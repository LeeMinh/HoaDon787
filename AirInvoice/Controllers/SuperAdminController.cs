using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using AirlineInvoice.Models;
using System.Web.Security;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Routing;
using System.IO;


namespace AirlineInvoice.Controllers
{
    //[RequireHttps]
    public class SuperAdminController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToLower().Equals("login"))
            {
                base.OnActionExecuted(filterContext);
            }
            else
            {
                if (Convert.ToBoolean(Session["SALogin"]) != true)
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                { "Controller", "superadmin" },
                { "Action", "login" },
                {"returnUrl",filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery}
            });
                }
            }
        }
       [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName.Equals("sa") && model.Password.Equals("123456@@"))
                {
                    Session["SALogin"] = true;
                    return RedirectToAction("ManagerAccount");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            var model = new RegisterModel();
            using (var db = new Models.InvoiceContext())
            {
                var agent = db.Agents.OrderByDescending(x=>x.AgentID).FirstOrDefault();
                if (agent != null)
                {
                    model.AgentName = agent.AgentName;
                }
            }
            return View(model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MembershipCreateStatus createStatus;
                    //if (model.userInRoles.Roles.Count(x => x.Value) == 0)
                    //{
                    //    ModelState.AddModelError("Bạn chưa phân quyền cho người dùng.", ErrorCodeToString(MembershipCreateStatus.UserRejected));
                    //    return View(model);
                    //}
                    Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        // cap nhat role
                        Roles.AddUserToRole(model.UserName, Utils.CommonFunction._AdminRole);
                        // cap nhat profile

                        var profile = userprofile.GetUser(model.UserName);
                        profile.FullName = model.FullName;
                        profile.Address = model.Address;
                        profile.Gender = (int)model.Gender;
                        profile.Tel = model.Tel;
                        //profile.AgentID = model.AgentID;
                        using (var db = new Models.InvoiceContext())
                        {
                            var agent = db.Agents.FirstOrDefault(x => x.AgentName.Equals(model.AgentName.Trim()));
                            if (agent != null)
                            {
                                profile.AgentID = model.AgentID;
                            }
                            else
                            {
                                var newAgent = new Models.Agent()
                                {
                                     AgentName = model.AgentName
                                };
                                db.Agents.Add(newAgent);
                                db.SaveChanges();
                                profile.AgentID = newAgent.AgentID;
                            }
                        }
                        profile.AgentBranchID = 0;
                        if (Request.Files.Count > 0)
                        {
                            var file = Request.Files[0];
                            var ext = ".jpg";
                            var posExt = file.FileName.LastIndexOf(".");
                            if (posExt > 0)
                            {
                                ext = file.FileName.Substring(posExt);
                            }
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = string.Format("{0}_{1}{2}", model.UserName, Guid.NewGuid().ToString().Replace("-", ""), ext);
                                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                                file.SaveAs(path);
                                profile.AvatarLink = "~/Images/" + fileName;
                            }
                        }
                        return RedirectToAction("ManagerAccount");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }
                }
            }
            catch (MembershipPasswordException ex)
            {
                ModelState.AddModelError("", ex.Message);

            }
            // Attempt to register the user



            // If we got this far, something failed, redisplay form
            return View(model);
        }


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Tên đăng nhập đã tồn tại. Xin mời nhập lại.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Email đã được sử dụng. Xin mời nhập lại.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Mật khẩu quá yếu. Xin mời nhập lại.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Email không hợp lệ. Xin mời nhập lại.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "Bạn chưa phân quyền cho người dùng";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        #region manager account
        public ActionResult EncryptConnectionString()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection section = config.GetSection("connectionStrings");
            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("RSAProtectedConfigurationProvider");
                config.Save();
            }
            return RedirectToAction("ManagerAccount", "Account");
        }
        public ActionResult DecryptConnectionString()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection section = config.GetSection("connectionStrings");
            if (section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
                config.Save();
            }
            return RedirectToAction("ManagerAccount", "Account");
        }
        public ActionResult ManagerAccount()
        {
            var lst = Membership.GetAllUsers();
            var lstRemove = new List<MembershipUser>();
            foreach (MembershipUser item in lst)
            {
                if (!Roles.IsUserInRole(item.UserName, Utils.CommonFunction._AdminRole))
                {
                    lstRemove.Add(item);
                }
            }
            foreach (var item in lstRemove)
            {
                lst.Remove(item.UserName);
            }
            return View(lst);
        }

        public ActionResult EditUser(string userName)
        {
            var profile = userprofile.GetUser(userName);
            var user = Membership.GetUser(userName);
            var model = new EditUserModel()
            {
                Address = profile.Address,
                FullName = profile.FullName,
                Email = user.Email,
                //OldPassword = user.GetPassword(),
                Gender = profile.Gender,
                Tel = profile.Tel,
                UserName = userName,
                AvatarLink = profile.AvatarLink,
                AgentID=profile.AgentID,
                AgentBranchID = profile.AgentBranchID
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(EditUserModel user)
        {
            if (ModelState.IsValid)
            {
                var obj = Membership.GetUser(user.UserName);
                var profile = userprofile.GetUser(user.UserName);
                profile.FullName = user.FullName;
                profile.Tel = user.Tel;
                profile.Gender = user.Gender;
                profile.Address = user.Address;
                if (obj != null)
                {
                    obj.Email = user.Email;
                    Membership.UpdateUser(obj);
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        var ext = ".jpg";
                        var posExt = file.FileName.LastIndexOf(".");
                        if (posExt > 0)
                        {
                            ext = file.FileName.Substring(posExt);
                        }
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = string.Format("{0}_{1}{2}", user.UserName, Guid.NewGuid().ToString().Replace("-", ""), ext);
                            var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                            file.SaveAs(path);
                            profile.AvatarLink = "~/Images/" + fileName; 
                        }
                    }
                }
                profile.AgentBranchID = 0;
                profile.AgentID = user.AgentID;
                return RedirectToAction("ManagerAccount");
            }
            return View(user);

        }

        public ActionResult DeleteUser(string userName)
        {
            Membership.DeleteUser(userName);
            return RedirectToAction("ManagerAccount");
        }

        public ActionResult RoleManager(string userName)
        {
            var obj = new UserInRoles();
            obj.UserName = userName;

            var dicRole = new Dictionary<string, bool>();
            var arrRoles = Roles.GetRolesForUser(userName);

            foreach (var item in arrRoles)
            {
                obj.Roles[item] = true;
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleManager(UserInRoles userInRoles)
        {
            try
            {
                List<string> lstRoles = new List<string>();
                List<string> lstRemove = new List<string>();
                foreach (var item in userInRoles.Roles)
                {
                    if (item.Value)
                    {
                        if (!Roles.Provider.IsUserInRole(userInRoles.UserName, item.Key))
                        {
                            lstRoles.Add(item.Key);
                        }
                    }
                    else
                    {
                        if (Roles.Provider.IsUserInRole(userInRoles.UserName, item.Key))
                        {
                            lstRemove.Add(item.Key);
                        }
                    }
                }
                if (lstRemove.Count > 0) Roles.RemoveUsersFromRoles(new string[] { userInRoles.UserName }, lstRemove.ToArray());
                if (lstRoles.Count > 0) Roles.AddUsersToRoles(new string[] { userInRoles.UserName }, lstRoles.ToArray());
                return RedirectToAction("ManagerAccount");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return View(userInRoles);
        }

        public ActionResult ChangePassword(string userName)
        {
            return View(new ChangePasswordModel() { UserName = userName });
        }
        public ActionResult UserChangePassword()
        {
            
            return View(new UserChangePasswordModel() { UserName = Membership.GetUser().ToString() });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            //if (!isAdmin())
            //    return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {

                    MembershipUser currentUser = Membership.GetUser(model.UserName);
                    var generatePass = currentUser.ResetPassword();
                    changePasswordSucceeded = currentUser.ChangePassword(generatePass, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ManagerAccount");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu không hợp lệ.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserChangePassword(UserChangePasswordModel model)
        {
            //if (!isAdmin())
            //    return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {

                    MembershipUser currentUser = Membership.GetUser(model.UserName);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu không hợp lệ.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ManageAgent()
        {
            using (var db = new Models.InvoiceContext())
            {
                var lst = db.Agents.ToList();
                return View(lst);
            }
        }
        #endregion

        private bool isAdmin()
        {
            if (!Roles.RoleExists("Admin") || !Roles.IsUserInRole("Admin"))
            {
                return false;
            }
            return true;
        }
    }
}
