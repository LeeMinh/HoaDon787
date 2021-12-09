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
    public partial class ReturnReport : DevExpress.XtraReports.UI.XtraReport
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(NormalReport));
        public ReturnReport()
        {
            InitializeComponent();
        }
        public ReturnReport(int copy)
        {
            InitializeComponent();
            //lblCopy.Text = string.Format(lblCopy.Tag.ToString(), copy);
            //var key = Utils.CommonFunction.Expand(id);
            //using (var db = new Models.InvoiceContext())
            //{
            //    var invoice = db.Invoices.FirstOrDefault(x => x.InvoiceID == key);
            //    if (invoice != null)
            //    {
            //        invoice.InvoiceDetails = db.InvoiceDetails.Where(x => x.InvoiceID == key).ToList();
            //        var branch = db.AgentBranches.FirstOrDefault(x => x.AgentBranchID == invoice.AgentBranchID);
            //        if (branch != null)
            //        {
            //            invoice.BranchName = branch.BranchName;
            //            invoice.BranchAddress = branch.Address;
            //            invoice.FormNo = branch.FormNo;
            //            invoice.Serial = branch.Serial;
            //        }
            //    }
            //    this.DataSource = new System.Collections.Generic.List<Models.Invoice>() { invoice };
            //}
        }

        private void PageHeader_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                //txtIssuingOffice.Text = string.Format(txtIssuingOffice.Tag.ToString(), GetCurrentColumnValue("BranchName"));
                //txtAgentAddress.Text = string.Format(txtAgentAddress.Tag.ToString(), GetCurrentColumnValue("BranchAddress"));
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

            }
            catch 
            {
                
            }
        }

        private void GroupFooter1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
       
        private void xrLabel46_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void lblCopy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //try
            //{
            //    lblCopy.Text = string.Format(lblCopy.Tag.ToString(), Parameters["Copy"].Value);
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //}
        }
    }
}
