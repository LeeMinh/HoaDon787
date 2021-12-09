using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace AirlineInvoice.ReportFiles
{
    public partial class NormalReport : DevExpress.XtraReports.UI.XtraReport
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(NormalReport));
        public NormalReport()
        {
            InitializeComponent();
        }
        public NormalReport(int copy)
        {
            InitializeComponent();
            //lblCopy.Text = string.Format(lblCopy.Tag.ToString(), copy);
            
        }

        private void PageHeader_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex);   
            }
        }

        private void GroupHeader1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //try
            //{
            //    var invoices = (List<Models.Invoice>)this.DataSource;
            //    if (invoices != null && invoices.Count > 0)
            //    {
            //        if (invoices[0].InvoiceDetails.Count > 3)
            //        {
            //            DetailReport.Visible = false;
            //            xrTable1.Visible = false;
            //            lblAirFare.Text = string.Format(lblAirFare.Tag.ToString(), invoices[0].InvoiceDetails.Sum(x => x.TicketFare), invoices[0].InvoiceDetails.Sum(x => x.TicketVAT));
            //        }
            //        var invoice = invoices[0];
            //        txtInvoiceDate.Text = string.Format(txtInvoiceDate.Tag.ToString(), invoice.InvoiceDate.Day, invoice.InvoiceDate.Month, invoice.InvoiceDate.Year);
            //        txtCustomerName.Text = string.Format(txtCustomerName.Tag.ToString(), invoice.CustomerName);
            //        txtCustomerAddress.Text = string.Format(txtCustomerAddress.Tag.ToString(), invoice.CustomerAddress);
            //    }
                
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //}
        }

        private int? totalFooter1 = 0;
        private void GroupFooter1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                var invoices = (List<Models.Invoice>)this.DataSource;
                 if (invoices != null && invoices.Count > 0)
                {
                    using (var db = new Models.InvoiceContext())
                    {
                        var branch = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == invoices[0].AgentBranchID);
                        var Agent = db.Agents.FirstOrDefault(x => x.AgentID == invoices[0].AgentID);
                        txtBranchTel.Text = branch.Tel;
                        xrPictureBox1.ImageUrl = Agent.AgentLogo;
                        txtPaymentMethod.Text = Convert.ToString(AirlineInvoice.Utils.DataComboSource.PaymentTypeList().Where(x => x.Value == "3").FirstOrDefault().Text);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            //try
            //{
            //    var invoices = (List<Models.Invoice>)this.DataSource;
            //    if (invoices != null && invoices.Count > 0)
            //    {
            //        var invoice = invoices[0];
            //        var totalVAT = invoice.InvoiceDetails.Sum(x => x.TicketVAT.ConvertToInt());
            //        txtVAT.Text = string.Format("{0:#,#}", totalVAT);
            //        lblVATUnit.Visible = totalVAT > 0;
            //        var totalTicketTax = invoice.InvoiceDetails.Sum(x => x.TicketTax.ConvertToInt());
            //        txtTicketTax.Text = string.Format("{0:#,#}", totalTicketTax);
            //        lblOtherTaxUnit.Visible = totalTicketTax > 0;
            //        var totalOtherFee = invoice.InvoiceDetails.Sum(x => x.OtherFees.ConvertToInt());
            //        txtOtherFee.Text = string.Format("{0:#,#}", totalOtherFee);
            //        lblOtherFeeUnit.Visible = totalOtherFee > 0;
            //        var totalFare = invoice.InvoiceDetails.Sum(x => x.TicketFare);
            //        totalFooter1 = totalFare + totalOtherFee + totalTicketTax + totalVAT;
            //        txtTotal1.Text = string.Format("{0:#,#}", totalFooter1);
            //        lblTotalFareUnit.Visible = totalFooter1 > 0;
            //        // footer 2
            //        var totalAgentFee = invoice.TotalAgentFee.ConvertToInt();
            //        var AgentFeeVAT = totalAgentFee / 11;
            //        var AgentFee = totalAgentFee - AgentFeeVAT;
            //        txtAgentFeeNoneVAT.Text = string.Format("{0:#,#}", AgentFee);
            //        txtAgentFeeUnit.Visible = totalAgentFee > 0;
            //        txtAgentFeeVAT.Text = string.Format("{0:#,#}", AgentFeeVAT);
            //        lblAgentFeeVATUnit.Visible = AgentFeeVAT > 0;
            //        txtTotalAgentFee.Text = string.Format("{0:#,#}", totalAgentFee);
            //        lblTotalAgentFeeUnit.Visible = totalAgentFee > 0;
            //        // footer 3
            //        var totalPayment = 0;
            //        invoice.InvoiceDetails.ForEach(x => totalPayment += x.ChangeLevelFee.ConvertToInt()
            //            + x.ChangeLevelFeeVAT.ConvertToInt()
            //            + x.OtherFees.ConvertToInt() + x.ReturnFees.ConvertToInt() + x.TicketFare.ConvertToInt()
            //            + x.TicketTax.ConvertToInt() + x.TicketVAT.ConvertToInt() + totalAgentFee);
            //if (invoice.PaymentType == (int)Utils.PaymentType.CashInBank)
            //        {
            //            lblFormOfPayment.Text = string.Format(lblFormOfPayment.Tag.ToString(), "Chuyển khoản (Bank transfer)");
            //        }
            //        else
            //        {
            //            lblFormOfPayment.Text = string.Format(lblFormOfPayment.Tag.ToString(), "Tiền mặt (Cash)");
            //        }
            //        txtTotalPayment.Text = string.Format("{0:#,#}", totalPayment);
            //        txtTotalPaymentText.Text = string.Format(txtTotalPaymentText.Tag.ToString(),
            //            Utils.CommonFunction.SayMoney(totalPayment.ConvertToInt()));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //}
        }
        private void GroupFooter2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                 var invoices = (List<Models.Invoice>)this.DataSource;
                 if (invoices != null && invoices.Count > 0)
                 {
                     
                 }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void GroupFooter3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                var invoices = (List<Models.Invoice>)this.DataSource;
                if (invoices != null && invoices.Count > 0)
                {
                    var invoice = invoices[0];
                    
                   
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void xrLabel46_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //try
            //{
            //    var invoices = (List<Models.Invoice>)this.DataSource;
            //    if (invoices != null && invoices.Count > 0)
            //    {
            //        Guid userID;
            //        var invoice = invoices[0];
            //        if (Guid.TryParse(invoice.UserCreate, out userID))
            //        {
            //            var user = Models.userprofile.GetUser(userID);
            //            if (user != null)
            //            {
            //                xrLabel46.Text = user.FullName;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //}
        }

        private void lblCopy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //try
            //{

            //    var invoices = (List<AirlineInvoice.Models.Invoice>)this.DataSource;
            //    if (invoices != null && invoices.Count > 0)
            //    {
            //        var invoice = invoices[0];
            //        lblCopy.Text = string.Format(lblCopy.Tag.ToString(), invoice.PrintCopy);
            //    }

            //}
            //catch
            //{

            //}
        }

    }
}
