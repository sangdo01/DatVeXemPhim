using BookingCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Controllers
{
    public class InformationController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Information
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(LienHe lienHe)
        {
            if (ModelState.IsValid)
            {
                lienHe.create_at = DateTime.Now;
                lienHe.status = 1;
                db.LienHes.Add(lienHe);
                db.SaveChanges();
                TempData["Message"] = "Bạn đã gửi liên hệ thành công!";
                //return View();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult AboutUs()
        {
            ViewBag.cinema = db.RapChieux.Where(x => x.status == 1).ToList();
            return View();
        }
    }
}