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
    public class ShowTimeController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Admin/ShowTime
        public ActionResult ListShowTime(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;


            return View();
        }

        //Khung Gio
        /// ---------------------------
        ///
        public ActionResult ListKhungGio(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            return View(db.KhungGios.Where(x => x.status == 1).ToList().OrderBy(n => n.ThoiGian).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CreateKhungGio()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateKhungGio(KhungGio khungGio)
        {
            var check = db.KhungGios.Where(x => x.ThoiGian == khungGio.ThoiGian && x.status == 1).Count();
            if (check != 0)
            {
                TempData["Warning"] = "Đã tồn tại thời gian trên!";
                return View();
            }
            if (ModelState.IsValid)
            {
                khungGio.status = 1;
                db.KhungGios.Add(khungGio);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListKhungGio");
            }
            return View(khungGio);
        }

        public ActionResult EditKhungGio(int? id)
        {
            var check = db.SuatChieu_KhungGio.Where(x => x.khung_gio_id == id);
            int dem = 0;
            foreach (var item in check)
            {
                var checksc = db.SuatChieux.Find(item.suat_chieu_id);
                if (checksc.status != 0)
                {
                    dem++;
                    break;
                }
            }
            if (dem > 0)
            {
                TempData["Warning"] = "Giờ này đang tồn tại trong suất chiếu đang có!";
                return RedirectToAction("ListShowTimeFrame");
            }

            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            KhungGio khungGio = db.KhungGios.Find(id);
            if (khungGio == null)
            {
                return HttpNotFound();
            }
            return View(khungGio);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditKhungGio(KhungGio khungGio)
        {
            var check = db.KhungGios.Where(x => x.ThoiGian == khungGio.ThoiGian && x.id != khungGio.id && x.status == 1).Count();
            if (check != 0)
            {
                TempData["Warning"] = "Đã tồn tại thời gian trên!";
                return RedirectToAction("EditKhungGio", "Showtime", new { id = khungGio.id });
            }
            if (ModelState.IsValid)
            {
                db.Entry(khungGio).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("ListKhungGio");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công!";
            }
            return View(khungGio);
        }

        public ActionResult DeleteKhungGio(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            var check = db.SuatChieu_KhungGio.Where(x => x.khung_gio_id == id);
            int dem = 0;
            foreach (var item in check)
            {
                var checksc = db.SuatChieux.Find(item.khung_gio_id);
                if (checksc.status != 0)
                {
                    dem++;
                    break;
                }
            }
            if (dem > 0)
            {
                TempData["Warning"] = "Giờ này đang tồn tại trong suất chiếu đang có!";
                return RedirectToAction("ListShowTimeFrame");
            }
            KhungGio khungGio = db.KhungGios.Find(id);
            khungGio.status = 0;
            TempData["Message"] = "Xóa thành công!";
            db.Entry(khungGio).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListKhungGio");
        }

    }
}