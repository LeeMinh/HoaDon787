using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirlineInvoice.Models;
using System.Threading;

namespace AirlineInvoice.Controllers
{
    public class ImportController : Controller
    {
        //
        // GET: /Import/
        public string ticketOk = "";
        public string ticketFail = "";

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ImportStringTicket()
        {
            return View();
        }

        public string InsertClick(string stringTicket)
        {
            string output = "";
            string input = "";
            input = stringTicket.Trim();
            string[] listTicketNo = input.Split(',');
            addticket(listTicketNo);
            //Thread thread = new Thread(() => addticket(listTicketNo));
            //thread.Start();
            output = "<div class='col-md-6'><div id='ok'>" + ticketOk + "</div></div><div class='col-md-6'><div id='fail'>" + ticketFail + "</div></div>";
            return output;
            
        }
        public void addticket(string[] listTicketNo)
        {
            using (var db = new InvoiceContext())
            {
                foreach (var item in listTicketNo)
                {
                    InvoiceDetail invoice = new InvoiceDetail();
                    invoice = invoice.GetByTicketNo(item);
                    if (invoice.InvoiceDetailID > 0)
                    {

                        ticketOk += "<p>" + item + " Đã có trong DB</p>";
                        //đã có trong DB
                        
                        continue;
                    }
                    else
                    {
                        var sv = new InvoiceService.VNAWSSoapClient();
                        var agent = db.Agents.FirstOrDefault(x => x.AgentID == Models.userprofile.CurrentUser.AgentID);
                        var res = sv.OpenFullTicketNumber(new InvoiceService.Authentication() { Username = "invoice", Password = "idb@@" },
                            item, agent.SigninSabre, agent.PasscodeSabre, agent.SuffixSabre);
                        if (res.VCRDisplay == null || res.VCRDisplay.Length <= 0)
                        {
                            ticketFail += "<p style='color:red;'>" + item + " Không load được số vé</p>";
                            //Không load được số vé
                            continue;
                        }
                        else
                        {
                            invoice.TicketNo = res.TicketNo;
                            invoice.TicketType = res.TicketType;
                            invoice.TripCode = res.TripCode;
                            invoice.TicketFare = res.TicketFare;
                            invoice.TicketVAT = res.TicketVAT;
                            invoice.TicketTax = res.TicketTax;
                            invoice.TicketTaxGlobal = res.TicketTaxGlobal;
                            invoice.RealPay = res.RealPay;
                            invoice.TicketPrice = res.TicketPrice;
                            invoice.VCRDisplay = res.VCRDisplay;
                            invoice.UpdateSystem = res.UpdateSystem;
                            invoice.Airline = res.Airline;
                            invoice.InvoiceID = 0;
                            invoice.PNRCode = res.PNRCode != null ? res.PNRCode : "0";
                            invoice.PaxName = res.PaxName;
                            invoice.AgentID = agent.AgentID; ;
                            invoice.AgentBranchID = Models.userprofile.CurrentUser.AgentBranchID;
                            invoice.UpdateTime = DateTime.Now;
                            invoice.OtherFees = res.OtherFees;
                            invoice.ReturnFees = res.ReturnFees;
                            invoice.ChangeTicket = res.ChangeTicket != null ? res.ChangeTicket : "0";
                            invoice.ChangeLevelFee = res.ChangeLevelFee;
                            invoice.ChangeLevelFeeVAT = res.ChangeLevelFeeVAT;
                            invoice.Note = res.Note != null ? res.Note : "0";
                            invoice.AgentFee = res.AgentFee;
                            invoice.AgentFeeVAT = res.AgentFeeVAT;
                            invoice.SoldDate = res.SoldDate;
                            int id = 0;
                            try
                            {
                                id = invoice.Insert();
                                if (id > 0)
                                {
                                    ticketOk += "<p>" + item + " Thêm thành công</p>";
                                    //Thêm thành công
                                }
                                else
                                {
                                    ticketFail += "<p style='color:red;'>" + item + " Thêm thất bại</p>";
                                    //Thêm thất bại
                                }
                            }
                            catch (Exception)
                            {
                                ticketFail += "<p style='color:red;'>" + item + " Thêm thất bại</p>";
                                //Thêm thất bại
                                continue;
                            }

                        }
                    }
                }
            }
        }
        public ActionResult AddSingleTicketForm()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertSingleTicket(FormCollection fc)
        {
            try
            {
                InvoiceDetail item = new InvoiceDetail();
                item.TicketNo = fc["TicketNo"];
                item.TicketType = Convert.ToInt32(fc["TicketType"]);
                item.TripCode = fc["TripCode"];
                item.TicketFare = Convert.ToInt32(fc["TicketFare"]);
                item.TicketVAT = Convert.ToInt32(fc["TicketVAT"]);
                item.TicketTax = Convert.ToInt32(fc["TicketTax"]);
                item.TicketTaxGlobal = 0;
                item.RealPay = 0;
                item.TicketPrice = Convert.ToInt32(fc["TicketPrice"]);
                item.VCRDisplay = "";
                item.UpdateSystem = false;
                item.Airline = fc["Airline"];
                item.InvoiceID = 0;
                item.PNRCode = "";
                item.PaxName = fc["PaxName"];
                item.AgentID = userprofile.CurrentUser.AgentID;
                item.AgentBranchID = userprofile.CurrentUser.AgentBranchID;
                item.UpdateTime = DateTime.Now;
                item.OtherFees = 0;
                item.ReturnFees = 0;
                item.ChangeTicket = "";
                item.ChangeLevelFee = 0;
                item.ChangeLevelFeeVAT = 0;
                item.Note = "";
                item.AgentFee = 0;
                item.AgentFeeVAT = 0;
                item.SoldDate = Convert.ToDateTime(fc["SoldDate"]);
                InvoiceDetail ticket = new InvoiceDetail();
                ticket = ticket.GetByTicketNo(item.TicketNo);
                if (ticket.InvoiceDetailID == null || ticket.InvoiceDetailID == 0)
                {
                    item.Insert();
                    return RedirectToAction("Dometic", "Ticket");
                }
                else
                {
                    TempData["Message"] = "exist";
                    return RedirectToAction("AddSingleTicketForm");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormUpload(HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (upload != null && upload.ContentLength > 0)
                    {
                        Stream stream = upload.InputStream;
                        IExcelDataReader reader = null;
                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "File không đúng định dạng EXCEL");
                            return View();
                        }

                        reader.IsFirstRowAsColumnNames = true;

                        DataSet result = reader.AsDataSet();
                        reader.Close();

                        //result.Tables[0].Rows[]

                        return View(result.Tables[0]);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Hãy chọn file EXCEL");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult InsertInvoiceDetails(FormCollection fc)
        {
            try
            {
                int count = 0;
                count = Convert.ToInt32(fc["count"]);
                List<InvoiceDetail> l_ticket = new List<InvoiceDetail>();
                List<string> l_exist = new List<string>();
                List<string> l_add = new List<string>();
                for (int i = 1; i < count; i++)
                {
                    if (!string.IsNullOrEmpty(fc["Column0" + i].Trim()))
                    {
                        InvoiceDetail item = new InvoiceDetail();
                        item.TicketNo = fc["Column3" + i].Trim();
                        switch (fc["Column6" + i].Trim().ToString())
                        {
                            case "S":
                                if (fc["Column6" + i].Trim().ToString() == "VND")
                                {
                                    item.TicketType = 1;
                                }
                                else
                                {
                                    item.TicketType = 2;
                                }
                                break;
                            case "R":
                                item.TicketType = 4;
                                break;
                            default:
                                item.TicketType = 0;
                                break;
                        }

                        item.TripCode = fc["Column5" + i].Trim();
                        item.TicketFare = Convert.ToInt32(Math.Round(Convert.ToDouble(fc["Column10" + i].Trim())));
                        item.TicketVAT = Convert.ToInt32(Math.Round(Convert.ToDouble(fc["Column13" + i].Trim())));
                        item.TicketTax = Convert.ToInt32(Math.Round(Convert.ToDouble(fc["Column11" + i].Trim())));
                        item.TicketTaxGlobal = 0;
                        item.RealPay = 0;
                        item.TicketPrice = Convert.ToInt32(Math.Round(Convert.ToDouble(fc["Column18" + i].Trim())));
                        item.VCRDisplay = "";
                        item.UpdateSystem = false;
                        item.Airline = "VN";
                        if (!string.IsNullOrEmpty(fc["InvoiceID"]))
                        {
                            item.InvoiceID = Convert.ToInt32(fc["InvoiceID"].Trim());
                        }
                        else
                        {
                            item.InvoiceID = 0;
                        }

                        item.PNRCode = "";
                        item.PaxName = fc["Column4" + i].Trim();
                        item.AgentID = userprofile.CurrentUser.AgentID;
                        item.AgentBranchID = userprofile.CurrentUser.AgentBranchID;
                        item.UpdateTime = DateTime.Now;
                        item.OtherFees = 0;
                        item.ReturnFees = 0;
                        item.ChangeTicket = "";
                        item.ChangeLevelFee = 0;
                        item.ChangeLevelFeeVAT = 0;
                        item.Note = "";
                        item.AgentFee = 0;
                        item.AgentFeeVAT = 0;
                        string soldDate = fc["Column2" + i].Trim().ToString();
                        TempData["soldDate"] = soldDate;
                        if (soldDate.Contains(","))
                        {
                            string[] txtsoldDate = soldDate.Split(',');
                            item.SoldDate = Convert.ToDateTime(txtsoldDate[1]);
                        }
                        else
                        {
                            item.SoldDate = Convert.ToDateTime(fc["Column2" + i].Trim());
                        }


                        InvoiceDetail ticket = new InvoiceDetail();
                        ticket = ticket.GetByTicketNo(item.TicketNo);
                        if (ticket.InvoiceDetailID == null || ticket.InvoiceDetailID == 0)
                        {
                            l_ticket.Add(item);
                            l_add.Add(item.TicketNo);
                        }
                        else
                        {
                            l_exist.Add(item.TicketNo);
                        }
                    }
                }
                TempData["Exist"] = l_exist;
                TempData["Add"] = l_add;
                if (l_ticket.Count() > 0)
                {
                    foreach (var item in l_ticket)
                    {
                        item.Insert();
                    }
                }
                return RedirectToAction("Upload");
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(TempData["soldDate"].ToString());
            }

        }

    }
}
