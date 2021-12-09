using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AirlineInvoice.Utils
{
    public class DataComboSource
    {
        public static System.Collections.Hashtable CustomerTypeMapping = new System.Collections.Hashtable() { 
                {(byte)Utils.CustomerType.Single, "Khách lẻ"},
                {(byte)Utils.CustomerType.Multi, "Khách đoàn"},
                {(byte)Utils.CustomerType.SingleVIP, "Khách lẻ VIP"},
                {(byte)Utils.CustomerType.MultiVIP, "Khách đoàn VIP"}
            };
        public static System.Collections.Hashtable TransactionStatusMapping = new System.Collections.Hashtable() { 
    {(int)Utils.TransactionStatus.InUse, "Đang sử dụng"},
    {(int)Utils.TransactionStatus.CheckoutHalf, "Checkout một phần"},
    {(int)Utils.TransactionStatus.CheckoutAll, "Đã checkout"},
    {(int)Utils.TransactionStatus.PaymentHalf, "Thanh toán một phần"},
    {(int)Utils.TransactionStatus.Payment, "Đã thanh toán"}
    };
        /// <summary>
        /// Kiểu xem biểu đồ theo dõi vi phạm theo tháng hay năm
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> ViewChartType()
        {
            return new List<SelectListItem>() { 
            new SelectListItem(){ Selected = true, Text="Trong tháng", Value = ((int)Utils.ViewChartType.MonthChart).ToString()},
            new SelectListItem(){  Text="Cả năm", Value = ((int)Utils.ViewChartType.YearChart).ToString()}
            };
        }

        public static List<SelectListItem> RoomType()
        {
            return new List<SelectListItem>() { 
            new SelectListItem(){ Selected = true, Text="Phòng đơn", Value = ((int)Utils.RoomType.Single).ToString()},
            new SelectListItem(){ Selected = true, Text="Phòng đôi", Value = ((int)Utils.RoomType.TwoBed).ToString()},
            new SelectListItem(){ Selected = true, Text="Phòng ba", Value = ((int)Utils.RoomType.ThreeBed).ToString()},
            new SelectListItem(){ Selected = true, Text="Phòng bốn", Value = ((int)Utils.RoomType.FourBed).ToString()}
            };
        }
        public static List<SelectListItem> Gender()
        {
            return new List<SelectListItem>() { 
            new SelectListItem(){ Selected = true, Text="Nam", Value = "true"},
            new SelectListItem(){ Text="Nữ", Value = "false"}
            };
        }

        public static List<SelectListItem> PaymentTypeList(int paymentType = 3)
        {
            return new List<SelectListItem>() { 
            new SelectListItem(){ Selected = paymentType == (int)PaymentType.CashOnHand, Text="Tiền mặt", Value = "1"},
            new SelectListItem(){ Selected = paymentType == (int)PaymentType.CashInBank, Text="Chuyển khoản", Value = "2"},
            new SelectListItem(){ Selected = paymentType == (int)PaymentType.Both, Text="TM/CK", Value = "3"}
            };
        }
        public static List<SelectListItem> CustomerType()
        {
            var lst = new List<SelectListItem>() { 
            new SelectListItem(){ Selected = true, Text="Khách lẻ", Value = ((int)Utils.CustomerType.Single).ToString()},
            new SelectListItem(){ Selected = true, Text="Khách đoàn", Value = ((int)Utils.CustomerType.Multi).ToString()},
            new SelectListItem(){ Selected = true, Text="Khách lẻ VIP", Value = ((int)Utils.CustomerType.SingleVIP).ToString()},
            new SelectListItem(){ Selected = true, Text="Khách đoàn VIP", Value = ((int)Utils.CustomerType.MultiVIP).ToString()}
            };
            return lst;
        }
        public static List<SelectListItem> RentType()
        {
            return new List<SelectListItem>() { 
            new SelectListItem(){ Selected = true, Text="Theo ngày", Value = ((int)Utils.RentType.Normal).ToString()},
            new SelectListItem(){ Selected = true, Text="Qua đêm", Value = ((int)Utils.RentType.OverNight).ToString()},
            new SelectListItem(){ Selected = true, Text="Theo giờ", Value = ((int)Utils.RentType.HourStep).ToString()},
            };
        }

        /// <summary>
        /// Danh sách năm
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> YearCombo()
        {
            var lst = new List<SelectListItem>();
            var n = DateTime.Now.Year;
            for (int i = n; i >= 1990 ; i--)
            {
                lst.Add(new SelectListItem() { Selected = i == n ? true : false, Text = i.ToString(), Value = i.ToString() });
            }
            return lst;
        }

        /// <summary>
        /// Danh sách tháng
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> MonthCombo()
        {
            var lst = new List<SelectListItem>();
            for (int i = 1; i <=12 ; i++)
            {
                lst.Add(new SelectListItem() { Selected = i == DateTime.Now.Month ? true : false, Text = i.ToString(), Value = i.ToString() });
            }
            return lst;
        }

        /// <summary>
        /// Danh sách tháng
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> RateStar(int defaultValue)
        {
            var lst = new List<SelectListItem>();
            for (int i = 1; i <= 7; i++)
            {
                lst.Add(new SelectListItem() { Selected = i == defaultValue ? true : false, Text = i.ToString(), Value = i.ToString() });
            }
            return lst;
        }

        /// <summary>
        /// Số bản ghi trong một trang - phần phân trang
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> LoadPageSizeCustom()
        {
            return new List<SelectListItem>() { 
            new SelectListItem(){ Selected = true, Text="20", Value="20"},
            new SelectListItem(){  Text="40", Value="40"},
            new SelectListItem(){  Text="60", Value="60"},
            new SelectListItem(){  Text="80", Value="80"},
            new SelectListItem(){  Text="100", Value="100"}
            };
        }
        /// <summary>
        /// Dannh muc khach san
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> LoadUserDictionary(string userID = "", bool addAll = false)
        {
           
            var lstUser = Membership.GetAllUsers();
                var lst = new List<SelectListItem>();
                var isAccounting = Roles.IsUserInRole(Utils.CommonFunction._Accounting);
                if (isAccounting && !Roles.IsUserInRole(CommonFunction._AdminRole) && Models.userprofile.CurrentUser.AgentBranchID > 0)
                {
                    var currentUser = Models.userprofile.CurrentUser;
                    lst.Add(new SelectListItem()
                    {
                        Text = currentUser.UserName,
                        Value = Membership.GetUser().ProviderUserKey.ToString(),
                        Selected =  true 
                    });
                    return lst;
                }
                if (addAll)
                {
                    lst.Add(new SelectListItem() { Text = "Tất cả --", Value = "", Selected = string.IsNullOrEmpty(userID) ? true : false });
                }
                if (lstUser != null)
                    foreach (MembershipUser item in lstUser)
                    {
                        if (isAccounting && item.ProviderUserKey.ToString().Equals(userID))
                        {
                            lst.Add(new SelectListItem()
                            {
                                Text = item.UserName,
                                Value = item.ProviderUserKey.ToString(),
                                Selected = item.ProviderUserKey.ToString().Equals(userID) ? true : false
                            });
                        }
                        else
                        {
                            if (Models.userprofile.CurrentUser.AgentID == Models.userprofile.GetUser(item.UserName).AgentID)
                                lst.Add(new SelectListItem()
                                {
                                    Text = item.UserName,
                                    Value = item.ProviderUserKey.ToString(),
                                    Selected = item.ProviderUserKey.ToString().Equals(userID) ? true : false
                                });
                            else
                                continue;
                        }
                    }
                return lst;
        }
       
        /// <summary>
        /// Dannh muc khach san
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> LoadAgentDictionary(int agentID = 0, bool addAll = false)
        {
            using (var db = new AirlineInvoice.Models.InvoiceContext())
            {
                var lstHotel = db.Agents.ToList();
                var lst = new List<SelectListItem>();
                if (addAll)
                {
                    lst.Add(new SelectListItem() { Text = "Tất cả --", Value = "0", Selected = agentID == 0 ? true : false });
                }
                if (lstHotel != null)
                    foreach (var item in lstHotel)
                    {
                        lst.Add(new SelectListItem() { Text = item.AgentName, Value = item.AgentID.ToString(), 
                            Selected = item.AgentID == agentID ? true : false });
                    }
                return lst;
            }
        }

        /// <summary>
        /// Dannh muc khach san
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> LoadBranchDictionary(int branchID = 0, bool addAll = false)
        {
            using (var db = new AirlineInvoice.Models.InvoiceContext())
            {
                var lstBranches = db.AgentBranches.Where(x=> x.AgentID == Models.userprofile.CurrentUser.AgentID)
                    .ToList();
                var lst = new List<SelectListItem>();
                if (addAll)
                {
                    lst.Add(new SelectListItem() { Text = "Tất cả --", Value = "0", Selected = branchID == 0 ? true : false });
                }
                if (lstBranches != null)
                    foreach (var item in lstBranches)
                    {
                        lst.Add(new SelectListItem()
                        {
                            Text = item.BranchName,
                            Value = item.AgentBranchID.ToString(),
                             Selected = item.AgentBranchID == branchID ? true : false
                        });
                    }
                return lst;
            }
        }

        public static List<SelectListItem> LoadCurrency(int currency = 0)
        {
            using (var db = new AirlineInvoice.Models.InvoiceContext())
            {
                var lstCurrenies = db.Currencies.OrderBy(x=>x.SortOrder).ToList();
                var lst = new List<SelectListItem>();

                if (lstCurrenies != null)
                    foreach (var item in lstCurrenies)
                    {
                        lst.Add(new SelectListItem() { Text = item.CurrencyCode, Value = item.CurrencyCode, Selected = item.CurrencyCode.Equals(currency) ? true : false });
                    }
                return lst;
            }
        }

        /// <summary>
        /// Kiểu sắp xếp
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> RoomStatus(bool addAll = true)
        {
            var lst = new List<SelectListItem>();
            if (addAll)
            {
                lst.Add(new SelectListItem() { Selected = true, Text = "Tất cả --", Value = "0" });
            }
            lst.Add(new SelectListItem() {  Text = "Còn trống", Value = ((int)Utils.RoomStatus.Available).ToString() });
            lst.Add( new SelectListItem() { Text = "Đã được thuê", Value = ((int)Utils.RoomStatus.InUse).ToString() });
            lst.Add( new SelectListItem() { Text = "Đã đặt trước", Value = ((int)Utils.RoomStatus.Booked).ToString() });
            lst.Add( new SelectListItem() { Text = "Đang dọn dẹp", Value = ((int)Utils.RoomStatus.Cleaning).ToString() });
            lst.Add( new SelectListItem() { Text = "Tạm ngừng phục vụ", Value = ((int)Utils.RoomStatus.Pending).ToString() });
            return lst;
        }
        /// <summary>
        /// Kiểu sắp xếp
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> AxleSummary()
        {
            
            return new List<SelectListItem>() { 
                new SelectListItem(){ Selected = true, Text="Tất cả", Value="0"},
            new SelectListItem(){ Text="2", Value="2"},
            new SelectListItem(){ Text="3", Value="3"},
            new SelectListItem(){ Text="4", Value="4"},
            new SelectListItem(){ Text="5", Value="5"},
            new SelectListItem(){ Text="6", Value="6"}
            };
        }

        /// <summary>
        /// Kiểu hoa don
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> InvoiceType(int invoiceType = 0, bool addAll = false)
        {
            var lst = new List<SelectListItem>();
            if (addAll)
            {
                lst.Add(new SelectListItem() { Text = "Tất cả --", Value = "0", Selected = invoiceType == 0 ? true : false });
            }
            lst.Add(new SelectListItem()
            {
                Text = "Hóa đơn xuất",
                Selected = invoiceType == (int)Utils.InvoiceType.Normal ? true : false,
                Value = ((int)Utils.InvoiceType.Normal).ToString()
            });
            lst.Add(new SelectListItem()
            {
                Text = "Hóa đơn hoàn",
                Selected = invoiceType == (int)Utils.InvoiceType.Return ? true : false,
                Value = ((int)Utils.InvoiceType.Return).ToString()
            });
            return lst;
        }
    }
}