using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Controllers
{
    public class NewsController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: News
        public ActionResult Index(String title, int? page)
        {
            ViewBag.titleDisplay = title;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(db.TinTucs.OrderByDescending(x => x.thoi_gian_dang).ToPagedList(pageNumber, pageSize));
        }
        //trang
        public ActionResult NewsDetail(int id)
        {
            if (id != 0)
            {
                return View(db.TinTucs.SingleOrDefault(t => t.id == id));
            }
            else
                return RedirectToAction("Error", "Home");
            
        }

    }
}