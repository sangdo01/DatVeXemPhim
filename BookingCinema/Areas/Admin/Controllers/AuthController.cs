using BookingCinema.Extensions;
using BookingCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();

        // GET: Admin/Auth


        public ActionResult Login()
        {
            //if(Session["Hoten"] != null)
            //{
            //    return RedirectToAction("Index", "Admin");         
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(String username, String password)
        {
            //if(Session["Hoten"] != null)
            //{
            //    return RedirectToAction("Index", "Admin");
            //}
            if (ModelState.IsValid)
            {
                var f_pass = MyString.GetMD5(password);
                var user = db.Users.FirstOrDefault(m => m.username.Equals(username) && m.password.Equals(f_pass));
                if (user != null)
                {
                    //add session
                    Session["Hoten"] = user.ho_ten;
                    Session["Username"] = user.username;
                    Session["Id"] = user.id;
                    Session["Role"] = user.role;
                    var ten = Convert.ToString(Session["Hoten"]);
                    TempData["Message"] = "Đăng nhập thành công!</br>" + " Xin chào" + ten;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    TempData["Warning"] = "Thông tin đăng nhập sai vui lòng nhập lại !";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["Hoten"] = null;
            Session["Username"] = null;
            Session["Id"] = null;
            Session["Role"] = null;
            return RedirectToAction("Login", "Auth");
        }

    }
}