using BookingCinema.Extensions;
using BookingCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Controllers
{
    public class UserController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(KhachHang user)
        {
            if(ModelState.IsValid)
            {
                var check = db.KhachHangs.FirstOrDefault(x => x.username == user.username || x.email == user.email);
                var checkuser = db.KhachHangs.FirstOrDefault(x => x.username == user.username);
                if (check == null && checkuser == null)
                {
                    user.password = MyString.GetMD5(user.password);                    
                    db.KhachHangs.Add(user);
                    db.SaveChanges();
                    TempData["Message"] = "Bạn đã tạo tài khoản thành công";
                    return RedirectToAction("Login");
                }
                else
                {
                    if(check != null)
                    {
                        ViewBag.MessageEmail = "Email this already exists!";
                        //this.AddNotification("Email này đã được đăng ký, vui lòng sử dụng email khác!!!", NotificationType.WARNING);
                        TempData["Warning"] = "Email này đã được đăng ký, vui lòng sử dụng email khác!";
                    }
                    else if(checkuser != null){
                        ViewBag.MessageUser = "Tài khoản this already exists!";
                        //this.AddNotification("Tài khoản này đã tồn tại, vui lòng chọn tài khoản khác!!!", NotificationType.WARNING);
                        TempData["Warning"] = "Tài khoản này đã tồn tại, vui lòng chọn tài khoản khác!";
                    }
                    return View();
                }                
            }
            return View();
        }


        public ActionResult Login()
        {
            //if (Request["statusRegister"] == null)
            //{
            //    return View();
            //}
            //else
            //{
            //    Status statusRgister = (Status)Enum.Parse(typeof(Status), Request["statusRegister"]);
            //    if (statusRgister == Status.Success)
            //    {
            //        this.AddNotification("Đăng ký thành công!!!", NotificationType.SUCCESS);
            //    }
            //}
            return View();
        }
        [HttpPost]
        public ActionResult Login( string email, string matkhau)
        {
            if(ModelState.IsValid)
            {
                var f_password = MyString.GetMD5(matkhau);
                var client = db.KhachHangs.FirstOrDefault(x => x.email.Equals(email) && x.password.Equals(f_password));
                if(client!= null)
                {
                    //add session
                    Session["TaiKhoan"] = client;
                    Session["TenKH"] = client.ho_ten;
                    Session["EmailKH"] = client.email;
                    Session["MaKH"] = client.id;

                    TempData["Message"] = "Bạn đã đăng nhập thành công";
                    //return RedirectToAction("Index", "Home", new { statusLogin = Status.Success });
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Warning"] = "Bạn đã đăng nhập thất bại";
                    //return RedirectToAction("Login", "User", new { statusRegister = Status.Error});
                    return RedirectToAction("Login");
                }
            }    
            return View();
        }

        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            Session["TenKH"] = null;
            Session["EmailKH"] = null;
            Session["MaKH"] = null;
            //add them thong bao khi dang xuat thanh cong
            TempData["Message"] = "Bạn đã đăng xuất thành công";
            return RedirectToAction("Index", "Home");
        }

    }
}