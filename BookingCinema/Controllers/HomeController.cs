using BookingCinema.Extensions;
using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Controllers
{
    
    public class HomeController : Controller
    {
        //private const string Success = "Bạn đã đăng nhập thành công";
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Home
        public ActionResult Index()
        {
            //thông báo đăng nhập thành công
            //if(Request["statusLogin"] == null)
            //{
            //    return View();
            //}
            //else
            //{
            //    Status status = (Status)Enum.Parse(typeof(Status), Request["statusLogin"]);
            //    if (status == Status.Success)
            //    {
            //        this.AddNotification("Bạn đã đăng nhập thành công!!!", NotificationType.SUCCESS);                    
            //    }
            //}

            ViewBag.tt = db.TinTucs.FirstOrDefault();
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        //menu thanh tieu de( search theo the loai )
        public ActionResult MenuPartial()
        {
            return PartialView();
        }


    }
}