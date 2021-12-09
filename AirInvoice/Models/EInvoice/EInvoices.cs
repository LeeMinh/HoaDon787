using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace AirlineInvoice.Models.EInvoiceVNPT
{
    [Serializable]
    public class Invoice
    {
        //[XmlElement(ElementName = "CusCode")]
        public string CusCode { get; set; } = "";
        //[XmlElement(ElementName = "CusName")]
        public string CusName { get; set; } = "";
        public string EmailDeliver { get; set; } = "";
        //[XmlElement(ElementName = "EmailDeliver")]
        public string Buyer { get; set; } = "";
        //[XmlElement(ElementName = "CusAddress")]
        public string CusAddress { get; set; } = "";
        //[XmlElement(ElementName = "CusPhone")]
        public string CusPhone { get; set; } = "";
        //[XmlElement(ElementName = "CusTaxCode")]
        public string CusTaxCode { get; set; } = "";
        //[XmlElement(ElementName = "PaymentMethod")]
        public string PaymentMethod { get; set; } = "";
        //[XmlElement(ElementName = "KindOfService")]
        public string KindOfService { get; set; } = "";
        //[XmlElement(ElementName = "Products")]
        public List<Product> Products { get; set; } = new List<Product>();
        //[XmlElement(ElementName = "Total")]
        public double Total { get; set; }
        //[XmlElement(ElementName = "Extra7")]
        public double Extra7 { get; set; }
        //[XmlElement(ElementName = "DiscountAmount")]
        public double DiscountAmount { get; set; }
        //[XmlElement(ElementName = "Extra")]
        public double Extra { get; set; }
        //[XmlElement(ElementName = "Extra1")]
        public double Extra1 { get; set; }
        //[XmlElement(ElementName = "Extra2")]
        public double Extra2 { get; set; }
        //[XmlElement(ElementName = "Extra4")]
        public double Extra4 { get; set; }
        //[XmlElement(ElementName = "Extra5")]
        public double Extra5 { get; set; }
        //[XmlElement(ElementName = "Extra6")]
        public double Extra6 { get; set; }
        //[XmlElement(ElementName = "VATAmount")]
        public double VATAmount { get; set; }
        //[XmlElement(ElementName = "Amount")]
        public double Amount { get; set; }
        //[XmlElement(ElementName = "AmountInWords")]
        public string AmountInWords { get; set; }
        //[XmlElement(ElementName = "ArisingDate")]
        //[XmlElement(IsNullable = true)]
        public string ArisingDate { get; set; } = "";
        //[XmlElement(ElementName = "PaymentStatus")]
        //[XmlElement(IsNullable = true)]
        public string PaymentStatus { get; set; } = "";
    }
    
    public class Product
    {
        public string ProdName { get; set; }
        public string ProdUnit { get; set; }
        public int ProdQuantity { get; set; } = 0;
        public int VATRate { get; set; } = 0;
        public double VATAmount { get; set; } = 0;
        public int Extra2 { get; set; } = 0;
        public double ProdPrice { get; set; } = 0;
        public double Amount { get; set; } = 0;
        public double Total { get; set; } = 0;
    }

}