using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models.EInvoiceONFINANCE
{
    [Serializable]
    public class Invoice
    {
        public string COMNAME { get; set; } = "";
        public string COMTAXCODE { get; set; } = "";
        public string COMADDRESS { get; set; } = "";
        public string FORMCODE { get; set; } = "";
        public string SYMBOLCODE { get; set; } = "";
        public string CUSNAME { get; set; } = "";
        public string CUSADDRESS { get; set; } = "";
        public string CUSBUYER { get; set; } = "";
        public string CUSEMAIL { get; set; } = "";
        public string CUSPHONENUMBER { get; set; } = "";
        public string CUSTAXCODE { get; set; } = "";
        public string CUSPAYMENTMETHOD { get; set; } = "";
        public string CUSACCOUNTNUMBER { get; set; } = "";
        public string CUSBANKNAME { get; set; } = "";
        public string DUEDATE { get; set; } = (DateTime.Now).ToString("yyyy-MM-dd");
        public string DISCOUNTTYPE { get; set; } = "";
        public string NOTE { get; set; } = "";
        public string CREATEDBY { get; set; } = "apiuser";
        public string IPADDRESS { get; set; } = "";
        public string SIGNEDTIME { get; set; } = (DateTime.Now).ToString("yyyy-MM-dd");
        public string INITTIME { get; set; } = (DateTime.Now).ToString("yyyy-MM-dd  hh:mm:ss");
        public string INVOICECODE { get; set; } = "";
        public List<Product> LISTPRODUCT { get; set; } = new List<Product>();
        
    }

    public class Product
    {
        public string SKU { get; set; }
        public string PRODUCTNAME { get; set; }
        public float QUANTITY { get; set; } = 1;
        public string QUANTITYUNIT { get; set; } = "vé";
        public float RETAILPRICE { get; set; } = 0;
        public float SALEPRICE { get; set; } = 0;
        public bool ISPROMOTION { get; set; } = false;
        public float TAXRATE { get; set; } = 0;
        public float TOTALTAX { get; set; } = 0;
        public float DISCOUNTRATE { get; set; } = 0;
        public float OTHERTAXFEE { get; set; } = 0;
        public float REFUNDFEE { get; set; } = 0;
    }
    public class GetTokken
    {
        public string taxCode { get; set; }
        public string appId { get; set; }
    }
}