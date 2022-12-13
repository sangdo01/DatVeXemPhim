using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Admin/News
        public ActionResult ListNews(int? page)
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
            return View(db.TinTucs.OrderByDescending(s => s.thoi_gian_dang).ToPagedList(pageNumber, pageSize));
        }

        //Tạo bài viết  
        public ActionResult CreateNews()
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
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNews(TinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(tinTuc.tieu_de) + numrd + tinTuc.id;
                tinTuc.thoi_gian_dang = DateTime.Now;
                tinTuc.status = 1;
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    tinTuc.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/news/"), filename);
                    file.SaveAs(StrPath);
                }
                db.TinTucs.Add(tinTuc);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListNews");
            }
            return View(tinTuc);
        }
        //Admin/News/Edit
        public ActionResult EditNews(int? id)
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
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return RedirectToAction("ListNews", "News");
            }
            return View(tinTuc);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews(TinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(tinTuc.tieu_de) + numrd + tinTuc.id;
                tinTuc.thoi_gian_dang = DateTime.Now;
                //Upload File
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    tinTuc.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/news/"));
                    file.SaveAs(Path.Combine(StrPath, filename));
                }
                db.Entry(tinTuc).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListNews");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(tinTuc);
        }

        public ActionResult DeleteNews(int id)
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
            TinTuc tinTuc = db.TinTucs.Find(id);
            db.TinTucs.Remove(tinTuc);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListNews");
        }
    }
}