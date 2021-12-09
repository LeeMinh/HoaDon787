using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Utils
{
    public enum GenderEnum
    {
        Female = 0,
        Male = 1,
        Hifi = 2
    }
    /// <summary>
    /// Trạng thái phòng
    /// </summary>
    public enum RoomStatus
    {
        /// <summary>
        /// Sẵn sàng cho thuê
        /// </summary>
        Available = 1,
        /// <summary>
        /// Đã được đặt trước
        /// </summary>
        Booked = 2,
        /// <summary>
        /// Đang được sử dụng
        /// </summary>
        InUse = 3,
        /// <summary>
        /// Đang dọn dẹp
        /// </summary>
        Cleaning = 4,
        /// <summary>
        /// Tạm thời ngưng phục vụ
        /// </summary>
        Pending = 5
    }

    public enum RoomType
    {
        Single = 1,
        TwoBed = 2,
        ThreeBed = 3,
        FourBed = 4
    }

    public enum ViewChartType
    {
        MonthChart = 1,
        YearChart = 2
    }
    public enum FilterOption
    {
        /// <summary>
        /// Tất cả
        /// </summary>
        All = 0,
        /// <summary>
        /// nhỏ hơn hoặc bằng 10%
        /// </summary>
        Under10Percent = 1,
        /// <summary>
        /// từ 10% đến 50%
        /// </summary>
        Bigger10Equal50 = 2,
        /// <summary>
        /// Lớn hơn 50% và nhỏ hơn hoặc bằng 100
        /// </summary>
        Bigger50Equal100 = 3,
        /// <summary>
        /// Lớn hơn 100
        /// </summary>
        Bigger100 = 4
    }
    public enum OrderBy
    {
        /// <summary>
        /// Phong trong den phong dang su dung
        /// </summary>
        PreferEmpty = 1,
        /// <summary>
        /// Phong dang co khach den phong trong
        /// </summary>
        PreferInUse = 2,
        /// <summary>
        /// Khong sap xep
        /// </summary>
        None = 0
    }

    /// <summary>
    /// Trạng thái vi phạm
    /// </summary>
    public enum Violate
    {
        /// <summary>
        /// Bình thường
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Vi phạm
        /// </summary>
        Violate = 1,
        /// <summary>
        /// Cảnh báo
        /// </summary>
        Warning = 2
    }

    public enum RentType
    {
        /// <summary>
        /// Binh thuong nghi theo ngay
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Chi qua dem
        /// </summary>
        OverNight = 2,
        /// <summary>
        /// nghi theo gio
        /// </summary>
        HourStep = 3
    }

    /// <summary>
    /// Kieu tra tien thanh toan
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// Tiền mặt
        /// </summary>
        CashOnHand = 1,
        /// <summary>
        /// Tiền gửi ngân hàng
        /// </summary>
        CashInBank = 2,
        /// <summary>
        /// Cả 2 hình thức
        /// </summary>
        Both = 3
    }

    /// <summary>
    /// Phan loai khach
    /// </summary>
    public enum CustomerType
    {
        /// <summary>
        /// Khach le
        /// </summary>
        Single = 1,
        /// <summary>
        /// Doan khach
        /// </summary>
        Multi = 2,
        /// <summary>
        /// Khach le vip
        /// </summary>
        SingleVIP = 3,
        /// <summary>
        /// Doan khach vip
        /// </summary>
        MultiVIP = 4
    }
    /// <summary>
    /// Trang thai giao dich
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Dang dung
        /// </summary>
        InUse = 0,
        /// <summary>
        /// check out mot it
        /// </summary>
        CheckoutHalf = 1,
        /// <summary>
        /// check out ca
        /// </summary>
        CheckoutAll = 2,
        /// <summary>
        /// thanh toan chua xong
        /// </summary>
        PaymentHalf = 3,
        /// <summary>
        /// Thanh toan het
        /// </summary>
        Payment = 4
    }
    public enum InvoiceStatus
    {
        /// <summary>
        /// Hóa đơn chưa xuất, đang xây dựng
        /// </summary>
        Building = 0,
        /// <summary>
        /// Đã xuất
        /// </summary>
        Exported = 1
    }
    public enum InvoiceAction
    {
        AddNew = 1,
        Update = 2,
        PrintInvoice = 3,
        PrintInvoiceDetail = 4,
        ExportInvoice = 5,
        CancelExport = 6, 
        PrintEInvoice = 7
    }

    public enum TicketType
    {
        Dometic = 1,
        Global = 2,
        Changed = 3,
        Return = 4,
        All = 5
    }

    public enum InvoiceType
    {
        Normal = 1,
        Return = 2
    }
}