using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class AgentElectronicBill
    {
        public int AgentId { get; set; }
        public string Account { get; set; }
        public string ACpass { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public string serialCert { get; set; }
        public string taxCode { get; set; }
        public string appId { get; set; }
        public string LinkONFINANCE { get; set; }
    }
}