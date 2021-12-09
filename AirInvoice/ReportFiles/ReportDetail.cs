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
    public partial class ReportDetail : DevExpress.XtraReports.UI.XtraReport
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(NormalReport));
        public ReportDetail()
        {
            InitializeComponent();
        }
        public ReportDetail(int copy)
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
            try
            {
                //var invoices = (List<Models.Invoice>)this.DataSource;
                //if (invoices != null && invoices.Count > 0)
                //{
                //    var invoice = invoices[0];
                //    txtInvoiceDate.Text = string.Format(txtInvoiceDate.Tag.ToString(), invoice.InvoiceDate.Day, invoice.InvoiceDate.Month, invoice.InvoiceDate.Year);
                //    txtCustomerName.Text = string.Format(txtCustomerName.Tag.ToString(), invoice.CustomerName);
                //    txtCustomerAddress.Text = string.Format(txtCustomerAddress.Tag.ToString(), invoice.CustomerAddress);
                //}
                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void GroupFooter1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
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

        }
        private int totalMoney = 0;
        private void lblTotalSub_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //try
            //{
            //    var invoices = (List<Models.Invoice>)this.DataSource;
            //    if (invoices != null && invoices.Count > 0)
            //    {
            //        var invoice = invoices[0];
            //        var VAT = DetailReport.GetCurrentColumnValue("TicketVAT").ConvertToInt();
            //        var TicketTax = DetailReport.GetCurrentColumnValue("TicketTax").ConvertToInt();
            //        var OtherFee = DetailReport.GetCurrentColumnValue("OtherFees").ConvertToInt();
            //        var Fare = DetailReport.GetCurrentColumnValue("TicketFare").ConvertToInt();
            //        var totalPayment = Fare + OtherFee + TicketTax + VAT;
            //        lblTotalSub.Text = string.Format("{0:#,#}", totalPayment);
            //        totalMoney += totalPayment;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //}
        }

        private void lblSumTotal_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                lblSumTotal.Text = string.Format("{0:#,#;(0:#,#);0}", totalMoney);
                lblTotalSub.Text = string.Format("{0:#,#;(0:#,#);0}", totalMoney);
                txtTotalMoneyText.Text = string.Format(txtTotalMoneyText.Tag.ToString(), Utils.CommonFunction.SayMoney(totalMoney));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void lblCopy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                //lblCopy.Text = string.Format(lblCopy.Tag.ToString(), Parameters["Copy"].Value);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
