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
    public class CustomerController : BaseController
    {
        #region Customers
        public ActionResult Index()
        {
            return View(GetListCustomer(1, 20));
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
                ViewBag.TotalRow = query.Count();
                return query.ToPagedList(currentPage, pageSize);
            }
        }

        public ActionResult AddCustomer()
        {
            var model = new Customer()
            {
                 CustomerType =(int) Utils.CustomerType.Single, Gender = true, IsOld = true,  IsForeigner = false
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomer(Models.Customer model)
        {
            if (model != null && ModelState.IsValid)
            {
                using (var db = new Models.InvoiceContext())
                {
                    model.IsOld = true;
                    db.Customers.Add(model);
                    var i = db.SaveChanges();
                    if (i > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                        return View(model);
                }
            }
            else
                return View(model);
        }

        public ActionResult EditCustomer(string id)
        {
            if (string.IsNullOrEmpty(id)) return View("AddCustomer", new Models.Customer() { Gender = true, IdentityNumber = "" });
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new Models.InvoiceContext())
            {
                var obj = db.Customers.FirstOrDefault(x => x.CustomerID == key);
                return View(obj);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(Models.Customer model)
        {
            if (model != null && ModelState.IsValid)
            {
                using (var db = new Models.InvoiceContext())
                {
                    var obj = db.Customers.FirstOrDefault(x => x.CustomerID == model.CustomerID);
                    if (obj != null)
                    {
                        obj.BirthDay = model.BirthDay;
                        obj.CustomerCode = model.CustomerCode;
                        obj.CustomerType = model.CustomerType;
                        obj.Description = model.Description;
                        obj.Email = model.Email;
                        obj.FullName = model.FullName;
                        obj.Gender = model.Gender;
                        obj.IdentityNumber = model.IdentityNumber;
                        obj.IsForeigner = model.IsForeigner;
                        obj.IsOld = true;
                        obj.Phone = model.Phone;
                    }
                    var i = db.SaveChanges();
                    if (i > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                        return View(model);
                }
            }
            else
                return View(model);
        }

        public ActionResult DeleteCustomer(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.Customers.FirstOrDefault(x => x.CustomerID == key);
                if (obj != null)
                {
                    db.Customers.Remove(obj);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
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
                    return Json(new {CustomerID = -1 }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion
    }
}
