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
    public class UserController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Admin/User

        //list User
        public ActionResult ListUser(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            return View(db.Users.OrderByDescending(model => model.id).ToList().ToPagedList(pageNumber, pageSize));
        }

        //Tạo nhan vien
        public ActionResult CreateUser()
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
            List<SelectListItem> roleUser = new List<SelectListItem>() {
            new SelectListItem {
                Text = "Admin", Value = "1"
            },
            new SelectListItem {
                Text = "Nhân Viên", Value = "2"
            },
            };
            ViewBag.role = roleUser;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(User uSer)
        {
            List<SelectListItem> roleUser = new List<SelectListItem>() {
            new SelectListItem {
                Text = "Admin", Value = "1"
            },
            new SelectListItem {
                Text = "Nhân Viên", Value = "2"
            },
            };
            ViewBag.role = roleUser;
            if (ModelState.IsValid)
            {
                var check = db.Users.FirstOrDefault(s => s.email == uSer.email || s.username == uSer.username);
                if (check == null)
                {

                    uSer.password = MyString.GetMD5(uSer.password);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Users.Add(uSer);
                    TempData["Message"] = "Bạn đã tạo tài khoản thành công!";
                    db.SaveChanges();
                    return RedirectToAction("ListUser");
                }
                else
                {
                    TempData["Error"] = "Email này đã đăng ký bằng tài khoản khác, vui lòng nhập email khác!";
                    ViewBag.error = "Email này đã đăng ký bằng tài khoản khác, vui lòng nhập email khác!";
                    return View();
                }
            }
            return View(uSer);
        }

        public ActionResult EditUser(int? id)
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
            List<SelectListItem> roleUser = new List<SelectListItem>() {
            new SelectListItem {
                Value = "1", Text = "Admin"
            },
            new SelectListItem {
                Value = "2", Text = "Nhân Viên"
            },
            };
            ViewBag.role = roleUser;
            User uSer = db.Users.Find(id);
            if (User == null)
            {
                return RedirectToAction("ListUser", "User");
            }
            return View(uSer);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User uSer)
        {
            List<SelectListItem> roleUser = new List<SelectListItem>() {
            new SelectListItem {
                 Value = "1", Text = "Admin"
            },
            new SelectListItem {
                Value = "2", Text = "Nhân Viên"
            },
            };
            ViewBag.role = roleUser;
            if (ModelState.IsValid)
            {
                db.Entry(uSer).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("ListUser");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(uSer);
        }

        public ActionResult DeleteConfirmed(int id)
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
            //if (Session["HoTen"] == null)
            //{
            //    return RedirectToAction("Login", "Auth");
            //}
            User uSer = db.Users.Find(id);
            db.Users.Remove(uSer);
            db.SaveChanges();
            TempData["Message"] = "Xóa thành công!";
            return RedirectToAction("ListUser");
        }


        //list khach hang
        public ActionResult ListKH(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View(db.KhachHangs.OrderByDescending(m => m.id).ToList().ToPagedList(pageNumber, pageSize));
        }




    }
}