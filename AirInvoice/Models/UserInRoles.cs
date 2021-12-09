using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class UserInRoles
    {
        public string UserName { get; set; }
        public Dictionary<string,bool> Roles { get; set; }
        public UserInRoles()
        {
            UserName = string.Empty;
            Roles = new Dictionary<string, bool>();
            var arrRole = System.Web.Security.Roles.GetAllRoles();
            foreach (var item in arrRole)
            {
                Roles.Add(item, false);
            }
        }
    }
    [Table("webpages_roles")]
    public class Role
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    
}