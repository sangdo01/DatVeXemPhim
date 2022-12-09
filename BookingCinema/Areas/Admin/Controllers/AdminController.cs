using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        public ActionResult Index()
        {
            if(Session["Hoten"] != null)
            {
                return View();                
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }            
        }


        //Error
        public ActionResult AERROR404()
        {
            return View();
        }
    }
}