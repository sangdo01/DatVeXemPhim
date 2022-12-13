using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Areas.Admin.Controllers
{
    public class FeedBackController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Admin/FeedBack
        public ActionResult ListFeedback(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            //ViewBag.count = db.LienHes.Count();
            //ViewBag.count1 = db.LienHes.Where(n => n.status == 1).Count();
            //ViewBag.count2 = db.LienHes.Where(n => n.status == 2).Count();
            //ViewBag.count0 = db.LienHes.Where(n => n.status == 0).Count();
            return View(db.LienHes.OrderByDescending(model => model.create_at).ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DeleteConfirm(int id)
        {
            LienHe fb = db.LienHes.Find(id);
            db.LienHes.Remove(fb);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListFeedback");
        }


    }
}