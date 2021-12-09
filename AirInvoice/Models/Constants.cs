using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using AirlineInvoice.Models;
namespace AirlineInvoice.Models
{
    public class Constants
    {
        public static string CONNECTION_STRING = ConfigurationManager.AppSettings["CONNECTION_STRING"];        
    }
}