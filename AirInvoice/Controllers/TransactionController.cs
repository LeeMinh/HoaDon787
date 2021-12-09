using AirlineInvoice.Models;
using AirlineInvoice.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirlineInvoice.Controllers
{
    public class TransactionController : BaseController
    {
        #region Transaction

      
        //
        // GET: /Transaction/
        public ActionResult Index()
        {
            return View(GetListTransaction(1,10,""));
        }

        public ActionResult SearchTransaction(string searchText)
        {
            ViewBag.SearchText = searchText;
            return View("index", GetListTransaction(1, 10, searchText));
        }

        public PartialViewResult GetTransactionData(int currentPage, int pageSize, string searchText)
        {
            return PartialView("TransactionData", GetListTransaction(currentPage, pageSize, searchText));
        }
        private List<Models.Transaction> GetListTransaction(int currentPage, int pageSize, string SearchText)
        {
            using (var db = new Models.InvoiceContext())
            {
                var lstTransactionID = db.Database.SqlQuery<int>("proc_TransactionHomeSearch @SearchText", new SqlParameter("@SearchText", SearchText)).ToList();
                var model = db.Transactions.Where(x => lstTransactionID.Contains(x.TransactionID)).OrderByDescending(x=>x.StartTime).ToPagedList(currentPage, pageSize);
                ViewBag.CurrentPage = currentPage;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRow = model.Count;
                return model;
            }
        }
        private TransactionModel GetTransaction(int id)
        {
            using (var db = new InvoiceContext())
            {
                var obj = db.Transactions.FirstOrDefault(x => x.TransactionID == id);
                if (obj != null)
                {
                    var roomCount = db.TransactionRooms.Count(x => x.TransactionID == id);
                    var customerCount = db.TransactionRoomCustomers.Count(x => x.TransactionID == id);
                    var serviceCount = db.TransactionServices.Count(x => x.TransactionID == id);
                    return new TransactionModel()
                    {
                        Transaction = obj,
                        CustomerCount = customerCount,
                        RoomCount = roomCount,
                        ServiceCount = serviceCount
                    };
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionResult AddTransaction()
        {
            var model = new TransactionModel()
            {
                Transaction = new Transaction() { StartTime = DateTime.Now,
                     EndTime = DateTime.Now.AddDays(1),
                CustomerType = 1
                },
                ServiceCount = 0,
                RoomCount = 0,
                CustomerCount = 0
            };
            return View(model);
        }
        /// <summary>
        /// Them, sua giao dich
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransaction(TransactionModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new InvoiceContext())
                {
                    if (model.Transaction.CustomerID <= 0)
                    {
                        var cusID = UpdateCustomer(model.Transaction.IdentityNumber, model.Transaction.CustomerName, model.Transaction.Phone, model.Transaction.CustomerType);
                        if (cusID > 0) model.Transaction.CustomerID = cusID;
                    }
                    db.Transactions.Add(model.Transaction);
                    db.SaveChanges();
                    return RedirectToAction("AddRoom", new { TransID = Utils.CommonFunction.Shrink(model.Transaction.TransactionID) });
                }
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult EditTransaction(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            // sua giao dich
            if (key > 0)
            {
                var model = GetTransaction(key);
                if (model != null && !model.Transaction.IsCheckOut)
                {
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            // ko co thi tro ve danh sach
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTransaction(TransactionModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new InvoiceContext())
                {
                    if (model.Transaction.CustomerID <= 0)
                    {
                        var cusID = UpdateCustomer(model.Transaction.IdentityNumber, model.Transaction.CustomerName, model.Transaction.Phone, model.Transaction.CustomerType);
                        if (cusID > 0) model.Transaction.CustomerID = cusID;
                    }
                    var obj = db.Transactions.FirstOrDefault(x => x.TransactionID == model.Transaction.TransactionID);
                    if (obj.IsCheckOut) return RedirectToAction("Index");
                    obj.CustomerID = model.Transaction.CustomerID;
                    obj.CurrencyCode = model.Transaction.CurrencyCode;
                    obj.CustomerName = model.Transaction.CustomerName;
                    obj.CustomerType = model.Transaction.CustomerType;
                    obj.Description = model.Transaction.Description;
                    obj.EndTime = model.Transaction.EndTime;
                    obj.StartTime = model.Transaction.StartTime;
                    obj.Phone = model.Transaction.Phone;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { TransactionID = Utils.CommonFunction.Shrink(model.Transaction.TransactionID) });
                }
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult DeleteTransaction(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.Transactions.FirstOrDefault(x => x.TransactionID == key);
                if (obj != null)
                {
                    db.Transactions.Remove(obj);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        public PartialViewResult TransactionDetail(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var model = new TransactionModel();
                model.Transaction = db.Transactions.FirstOrDefault(x => x.TransactionID == key);
                model.TransactionRooms = GetTransactionRoomList(key);
                model.Customers = db.TransactionRoomCustomers.Where(x => x.TransactionID == key).ToList();
                model.Services = db.TransactionServices.Where(x => x.TransactionID == key).ToList();
                model.CustomerCount = model.Customers.Count;
                model.RoomCount = model.TransactionRooms.Count;
                model.ServiceCount = model.Services.Count;
                return PartialView(model);
            }
        }
        #endregion

        #region Room
        public ActionResult TransactionRoom(string TransactionID)
        {
            var model = new TransactionModel();
            var key = Utils.CommonFunction.Expand(TransactionID);
            using (var db = new InvoiceContext())
            {
                var transaction = db.Transactions.FirstOrDefault(x => x.TransactionID == key);
                if (transaction != null)
                {
                    model.Transaction = transaction;
                    model.TransactionRooms = GetTransactionRoomList(key);
                    model.TransactionRooms.ForEach((item) => {
                        var room = db.Rooms.FirstOrDefault(x => x.RoomID == item.RoomID);
                        if (room != null)
                        {
                            item.RoomCode = room.RoomCode;
                            item.Floor = room.Floor;
                        }
                    });
                    model.CustomerCount = db.TransactionRoomCustomers.Count(x => x.TransactionID == key);
                    model.RoomCount = db.TransactionRooms.Count(x => x.TransactionID == key);
                    model.ServiceCount = db.TransactionServices.Count(x => x.TransactionID == key);
                    if (model.TransactionRooms == null || model.TransactionRooms.Count <= 0)
                    {
                        return RedirectToAction("AddRoom", new { TransID = CommonFunction.Shrink(model.Transaction.TransactionID) });
                    }
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        public ActionResult AddRoom(string TransID)
        {
            using (var db = new Models.InvoiceContext())
            {
                var key = Utils.CommonFunction.Expand(TransID);
                var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == key);
                if (tran != null)
                {
                    if (tran.IsCheckOut) return RedirectToAction("Index");
                    ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == key);
                    ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == key);
                    ViewBag.CountCustomer = db.TransactionRoomCustomers.Count(x => x.TransactionID == key);
                    var model = new Models.TransactionRoom()
                    {
                        TransactionID = tran.TransactionID,
                        StartTime = tran.StartTime,
                        EndTime = tran.EndTime,
                        CustomerID = tran.CustomerID,
                        CustomerName = tran.CustomerName,
                        IdentityNumber = tran.IdentityNumber,
                        PersonCount = 1,
                        RentType = (byte)Utils.RentType.Normal,
                         Transaction = tran
                    };
                    return View(model);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoom(TransactionRoom model)
        {
            using (var db = new InvoiceContext())
            {
                ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountCustomer = db.TransactionRoomCustomers.Count(x => x.TransactionID == model.TransactionID);
                var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == model.TransactionID);
                if (tran == null || tran.IsCheckOut) return RedirectToAction("Index");
                model.Transaction = tran;
                if (ModelState.IsValid)
                {
                    model.BookingTime = DateTime.Now;
                    if (model.CustomerID <= 0)
                    {
                        var cusID = UpdateCustomer(model.IdentityNumber, model.CustomerName, model.Phone, model.CustomerType);
                        if (cusID > 0) model.CustomerID = cusID;
                    }
                 
                    
                    // cap nhat trang thai phong
                    var room = db.Rooms.FirstOrDefault(x => x.RoomID == model.RoomID);
                    // cap nhat trang thai phong
                    if (room != null)
                    {
                        room.Status = (int)Utils.RoomStatus.InUse;
                        model.Price = Utils.CommonFunction.GetRoomPrice(model.RentType, room);
                    }
                    db.TransactionRooms.Add(model);
                    var i = db.SaveChanges();
                    return RedirectToAction("TransactionRoom", new { TransactionID = Utils.CommonFunction.Shrink(model.TransactionID) });
                }
                else
                {
                    return View(model);
                }
            }
        }

        /// <summary>
        /// Them phong moi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoom(TransactionRoom model)
        {
            using (var db = new InvoiceContext())
            {
                ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountCustomer = db.TransactionRoomCustomers.Count(x => x.TransactionID == model.TransactionID);
                var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == model.TransactionID);
                if (tran.IsCheckOut) return RedirectToAction("Index");
                if (ModelState.IsValid)
                {
                    var obj = db.TransactionRooms.FirstOrDefault(x => x.TransactionRoomID == model.TransactionRoomID);
                    
                    obj.RentType = model.RentType;
                    obj.StartTime = model.StartTime;
                    obj.EndTime = model.EndTime;
                    obj.Description = model.Description;
                    //obj.CustomerID = model.CustomerID;
                    //obj.CustomerName = model.CustomerName;
                    //obj.CustomerType = model.CustomerType;
                    //obj.IdentityNumber = model.IdentityNumber;
                    obj.DaySpending = model.DaySpending;
                    if (model.CustomerID <= 0)
                    {
                        var cusID = UpdateCustomer(model.IdentityNumber, model.CustomerName, model.Phone, model.CustomerType);
                        if (cusID > 0) model.CustomerID = cusID;
                    }
                    if (model.OldRoomID != model.RoomID)
                    {
                        var oldRoom = db.Rooms.FirstOrDefault(x => x.RoomID == model.RoomID);
                        oldRoom.Status = (byte)Utils.RoomStatus.Available;
                    }
                    obj.RoomID = model.RoomID;
                    // cap nhat trang thai phong
                    var room = db.Rooms.FirstOrDefault(x => x.RoomID == model.RoomID);
                    if (room != null)
                    {
                        room.Status = (int)Utils.RoomStatus.InUse;
                        obj.Price = Utils.CommonFunction.GetRoomPrice(model.RentType, room);
                    }
                    db.SaveChanges();

                    return RedirectToAction("TransactionRoom", new { TransactionID = Utils.CommonFunction.Shrink(model.TransactionID) });
                }
                else
                {
                    ViewBag.Room = db.Rooms.FirstOrDefault(x => x.RoomID == model.RoomID);
                    model.Transaction = tran;
                    return View(model);
                }
            }
        }

        public ActionResult EditRoom(string id)
        {
            using (var db = new Models.InvoiceContext())
            {
                var key = Utils.CommonFunction.Expand(id);
                var Transroom = db.TransactionRooms.FirstOrDefault(x => x.TransactionRoomID == key);
                ViewBag.Room = db.Rooms.FirstOrDefault(x => x.RoomID == Transroom.RoomID);
                if (Transroom != null)
                {
                    var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == Transroom.TransactionID);
                    if (tran == null || tran.IsCheckOut) return RedirectToAction("Index");
                    ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == Transroom.TransactionID);
                    ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == Transroom.TransactionID);
                    ViewBag.CountCustomer = db.TransactionRoomCustomers.Count(x => x.TransactionID == Transroom.TransactionID);
                    return View(Transroom);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
        }
        public bool CheckRoomValid(int TransID, int SelectedRoomID)
        {
            using (var db = new InvoiceContext())
            {
                var room = db.Rooms.FirstOrDefault(x => x.RoomID == SelectedRoomID);
                if (room == null || (room.Status != (byte)Utils.RoomStatus.Available  && room.Status != (byte)Utils.RoomStatus.Cleaning)) return false;
               
                var i = db.TransactionRooms.Count(x => x.TransactionID == TransID 
                    && x.RoomID == SelectedRoomID) <= 0;
                return i;
            }
        }

        public ActionResult DeleteTransactionRoom(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.TransactionRooms.FirstOrDefault(x => x.TransactionRoomID == key);
                int transID = -1;
                if (obj != null)
                {
                    transID = obj.TransactionID;
                    db.TransactionRooms.Remove(obj);
                    var room = db.Rooms.FirstOrDefault(x => x.RoomID == obj.RoomID);
                    // cap nhat trang thai phong
                    if (room != null)
                    {
                        room.Status = (int)Utils.RoomStatus.Available;
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("TransactionRoom", new  { TransactionID = Utils.CommonFunction.Shrink(transID)});
            }
        }

        public PartialViewResult GetTransactionRoomCustomer(string TransactionRoomID)
        {
            using (var db = new InvoiceContext())
            {
                var key = Utils.CommonFunction.Expand(TransactionRoomID);
                var lst = db.TransactionRoomCustomers.Where(x => x.TransactionRoomID == key).ToList();
                var transRoom = db.TransactionRooms.FirstOrDefault(x => x.TransactionRoomID == key);
                var roomCode = string.Empty;
                if (transRoom != null)
                {
                    var room = db.Rooms.FirstOrDefault(x => x.RoomID == transRoom.RoomID);
                    if (room != null)
                    {
                        roomCode = room.RoomCode;
                    }
                }
                if (lst != null)
                {
                    lst.ForEach(item => { item.RoomCode = roomCode; });
                }
                return PartialView(lst);
            }
        }
        #endregion

        #region Customer
        /// <summary>
        /// cap nhat khach hang tu dong vao danh sach
        /// </summary>
        /// <param name="identityNumber"></param>
        /// <param name="customerName"></param>
        /// <param name="phone"></param>
        /// <returns>id khach hang moi</returns>
        public static int UpdateCustomer(string identityNumber, string customerName, string phone, byte custype = 1)
        {
            if (string.IsNullOrEmpty(identityNumber)) return 0;
            using (var db = new InvoiceContext())
            {
                var obj = db.Customers.FirstOrDefault(x => x.IdentityNumber.Equals(identityNumber));
                if (obj != null) return obj.CustomerID;
                var newCustomer = new Customer()
                {
                    CustomerType = custype,
                    FullName = customerName,
                    IdentityNumber = identityNumber,
                    Phone = phone
                };
                db.Customers.Add(newCustomer);
                db.SaveChanges();
                return newCustomer.CustomerID;
            }
        }

        public ActionResult TransactionCustomer(string TransactionID)
        {
            var key = CommonFunction.Expand(TransactionID);
            using (var db = new InvoiceContext())
            {
                var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == key);
                if (tran == null)
                {
                    return RedirectToAction("Index");
                }
                var model = new TransactionModel();
                model.Customers = db.TransactionRoomCustomers.Where(x => x.TransactionID == key).ToList();
                model.ServiceCount = db.TransactionServices.Count(x => x.TransactionID == key);
                model.TransactionRooms = GetTransactionRoomList(key);
                model.Transaction = tran;
                if (model.Customers == null || model.Customers.Count <= 0)
                {
                    return RedirectToAction("AddCustomer", new { TransID = CommonFunction.Shrink(model.Transaction.TransactionID) });
                }
                return View(model);
            }
        }

        public ActionResult AddCustomer(string TransID)
        {
            var key = CommonFunction.Expand(TransID);
            using (var db = new InvoiceContext())
            {
                ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == key);
                ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == key);
                ViewBag.CountCustomer = db.TransactionRoomCustomers.Count(x => x.TransactionID == key);
                var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == key);
                if (tran == null || tran.IsCheckOut)
                {
                    return RedirectToAction("Index");
                }
                var model = new TransactionRoomCustomer();
                model.TransactionID = key;
                model.Transaction = tran;
                model.StartTime = DateTime.Now;
                model.EndTime = DateTime.Today.AddDays(1.5);
                model.TransactionRooms = GetTransactionRoomList(key);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomer(TransactionRoomCustomer model)
        {
            using (var db = new InvoiceContext())
            {
                ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountCustomer = db.TransactionRoomCustomers.Count(x => x.TransactionID == model.TransactionID);
               
                    var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == model.TransactionID);
                    if (tran == null || tran.IsCheckOut) return RedirectToAction("Index");
                    model.Transaction = tran;

                    if (ModelState.IsValid)
                    {
                        if (model.CustomerID <= 0)
                        {

                            var cusID = UpdateCustomer(model.IdentityNumber, model.CustomerName, model.Phone, tran.CustomerType);
                            if (cusID > 0) model.CustomerID = cusID;
                        }
                        // check valid
                        if (model.TransactionRoomID <= 0 || model.CustomerID <= 0) return View(model);

                        db.TransactionRoomCustomers.Add(model);
                        var i = db.SaveChanges();
                        return RedirectToAction("TransactionCustomer", new { TransactionID = Utils.CommonFunction.Shrink(model.TransactionID) });
                    }
                    else
                    {
                        model.TransactionRooms = GetTransactionRoomList(model.TransactionID);
                        return View(model);
                    }
            }
        }

        public ActionResult EditCustomer(string id)
        {
            using (var db = new InvoiceContext())
            {
                var key = CommonFunction.Expand(id);
                var model = db.TransactionRoomCustomers.FirstOrDefault(x => x.TransactionCustomerID == key);
                if (model == null) return RedirectToAction("index");
                var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == model.TransactionID);
                if (tran == null || tran.IsCheckOut) return RedirectToAction("Index");
                if (tran.IsCheckOut) return RedirectToAction("Index");
                ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountCustomer = db.TransactionRoomCustomers.Count(x => x.TransactionID == model.TransactionID);
                model.TransactionRooms = GetTransactionRoomList(model.TransactionID);
                model.Transaction = tran;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(TransactionRoomCustomer model)
        {
            using (var db = new InvoiceContext())
            {
                var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == model.TransactionID);
                if (tran == null || tran.IsCheckOut) return RedirectToAction("Index");
                ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == model.TransactionID);
                ViewBag.CountCustomer = db.TransactionRoomCustomers.Count(x => x.TransactionID == model.TransactionID);
                model.Transaction = tran;
                if (ModelState.IsValid)
                {
                    var obj = db.TransactionRoomCustomers.FirstOrDefault(x => x.TransactionCustomerID == model.TransactionCustomerID);
                    if (obj == null) return RedirectToAction("TransactionCustomer");
                    obj.TransactionID = model.TransactionID;
                    obj.TransactionRoomID = model.TransactionRoomID;
                    obj.CustomerID = model.CustomerID;
                    obj.CustomerName = model.CustomerName;
                    obj.IdentityNumber = model.IdentityNumber;
                    obj.Phone = model.Phone;
                    obj.StartTime = model.StartTime;
                    obj.EndTime = model.EndTime;
                    obj.Description = model.Description;
                    db.SaveChanges();
                    return RedirectToAction("TransactionCustomer", new { TransactionID = CommonFunction.Shrink(model.TransactionID) });
                }
                else
                {
                    return View(model);
                }
            }
        }
        #endregion

        #region service
        public ActionResult TransactionService(string TransactionID)
        {
            var model = new TransactionModel();
            var key = Utils.CommonFunction.Expand(TransactionID);
            using (var db = new InvoiceContext())
            {
                var transaction = db.Transactions.FirstOrDefault(x => x.TransactionID == key);
                if (transaction != null)
                {
                    model.CustomerCount = db.TransactionRoomCustomers.Count(x => x.TransactionID == key);
                    model.RoomCount = db.TransactionRooms.Count(x => x.TransactionID == key);
                    model.ServiceCount = db.TransactionServices.Count(x => x.TransactionID == key);
                    if (model.ServiceCount <= 0)
                    {
                        return RedirectToAction("AddService", new { TransID = TransactionID });
                    }
                    var rooms = GetTransactionRoomList(key);
                    var services = db.TransactionServices.Where(x => x.TransactionID == key).ToList();
                    model.Transaction = transaction;
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        public ActionResult AddService(string TransID)
        {
            using (var db = new Models.InvoiceContext())
            {
                var key = Utils.CommonFunction.Expand(TransID);
                var tran = db.Transactions.FirstOrDefault(x => x.TransactionID == key);
                if (tran != null && !tran.IsCheckOut)
                {
                    var model = new TransactionServiceModel();
                    model.TransactionRooms = GetTransactionRoomList(key);
                    model.Customers = db.TransactionRoomCustomers.Where(x => x.TransactionID == key).ToList();
                    ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == key);

                    var service = new Models.TransactionService()
                    {
                        TransactionID = tran.TransactionID,
                        CreateDate = DateTime.Now,
                        CustomerID = tran.CustomerID,
                        CustomerName = tran.CustomerName,
                        IdentityNumber = tran.IdentityNumber,
                        Amount = 0,
                        PaymentType = (byte)Utils.PaymentType.CashOnHand,
                        Quantity = 1
                    };
                    model.TransactionService = service;
                    return View(model);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
        }

        /// <summary>
        /// Them phong moi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddService(TransactionService model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new InvoiceContext())
                {
                    db.TransactionServices.Add(model);
                    if (model.CustomerID <= 0)
                    {
                        var cusID = UpdateCustomer(model.IdentityNumber, model.CustomerName, model.Phone, model.CustomerType);
                        if (cusID > 0) model.CustomerID = cusID;
                    }
                    var i = db.SaveChanges();
                    return RedirectToAction("TransactionService", new { TransactionID = Utils.CommonFunction.Shrink(model.TransactionID) });
                }
            }
            else
            {
                return View(model);
            }
        }

        /// <summary>
        /// Them phong moi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditService(TransactionService model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new InvoiceContext())
                {
                    if (model.CustomerID <= 0)
                    {
                        var cusID = UpdateCustomer(model.IdentityNumber, model.CustomerName, model.Phone, model.CustomerType);
                        if (cusID > 0) model.CustomerID = cusID;
                    }
                    var obj = db.TransactionServices.FirstOrDefault(x => x.TransactionServiceID == model.TransactionServiceID);
                    obj.CustomerID = model.CustomerID;
                    obj.CreateDate = model.CreateDate;
                    obj.CustomerName = model.CustomerName;
                    obj.CustomerType = model.CustomerType;
                    obj.Description = model.Description;
                    obj.IdentityNumber = model.IdentityNumber;
                    obj.Phone = model.Phone;
                    obj.Quantity = model.Quantity;
                    obj.IsPayment = model.IsPayment;
                    obj.TransactionRoomID = model.TransactionRoomID;
                    obj.ServiceID = model.ServiceID;
                    obj.ServiceName = model.ServiceName;
                    var i = db.SaveChanges();
                    return RedirectToAction("TransactionService", new { TransactionID = Utils.CommonFunction.Shrink(model.TransactionID) });
                }
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult EditService(string id)
        {
            using (var db = new Models.InvoiceContext())
            {
                var key = Utils.CommonFunction.Expand(id);
                var service = db.TransactionServices.FirstOrDefault(x => x.TransactionServiceID == key);
                if (service != null)
                {
                    ViewBag.CountRoom = db.TransactionRooms.Count(x => x.TransactionID == key);
                    ViewBag.CountService = db.TransactionServices.Count(x => x.TransactionID == key);
                    return View("UpdateService", service);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
        }

        public ActionResult DeleteTransactionService(string id)
        {
            var key = Utils.CommonFunction.Expand(id);
            using (var db = new InvoiceContext())
            {
                var obj = db.TransactionServices.FirstOrDefault(x => x.TransactionServiceID == key);
                int transID = -1;
                if (obj != null)
                {
                    transID = obj.TransactionID;
                    db.TransactionServices.Remove(obj);
                    db.SaveChanges();
                }
                return RedirectToAction("TransactionService", new { TransactionID = Utils.CommonFunction.Shrink(transID) });
            }
        }
        #endregion

        private List<TransactionRoom> GetTransactionRoomList(int transID)
        {
            using (var db = new InvoiceContext())
            {
                var lst = db.TransactionRooms.Where(x => x.TransactionID == transID).ToList();
                lst.ForEach(item => {
                    var room = db.Rooms.FirstOrDefault(x => x.RoomID == item.RoomID);
                    if (room != null)
                    {
                        item.RoomCode = room.RoomCode;
                        item.Floor = room.Floor;
                    }
                });
                lst.Sort((x, y) => x.RoomCode.CompareTo(y.RoomCode));
                return lst;
            }
        }
    }
}