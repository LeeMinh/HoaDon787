using AirlineInvoice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirlineInvoice.Controllers
{

    public class AgentController : BaseController
    {
        #region Agent
        //
        // GET: /Agent/
        public ActionResult Index()
        {
            using (var db = new Models.InvoiceContext())
            {
                return View(GetListAgent(1,10));            
            }
        }
        private List<Models.Agent> GetListAgent(int currentPage, int pageSize)
        {
            using (var db = new Models.InvoiceContext())
            {
                var query = db.Agents.OrderBy(x=>x.AgentName);
                ViewBag.CurrentPage = currentPage;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRow = query.Count();
                return query.ToPagedList(currentPage, pageSize);
            }
        }

        public PartialViewResult GetAgentData(int currentPage, int pageSize)
        {
            return PartialView("AgentData", GetListAgent(currentPage, pageSize));
        }


        public ActionResult AddAgent()
        {
            var obj = new Agent() { IsActive = true };
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAgent(Agent model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new InvoiceContext())
                {
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
                            var fileName = string.Format("{0}_{1}{2}", model.AgentTaxCode, Guid.NewGuid().ToString().Replace("-", ""), ext);
                            var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                            file.SaveAs(path);
                            model.AgentLogo = path;
                        }
                    }
                    db.Agents.Add(model);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult EditAgent(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.Agents.FirstOrDefault(x => x.AgentID == key);
                if (obj != null)
                {
                    return View(obj);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAgent(Agent model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new InvoiceContext())
                {
                    db.Agents.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            var ext = string.Empty;
                            var posExt = file.FileName.LastIndexOf(".");
                            if (posExt > 0)
                            {
                                ext = file.FileName.Substring(posExt);
                            }
                            var fileName = string.Format("{0}_{1}{2}", model.AgentTaxCode, Guid.NewGuid().ToString().Replace("-", ""), ext);
                            var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                            file.SaveAs(path);
                            model.AgentLogo = "~/Images/" + fileName;
                        }
                        else
                        {
                            db.Entry(model).Property(x => x.AgentLogo).IsModified = false;
                        }
                    }
                    else
                    {
                        db.Entry(model).Property(x => x.AgentLogo).IsModified = false;
                    }
                    var i = db.SaveChanges();
                    if (i > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(model);
                    }
                }
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult DeleteAgent(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.Agents.FirstOrDefault(x => x.AgentID == key);
                if (obj != null)
                {
                    db.Agents.Remove(obj);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
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
