using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Admin/Orders

        //danh sach vé đặt
        public ActionResult ListOrders(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
            //Status 2: đang chờ thanh toán tại quầy
            var orderss = db.Orders.Where(n => n.status == 2);
            foreach (var itemm in orderss)
            {
                if (itemm.ngay_mua + tinhgio <= DateTime.Now)
                {
                    itemm.status = 0;
                    db.Entry(itemm).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            return View(db.Orders.OrderByDescending(n => n.ngay_mua).ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult OrdDetail(int? id)
        {
            TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
            if (id == null)
            {
                return RedirectToAction("ListOrders");
            }
            try
            {
                Order ord = db.Orders.Find(id);
                if (ord == null)
                {
                    return RedirectToAction("ListOrders");
                }
                if (ord.status == 2 && ord.ngay_mua + tinhgio <= DateTime.Now)
                {
                    ord.status = 0;
                    db.Entry(ord).State = EntityState.Modified;
                    db.SaveChanges();
                }
                ViewBag.detail = db.CT_Orders.Where(n => n.orders_id == id);
                return View(ord);
            }
            catch (Exception)
            {
                return RedirectToAction("ListOrders");
            }
        }


        public ActionResult confirmPay(int? id)
        {
            var idord = Convert.ToInt32(id);
            Order ord = db.Orders.Find(idord);
            if (ord.status == 2)
            {
                ord.status = 1;
                db.Entry(ord).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Đã xác nhận thanh toán thành công!";
                return RedirectToAction("ListOrders", "Orders");
            }
            else
            {
                TempData["Error"] = "Lỗi!";
            }
            return RedirectToAction("ListOrders", "Orders");
        }


        public ActionResult confirmCancel(int id)
        {
            var idord = Convert.ToInt32(id);
            Order ord = db.Orders.Find(idord);
            if (ord.status == 2)
            {
                ord.status = 0;
                db.Entry(ord).State = EntityState.Modified;                
                db.SaveChanges();
                TempData["Warning"] = "Đã hủy vé thành công!";
                return RedirectToAction("ListOrders", "Orders");
            }
            else
            {
                TempData["Error"] = "Lỗi!";
            }
            return RedirectToAction("ListOrders", "Orders");
        }



    }
}