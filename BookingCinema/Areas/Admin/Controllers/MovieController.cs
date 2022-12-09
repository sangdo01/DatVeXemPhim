using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Areas.Admin.Controllers
{
    public class MovieController : Controller
    {
        private DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();

        // GET: Admin/Movie
        public ActionResult ListMovie(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            if (Session["Hoten"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt16(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.trash = db.Phims.Where(x => x.status == 0).Count();
            return View(db.Phims.OrderByDescending(m => m.id).ToList().ToPagedList(pageNumber, pageSize));
        }

        //them moi phim
        public ActionResult CreateMovie()
        {
            if (Session["Hoten"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt16(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.the_loai_phim_id = new SelectList(db.TheLoaiPhims.ToList().OrderBy(n => n.id), "id", "ten_the_loai");
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMovie(Phim phim, CT_TheLoai listTheLoai, String[] theloaiarray)
        {
            ViewBag.the_loai_phim_id = new SelectList(db.TheLoaiPhims.ToList().OrderBy(model => model.id), "id", "ten_the_loai");
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                string except = " ";
                string strResult = Regex.Replace(phim.ten_phim, @"[^a-zA-Z0-9" + except + "]+", string.Empty);
                string strSlug = MyString.ToAscii(strResult) + numrd + phim.id;
                phim.status = 2;
                phim.comingsoon = 2;
                //Upload File
                var fileAnh = Request.Files["anh"];
                if (fileAnh != null && fileAnh.ContentLength > 0)
                {

                    string filename = strSlug + fileAnh.FileName.Substring(fileAnh.FileName.LastIndexOf("."));
                    phim.anh = filename;
                    string path = Server.MapPath("~/images/movies/");
                    string StrPath = Path.Combine(path, filename);
                    fileAnh.SaveAs(StrPath);
                }
                var fileBanner = Request.Files["Banner"];
                if (fileBanner != null && fileBanner.ContentLength > 0)
                {
                    String filename = strSlug + fileBanner.FileName.Substring(fileBanner.FileName.LastIndexOf("."));
                    phim.Banner = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/banner/"));
                    fileBanner.SaveAs(Path.Combine(StrPath, filename));
                }
                //không cần thêm thể loại chính : nếu thêm thì nên dùng string cho thêm dấu "-" ở giữa mỗi thể loại
                //tránh trường hợp id thể loại lưu vào có {1,3} mà có id thể loại là 13 thì nó suất theo luôn id 13 nữa. 
                db.Phims.Add(phim);
                db.SaveChanges();
                TempData["Message"] = "Bạn đã thêm phim mới thành công!";

                foreach (string theloaiid in theloaiarray)
                {
                    TheLoaiPhim selecttheloai = db.TheLoaiPhims.ToList().Find(p => p.id.ToString() == theloaiid);
                    listTheLoai.phim_id = phim.id;
                    listTheLoai.theloai_id = selecttheloai.id;
                    db.CT_TheLoai.Add(listTheLoai);
                    db.SaveChanges();
                }
                return RedirectToAction("ListMovie");
            }
            return View(phim);
        }

        //Edit phim
        public ActionResult EditMovie(int? id)
        {
            if (Session["Hoten"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt16(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.the_loai_phim_id = new SelectList(db.TheLoaiPhims.ToList().OrderBy(n => n.id), "id", "ten_the_loai");
            Phim phim = db.Phims.Find(id);
            if (phim == null)
            {
                return RedirectToAction("AERROR404", "Admin");
            }

            var checkstime = db.SuatChieux.Where(x => x.phim_id == id && x.status != 0).Count();
            if (checkstime != 0)
            {
                TempData["Warning"] = "Hiện tại phim này còn suất chiếu! không thể sửa";
                return RedirectToAction("ListMovie");
            }
            return View(phim);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditMovie(Phim phim, CT_TheLoai listTheLoai, String[] theloaiarray)
        {
            ViewBag.the_loai_phim_id = new SelectList(db.TheLoaiPhims.ToList().OrderBy(model => model.id), "id", "ten_the_loai");
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                string except = " ";
                string strResult = Regex.Replace(phim.ten_phim, @"[^a-zA-Z0-9" + except + "]+", string.Empty);
                string strSlug = MyString.ToAscii(strResult) + numrd + phim.id;
                //Upload File
                var fileAnh = Request.Files["anh"];
                if (fileAnh != null && fileAnh.ContentLength > 0)
                {

                    string filename = strSlug + fileAnh.FileName.Substring(fileAnh.FileName.LastIndexOf("."));
                    phim.anh = filename;
                    string path = Server.MapPath("~/images/movies/");
                    string StrPath = Path.Combine(path, filename);
                    fileAnh.SaveAs(StrPath);
                }
                var fileBanner = Request.Files["Banner"];
                if (fileBanner != null && fileBanner.ContentLength > 0)
                {
                    String filename = strSlug + fileBanner.FileName.Substring(fileBanner.FileName.LastIndexOf("."));
                    phim.Banner = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/banner/"));
                    fileBanner.SaveAs(Path.Combine(StrPath, filename));
                }
                //không cần thêm thể loại chính : nếu thêm thì nên dùng string cho thêm dấu "-" ở giữa mỗi thể loại
                //tránh trường hợp id thể loại lưu vào có {1,3} mà có id thể loại là 13 thì nó suất theo luôn id 13 nữa. 
                db.Entry(phim).State = EntityState.Modified;
                TempData["Message"] = "Bạn đã cập nhật thành công!";
                db.SaveChanges();
                //Edit the loai
                if (theloaiarray != null)
                {
                    foreach (var item in db.CT_TheLoai.Where(x => x.phim_id == phim.id).ToList())
                    {
                        db.CT_TheLoai.Remove(item);
                        db.SaveChanges();
                    }
                    foreach (string theloaiid in theloaiarray)
                    {
                        TheLoaiPhim selecttheloai = db.TheLoaiPhims.ToList().Find(p => p.id.ToString() == theloaiid);
                        listTheLoai.phim_id = phim.id;
                        listTheLoai.theloai_id = selecttheloai.id;
                        db.CT_TheLoai.Add(listTheLoai);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("ListMovie");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công !";
            }
            return View(phim);
        }

        //Thay đổi trạng thái xuất hiện hay ẩn của phim 
        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            Phim phim = db.Phims.Find(id);
            var checkstime = db.SuatChieux.Where(x => x.phim_id == id && x.status != 0).Count();
            if (checkstime != 0)
            {
                return Json(new { success = false });
            }
            //status = 1 (hiện phim)
            //status = 2 (ẩn phim);
            phim.status = (phim.status == 1) ? 2 : 1;
            //if (mProduct.Status==1)
            //{
            //    mProduct.Status = 2;
            //}
            //else
            //{
            //    mProduct.Status = 1;
            //}
            db.Entry(phim).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = phim.status });
        }

        //Thay đổi phim đang chiếu hay sắp chiếu
        [HttpPost]
        public JsonResult ChangeRelease(int id)
        {
            Phim phim = db.Phims.Find(id);
            var checkstime = db.SuatChieux.Where(x => x.phim_id == id && x.status != 0).Count();
            if (checkstime != 0)
            {
                return Json(new { success = false });
            }
            phim.comingsoon = (phim.comingsoon == 1) ? 2 : 1;
            db.Entry(phim).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = phim.comingsoon });
        }

        //xoa phim
        public ActionResult DeleteMovie(int id)
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
            Phim phim = db.Phims.Find(id);
            if (phim == null)
            {
                return RedirectToAction("AERROR404", "Admin");
            }

            var checkstime = db.SuatChieux.Where(x => x.phim_id == id && x.status != 0).Count();
            if (checkstime != 0)
            {
                TempData["Warning"] = "Đã xảy ra lỗi không thể xóa !";
                return RedirectToAction("ListMovie");
            }
            var listtheloai = db.CT_TheLoai.Where(x => x.phim_id == id).ToList();
            foreach (CT_TheLoai item in listtheloai)
            {
                db.CT_TheLoai.Remove(item);
                db.SaveChanges();
            }
            // cho nó khỏi hiện thị lên. Xóa là lỗi
            phim.status = 10;
            db.Entry(phim).State = EntityState.Modified;
            TempData["Message"] = "Xóa phim thành công!";
            db.SaveChanges();
            return RedirectToAction("ListMovie");
        }
        public ActionResult ListCategory(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            if (Session["Hoten"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt16(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            return View(db.TheLoaiPhims.OrderByDescending(model => model.id).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CreateCategoryMovie()
        {
            if (Session["Hoten"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt16(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateCategoryMovie(TheLoaiPhim theLoai)
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
            //String strSlug = MyString.ToAscii(theLoai.ten_the_loai); // Tạo slug cho tên thể loại phim
            //theLoai.slug = strSlug;
            db.TheLoaiPhims.Add(theLoai);
            TempData["Message"] = "Thêm mới thể loại thành công!";
            db.SaveChanges();
            return RedirectToAction("ListCategory");
        }

        //sửa thể loại phim
        public ActionResult EditCategory(int? id)
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
            TheLoaiPhim theLoai = db.TheLoaiPhims.Find(id);
            if(theLoai == null)
            {
                return RedirectToAction("AERROR404", "Admin");
            }
            return View(theLoai);
        }
        [HttpPost]
        public ActionResult EditCategory(TheLoaiPhim theLoai)
        {
            if(ModelState.IsValid)
            {
                db.Entry(theLoai).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListCategory");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(theLoai);
        }

        public ActionResult DeleteCate(int id)
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
            //Tìm kiếm id thể loại phim có tồn tại trong phim nào không
            var del = from dele in db.CT_TheLoai
                      where dele.theloai_id == id
                      select dele;
            var coundel = del.Count(); //Đếm số lượng id thể loại phim có trong phim

            if (coundel == 0) //Nếu không thì tiến hành xóa thể loại này
            {
                TheLoaiPhim theLoai = db.TheLoaiPhims.Find(id);
                db.TheLoaiPhims.Remove(theLoai);
                TempData["Message"] = "Xóa thể loại thành công!";
                db.SaveChanges();
            }
            else
            {
                TempData["Warning"] = "Không thể xóa vì đang có phim tồn tại trong mục này!";
            }
            return RedirectToAction("ListCategory");
        }
    }
}