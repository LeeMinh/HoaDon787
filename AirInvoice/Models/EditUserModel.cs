using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class EditUserModel
    {
       
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [DataType(DataType.Password)] 
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }
        public string Address { get; set; }
        public int Gender { get; set; }
        public string Tel { get; set; }
        public string UserName { get; set; }
        public string AvatarLink { get; set; }

        public int AgentID { get; set; }
        public int AgentBranchID { get; set; }
        [Display(Name = "Người dùng cấp 2")]
        public bool IsAccounting { get; set; }
    }
}