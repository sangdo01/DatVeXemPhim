    using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Controllers
{
    public class MovieController : Controller
    {
        private DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Movie
        public ActionResult Index()
        {
            return View();
        }

        //Chi tiet phim
        public ActionResult MovieDetail(int id)
        {
            var movie = db.Phims.Where(x => x.id == id && x.status == 1).First();
            return View(movie);
        }

        //Phim Đang Chiếu
        public ActionResult NowShowing(int? page, int? category)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            if(category != null)
            {
                var check = db.CT_TheLoai.Where(x => x.theloai_id == category).ToList();
                List<Phim> phims = new List<Phim>();
                foreach(var item in check)
                {
                    phims.Add(db.Phims.Find(item.phim_id));
                }
                ViewBag.category = category;
                //String category1 = category.ToString();
                return View(phims.Where(x => x.status == 1 && x.comingsoon == 1).OrderByDescending(x => x.ngay_cong_chieu).ToPagedList(pageNumber, pageSize));

            }
            else
            {
                return View(db.Phims.Where(x => x.status == 1 && x.comingsoon == 1).OrderByDescending(x => x.ngay_cong_chieu).ToPagedList(pageNumber, pageSize));
            }
            
        }

        //Phim Sắp Chiếu
        public ActionResult ComingSoon(int? page, int? category)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            if (category != null)
            {
                var check = db.CT_TheLoai.Where(x => x.theloai_id == category).ToList();
                List<Phim> phims = new List<Phim>();
                foreach (var item in check)
                {
                    phims.Add(db.Phims.Find(item.phim_id));
                }
                ViewBag.category = category;
                //String category1 = category.ToString();
                return View(phims.Where(x => x.status == 1 && x.comingsoon == 2).OrderByDescending(x => x.ngay_cong_chieu).ToPagedList(pageNumber, pageSize));

            }
            else
            {
                return View(db.Phims.Where(x => x.status == 1 && x.comingsoon == 2).OrderByDescending(x => x.ngay_cong_chieu).ToPagedList(pageNumber, pageSize));
            }

        }

        // Book ngay rap thoi gian
        public ActionResult BookTicket()
        {
            return View();
        }
    }
}