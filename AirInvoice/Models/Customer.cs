//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirlineInvoice.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("Customer")]
    public partial class Customer
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public System.DateTime BirthDay { get; set; }
        public string Phone { get; set; }
        public bool Gender { get; set; }
        public bool IsForeigner { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public byte CustomerType { get; set; }
        public bool IsOld { get; set; }
    }
}
