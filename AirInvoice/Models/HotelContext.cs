using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class InvoiceContext : DbContextBase
    {
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AgentBranch> AgentBranches { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<DBOption> DBOptions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
    }
}