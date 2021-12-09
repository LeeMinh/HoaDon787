using AirlineInvoice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirlineInvoice.Controllers
{

    public class AgentBranchController : BaseController
    {
        public ActionResult Index()
        {
            var lstAgentBranch = GetListAgentBranch(1, 20);
            ViewBag.Agent= GetListAgent();
            return View(lstAgentBranch);
        }

        private List<Agent> GetListAgent()
        {
            using (var db = new InvoiceContext())
            {
                return db.Agents.ToList();
            }
        }

        private List<Models.AgentBranch> GetListAgentBranch(int currentPage, int pageSize)
        {
            using (var db = new Models.InvoiceContext())
            {
                var agentID = Models.userprofile.CurrentUser.AgentID;
                var query = db.AgentBranches.Where(x => x.AgentID == agentID ).OrderBy(x => x.BranchName);
                ViewBag.CurrentPage = currentPage;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRow = query.Count();
                var lst = query.OrderBy(x => x.AgentID).ToPagedList(currentPage, pageSize);
                return lst;
            }
        }

        public PartialViewResult GetAgentBranchData(int currentPage, int pageSize, int agentID)
        {
            return PartialView("AgentBranchData", GetListAgentBranch(currentPage, pageSize));
        }


        public ActionResult AddAgentBranch()
        {
            var agentID = userprofile.CurrentUser.AgentID;
            var obj = new AgentBranch()
            {
                AgentID = agentID
            };
            ViewBag.Agents = GetListAgent();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddAgentBranch(AgentBranch model)
        {
            if (ModelState.IsValid)
            {
                var agentID = userprofile.CurrentUser.AgentID;
                using (var db = new InvoiceContext())
                {
                    model.AgentID = agentID;   
                    db.AgentBranches.Add(model);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Agents = GetListAgent();
                return View(model);
            }
        }

        public ActionResult EditAgentBranch(string id)
        {
            ViewBag.Agents = GetListAgent();
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == key);
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
        [ValidateInput(false)]
        public ActionResult EditAgentBranch(AgentBranch model)
        {
            ViewBag.Agents = GetListAgent();
            if (ModelState.IsValid)
            {
                using (var db = new InvoiceContext())
                {
                    var agentID = userprofile.CurrentUser.AgentID;
                    var obj = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == model.AgentBranchID);
                    obj.Address = model.Address;
                    obj.AgentID = agentID;
                    obj.BankAcc = model.BankAcc;
                    obj.BranchName = model.BranchName;
                    obj.HeaderInvoice = model.HeaderInvoice;
                    obj.InvoicePrefix = model.InvoicePrefix;
                    obj.InvoiceStartNumber = model.InvoiceStartNumber;
                    obj.InvoiceEndNumber = model.InvoiceEndNumber;
                    obj.Tax= model.Tax;
                    obj.Tel = model.Tel;
                    obj.FormNo = model.FormNo;
                    obj.Serial = model.Serial;
                    obj.InvoiceReturnStart = model.InvoiceReturnStart;
                    obj.InvoiceReturnEnd = model.InvoiceReturnEnd;
                    obj.FormNoReturn = model.FormNoReturn;
                    obj.SerialReturn = model.SerialReturn;
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

        public ActionResult DeleteBranch(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == key);
                if (obj != null)
                {
                    db.AgentBranches.Remove(obj);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Nhân bản loại phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CloneAgentBranch(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == key);
                if (obj != null)
                {
                    var newBranch= new Models.AgentBranch()
                    {
                        Address = obj.Address,
                          AgentID  = obj.AgentID,
                           BankAcc = obj.BankAcc,
                            BranchName = obj.BranchName,
                             HeaderInvoice = obj.HeaderInvoice,
                              InvoicePrefix = obj.InvoicePrefix,
                               InvoiceStartNumber    = obj.InvoiceStartNumber,
                                Tax = obj.Tax,
                                 Tel = obj.Tel
                    };
                    db.AgentBranches.Add(newBranch);
                    var i = db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }
    }
}
