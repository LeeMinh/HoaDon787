using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Profile;
using System.Web.Security;

namespace AirlineInvoice.Models
{
    public class userprofile : ProfileBase
    {
        public static userprofile CurrentUser
        {
            get
            {
                var user = Membership.GetUser();
                if (user != null)
                    return (userprofile)
                           ProfileBase.Create(Membership.GetUser().UserName);
                else return null;
            }
        }

       public static  userprofile GetUser(string userName)
        {
            return (userprofile)
                           ProfileBase.Create(userName);
        }
       public static userprofile GetUser(Guid userID)
       {
           var users = Membership.GetAllUsers();
           MembershipUser user = null;
           foreach (MembershipUser item in users)
           {
               if (item.ProviderUserKey.ToString().Equals(userID.ToString()))
               {
                   user = item;
                   break;
               }
           }
           if (user != null)
               return (userprofile)
                              ProfileBase.Create(user.UserName);
           else return (userprofile)ProfileBase.Create(string.Empty);
       }
        [Required(ErrorMessage="Họ tên không được để trống.")]
        public string FullName
        {
            get { return ((string)(base["FullName"])); }
            set { base["FullName"] = value; Save(); }
        }

        public string Address
        {
            get { return ((string)(base["Address"])); }
            set { base["Address"] = value; Save(); }
        }
        public int Gender
        {
            get { return ((int)(base["Gender"])); }
            set { base["Gender"] = value; Save(); }
        }
        public string Tel
        {
            get { return ((string)(base["Tel"])); }
            set { base["Tel"] = value; Save(); }
        }
        public string AvatarLink
        {
            get { return ((string)(base["AvatarLink"])); }
            set { base["AvatarLink"] = value; Save(); }
        }
        public int AgentID
        {
            get { return ((int)(base["AgentID"])); }
            set { base["AgentID"] = value; Save(); }
        }
        public int AgentBranchID
        {
            get { return ((int)(base["AgentBranchID"])); }
            set { base["AgentBranchID"] = value; Save(); }
        }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage="Tên đăng nhập không được để trống.")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [StringLength(100, ErrorMessage = "{0} tối thiểu {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu mới")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "Mật khẩu mới không khớp nhau.")]
        public string ConfirmPassword { get; set; }
    }
    public class UserChangePasswordModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu cũ không được để trống.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} tối thiểu {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu mới")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "Mật khẩu mới nhập không khớp nhau")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, ErrorMessage = "{0} ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Nhập lại mật khẩu không đúng.")]
        public string ConfirmPassword { get; set; }
        
         [Display(Name = "Họ và tên")]
         [Required(ErrorMessage = "Họ và tên không được để trống.")]
        public string FullName { get; set; }

         [Display(Name = "Địa chỉ")]
         public string Address { get; set; }

         [Display(Name = "Số điện thoại")]
         public string Tel { get; set; }
         public Utils.GenderEnum Gender { get; set; }
         public string AvatarLink { get; set; }
         public int AgentID { get; set; }
         public int AgentBranchID { get; set; }
         public UserInRoles userInRoles { get; set; }
         public string AgentName { get; set; }
        [Display(Name = "Người dùng cấp 2")]
         public bool IsAccounting { get; set; }
    }
}
