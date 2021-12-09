using AirlineInvoice.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace AirlineInvoice.Controllers
{
    public class GeneralInfoController : BaseController
    {
        //
        // GET: /GeneralInfo/

        public ActionResult Index()
        {
            using (var db = new Models.InvoiceContext())
            {
                var obj = db.Agents.FirstOrDefault(x => x.AgentID == Models.userprofile.CurrentUser.AgentID);
                ViewBag.InfoElecAgent = LoadInfoElectronicBill(obj.AgentID);
                return View(obj);         
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Agent model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                using (var db = new InvoiceContext())
                {
                    var obj = db.Agents.FirstOrDefault(x => x.AgentID == model.AgentID);
                    if (obj == null) return RedirectToAction("Index", "Home");
                    obj.AgentCompanyAddress = model.AgentCompanyAddress;
                    obj.AgentCompanyName = model.AgentCompanyName;
                    obj.AgentName = model.AgentName;
                    obj.AgentTaxCode = model.AgentTaxCode;
                    obj.HardcopyPrinterAddress = model.HardcopyPrinterAddress;
                    obj.IsActive = model.IsActive;
                    obj.ParentID = model.ParentID;
                    if (!string.IsNullOrEmpty(model.PasscodeSabre))
                        obj.PasscodeSabre = model.PasscodeSabre;
                    obj.SigninSabre = model.SigninSabre;
                    obj.StationNumber = model.StationNumber;
                    obj.SuffixSabre = model.SuffixSabre;
                    obj.TicketPrinterAddress = model.TicketPrinterAddress;
                    obj.TicketPrintRoutine = model.TicketPrintRoutine;
                    obj.Tel = model.Tel;
                    obj.Email = model.Email;
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
                            obj.AgentLogo = "~/Images/" + fileName;
                        }
                    }
                    var i = db.SaveChanges();
                    ViewBag.Status = 1;
                    //Lưu thông tin tài khoản VNPT
                    AgentElectronicBill agentElectronicBill = LoadInfoElectronicBill(model.AgentID);
                    agentElectronicBill.Account = fc["Account"].Trim();
                    agentElectronicBill.ACpass = fc["ACpass"].Trim();
                    agentElectronicBill.username = fc["username"].Trim();
                    agentElectronicBill.password = fc["password"].Trim();
                    agentElectronicBill.pattern = fc["pattern"].Trim();
                    agentElectronicBill.serial = fc["serial"].Trim();
                    agentElectronicBill.serialCert = fc["serialCert"].Trim();
                    UpdateInfoElectronicBill(model.AgentID, agentElectronicBill);
                    ViewBag.InfoElecAgent = LoadInfoElectronicBill(obj.AgentID);
                    return View(model);
                }
            }
            else
            {
                ViewBag.Status = 0;
                return View(model);
            }
        }

        public AgentElectronicBill LoadInfoElectronicBill(int AgentId =0)
        {
            try
            {
                AgentElectronicBill InfoElecAgent = new AgentElectronicBill();
                string readText = System.IO.File.ReadAllText(Server.MapPath("~/Config/AgentInfoVNPT.ini"));
                List<AgentElectronicBill> L_InfoElecAgent = JsonConvert.DeserializeObject<List<AgentElectronicBill>>(readText);
                InfoElecAgent = L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault();
                return InfoElecAgent;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void UpdateInfoElectronicBill(int AgentId = 0, AgentElectronicBill InfoElecAgent = null)
        {
            try
            {
                if(InfoElecAgent != null)
                {
                    string readText = System.IO.File.ReadAllText(Server.MapPath("~/Config/AgentInfoVNPT.ini"));
                    List<AgentElectronicBill> L_InfoElecAgent = JsonConvert.DeserializeObject<List<AgentElectronicBill>>(readText);
                    L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault().Account = InfoElecAgent.Account;
                    L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault().ACpass = InfoElecAgent.ACpass;
                    L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault().username = InfoElecAgent.username;
                    L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault().password = InfoElecAgent.password;
                    L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault().pattern = InfoElecAgent.pattern;
                    L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault().serial = InfoElecAgent.serial;
                    L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault().serialCert = InfoElecAgent.serialCert;
                    L_InfoElecAgent.Where(x => x.AgentId == AgentId).FirstOrDefault().AgentId = InfoElecAgent.AgentId;
                    string json = JsonConvert.SerializeObject(L_InfoElecAgent, Formatting.Indented);
                    System.IO.File.WriteAllText(Server.MapPath("~/Config/AgentInfoVNPT.ini"), json);
                }
               // return ;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
