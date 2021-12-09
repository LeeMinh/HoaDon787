using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class DbContextBase : DbContext
    {
        public DbContextBase()
            : base("DefaultConnection")
        { 
        
        }
    }
}