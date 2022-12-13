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
    public class ShowTimeController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Admin/ShowTime


        //Suat Chieu
        public ActionResult ListShowTime(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            var listsc = db.SuatChieux.Where(x => x.status == 1 || x.status == 2).ToList();
            TimeSpan timecheck = new TimeSpan(0, 10, 0);
            //suất chiếu + giờ chiếu cuối cùng - đi 10 phút < giờ hiện tại thì sc hết hạn
            foreach (var item in listsc)
            {
                var listtime = db.SuatChieu_KhungGio.Where(x => x.khung_gio_id == item.id).OrderByDescending(x => x.KhungGio.ThoiGian).FirstOrDefault();
                if (listtime != null)
                {
                    if (item.ngay_chieu + listtime.KhungGio.ThoiGian - timecheck <= DateTime.Now)
                    {
                        //Chuyển sang hết hạn
                        item.status = 0;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (listtime == null)
                {
                    if (item.ngay_chieu <= DateTime.Now)
                    {
                        item.status = 0;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            ViewBag.cbcongchieu = db.SuatChieux.Where(x => x.status == 2).Count();
            return View(db.SuatChieux.ToList().OrderBy(x => x.ngay_chieu).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CreateSuatChieu()
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
            ViewBag.Timeid = new SelectList(db.KhungGios.Where(x => x.status == 1).ToList().OrderBy(n => n.ThoiGian), "id", "ThoiGian");
            ViewBag.phim_id = new SelectList(db.Phims.ToList().Where(n => n.comingsoon == 1 && n.status == 1).OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.rapchieu = db.RapChieux.Where(x => x.status == 1).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult CreateSuatChieu(SuatChieu suatChieu, string[] timeeframe)
        {
            if (timeeframe == null)
            {
                TempData["Warning"] = "Giờ chiếu không thể trống!";
                return RedirectToAction("CreateSuatChieu");
            }
            if (ModelState.IsValid)
            {
                ViewBag.Timeid = new SelectList(db.KhungGios.Where(n => n.status == 1).ToList().OrderBy(n => n.ThoiGian), "id", "ThoiGian");
                ViewBag.phim_id = new SelectList(db.Phims.Where(n => n.comingsoon == 1 && n.status == 1).ToList().OrderBy(n => n.id), "id", "ten_phim");
                ViewBag.rapchieu = db.RapChieux.Where(x => x.status == 1).ToList();
                SuatChieu_KhungGio sctime = new SuatChieu_KhungGio();
                //set status = 2(sc chuan bi cong chieu)
                suatChieu.status = 2;

                // không cho đặt suất chiếu ngày hiện tại Hoặc đặt suất chiếu trước 10 ngày
                string now = (DateTime.Now).ToString("dd/MM/yyyy");
                TimeSpan plustime1day = new TimeSpan(1, 0, 0, 0);
                DateTime datenow = Convert.ToDateTime(now) + plustime1day;
                //10 ngày
                TimeSpan check10day = new TimeSpan(11, 0, 0, 0);
                DateTime date10day = Convert.ToDateTime(now) + check10day;
                if (suatChieu.ngay_chieu < datenow || suatChieu.ngay_chieu > date10day)
                {
                    TempData["Warning"] = "Ngày chiếu phải hơn ngày hiện tại 1 ngày hoặc không thể hơn quá 10 ngày!";
                    return View();
                }
                var phim = db.Phims.Find(suatChieu.phim_id);
                if (phim.status != 1 || phim.comingsoon != 1)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Phim này chưa công chiếu hoặc đã bị ẩn";
                    return RedirectToAction("CreateSuatChieu");
                }

                //check ngày công chiếu với ngày chiếu
                string ngaycongchieu = Convert.ToDateTime(phim.ngay_cong_chieu).ToString("dd/MM/yyyy");
                if (phim.ngay_cong_chieu > suatChieu.ngay_chieu)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Ngày công chiếu phải từ " + ngaycongchieu + " trở đi";
                    return RedirectToAction("CreateSuatChieu");
                }

                try
                {
                    //check lại nếu có suất chiếu trùng ngày + trùng phòng (cùng rạp) => không cho tạo
                    var rc = db.PhongChieux.Find(suatChieu.phong_chieu_id);
                    var checksc = db.SuatChieux.Where(x => x.phim_id == suatChieu.phim_id && x.ngay_chieu == suatChieu.ngay_chieu
                                                            && x.PhongChieu.RapChieu.id == rc.RapChieu.id && x.status != 0).Count();
                    if (checksc != 0)
                    {
                        TempData["Warning"] = "Đã xảy ra lỗi! Đã tồn 1 suất chiếu tại rạp này. Vui lòng kiểm tra lại!";
                        return View();
                    }
                }
                catch (Exception)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Vui lòng kiểm tra lại!";
                    return View();
                }

                //check khác film cùng rạp thì không cho tạo
                var checkkhac = db.SuatChieux.Where(x => x.phong_chieu_id == suatChieu.phong_chieu_id && x.phim_id != suatChieu.phim_id
                                                       && x.ngay_chieu == suatChieu.ngay_chieu && x.status != 0 && x.id != suatChieu.id).Count();

                if (checkkhac != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Phòng chiếu đã chọn đã có suất chiếu. Vui lòng kiểm tra lại!";
                    return View();
                }

                db.SuatChieux.Add(suatChieu);
                db.SaveChanges();
                if (ModelState.IsValid)
                {
                    foreach (var timeidd in timeeframe)
                    {
                        int timeee = Convert.ToInt32(timeidd);
                        //TimeFrame time = db.TimeFrames.Find(timeee);
                        sctime.khung_gio_id = timeee;
                        sctime.suat_chieu_id = suatChieu.id;
                        db.SuatChieu_KhungGio.Add(sctime);
                        db.SaveChanges();
                    }
                    TempData["Message"] = "Tạo thành công, Suất chiếu đã hiện có tình trạng chuẩn bị chiếu!";
                    return RedirectToAction("ListShowTime");
                }
            }
            else
            {
                TempData["Warning"] = "Không thể để trống!";
                return RedirectToAction("CreateSuatChieu");
            }
            return View(suatChieu);
        }

        public ActionResult EditSuatChieu(int? id)
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
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }
            try
            {
                SuatChieu suatChieu = db.SuatChieux.Find(id);
                if (suatChieu.status != 2)
                {
                    TempData["Warning"] = "Suất này đang chiếu. Chỉ chỉnh sửa được suất chiếu đang chuẩn bị chiếu!";
                    return RedirectToAction("ListShowTime");
                }
                ViewBag.phim_id = new SelectList(db.Phims.Where(n => n.comingsoon == 1 && n.status == 1).ToList().OrderBy(n => n.id), "id", "ten_phim");
                ViewBag.rapchieu = db.RapChieux.Where(x => x.status == 1).ToList();
                if (suatChieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                return View(suatChieu);
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditSuatChieu(SuatChieu suatChieu)
        {
            ViewBag.phim_id = new SelectList(db.Phims.Where(n => n.comingsoon == 1 && n.status == 1).ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.rapchieu = db.RapChieux.Where(x => x.status == 1).ToList();
            if (ModelState.IsValid)
            {
                //// check xem có sửa gì không nếu không thì trả về
                //suat_chieu sccc = db.suat_chieu.Find(suatChieu.id);
                //if (sccc.ngay_chieu == suatChieu.ngay_chieu && sccc.phim_id == suatChieu.phim_id && sccc.phong_chieu_id == suatChieu.phong_chieu_id 
                //    && sccc.id == suatChieu.id)
                //{
                //    TempData["Message"] = "Bạn đã chưa thay đổi gì khi sửa suất chiếu!";
                //    return RedirectToAction("ListShowTime");
                //}

                var phim = db.Phims.Find(suatChieu.phim_id);
                if (phim == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                if (phim.status != 1 || phim.comingsoon != 1)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Phim này chưa công chiếu hoặc đã bị ẩn";
                    return RedirectToAction("ListShowTime");
                }

                var pccc = db.PhongChieux.Find(suatChieu.phong_chieu_id);
                if (pccc == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                if (pccc.status != 1)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! phòng chiếu này chưa hoạt động hoặc không có";
                    return RedirectToAction("ListShowTime");
                }


                string ngaycongchieu = Convert.ToDateTime(phim.ngay_cong_chieu).ToString("dd/MM/yyyy");
                //check ngày công chiếu với ngày chiếu
                if (phim.ngay_cong_chieu > suatChieu.ngay_chieu)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Ngày công chiếu phải từ " + ngaycongchieu + " trở đi";
                    return RedirectToAction("EditShowTime", new { id = suatChieu.id });
                }

                // không cho đặt suất chiếu ngày hiện tại Hoặc đặt suất chiếu trước 10 ngày
                string now = (DateTime.Now).ToString("dd/MM/yyyy");
                //string checkdate = Convert.ToDateTime(suatChieu.ngay_chieu).ToString("dd/MM/yyyy");
                DateTime datee = Convert.ToDateTime(now);
                //TimeSpan plustime1day = new TimeSpan(1, 0, 0, 0);
                //DateTime datenow = Convert.ToDateTime(now) + plustime1day;
                //10 ngày
                TimeSpan check10day = new TimeSpan(11, 0, 0, 0);
                DateTime date10day = Convert.ToDateTime(now) + check10day;
                if (suatChieu.ngay_chieu < datee || suatChieu.ngay_chieu > date10day)
                {
                    TempData["Warning"] = "Ngày chiếu phải hơn ngày hiện tại 1 ngày hoặc không thể hơn quá 10 ngày!";
                    return RedirectToAction("EditSuatChieu", new { id = suatChieu.id });
                }

                //check lại nếu có suất chiếu trùng ngày + trùng phòng (cùng rạp) => không cho sửa lại(không tính chính nó)
                var rc = db.PhongChieux.Find(suatChieu.phong_chieu_id);
                var checksc = db.SuatChieux.Where(x => x.phim_id == suatChieu.phim_id && x.ngay_chieu == suatChieu.ngay_chieu
                                                        && x.PhongChieu.RapChieu.id == rc.RapChieu.id && x.status != 0 && x.id != suatChieu.id).Count();
                if (checksc != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Đã tồn 1 suất chiếu tại rạp này. Vui lòng kiểm tra lại!";
                    return RedirectToAction("EditSuatChieu", new { id = suatChieu.id });
                }


                //check khác film cùng rạp thì không cho tạo
                var checkkhac = db.SuatChieux.Where(x => x.phong_chieu_id == suatChieu.phong_chieu_id && x.phim_id != suatChieu.phim_id
                                                       && x.ngay_chieu == suatChieu.ngay_chieu && x.status != 0 && x.id != suatChieu.id).Count();

                if (checkkhac != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Phòng chiếu đã chọn đã có suất chiếu. Vui lòng kiểm tra lại!";
                    return RedirectToAction("EditShowTime", new { id = suatChieu.id });
                }

                db.Entry(suatChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("ListShowTime");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công!";
            }
            return View(suatChieu);
        }

        //Phòng chiếu của rạp chiếu
        public JsonResult RoomOfCinema(int? id, int? idphong)
        {
            if (id == null)
            {
                return Json(new { succes = false });
            }
            List<int> idroom = new List<int>();
            List<string> roomname = new List<string>();
            if (id == 0)
            {
                var count = idroom.Count();
                return Json(new { count, idroom, roomname }, JsonRequestBehavior.AllowGet);
            }
            if (idphong == null)
            {


                var room = db.PhongChieux.Where(n => n.rap_chieu_id == id && n.status == 1).ToList();
                foreach (var item in room)
                {
                    idroom.Add(item.id);
                    roomname.Add(item.ten_phong_chieu);
                }
                var count = idroom.Count();
                return Json(new { count, idroom, roomname }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var room = db.PhongChieux.Where(n => n.rap_chieu_id == id && n.status == 1).ToList();
                foreach (var item in room)
                {
                    if (item.id != idphong)
                    {
                        idroom.Add(item.id);
                        roomname.Add(item.ten_phong_chieu);
                    }

                }
                var count = idroom.Count();
                return Json(new { count, idroom, roomname }, JsonRequestBehavior.AllowGet);
            }
        }


        // chuyển sang công chiếu cho suất chiếu status 2 => 1
        public ActionResult changetoSTime(int? id)
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
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }

            try
            {
                SuatChieu sc = db.SuatChieux.Find(id);
                if (sc == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }

                if (sc.status != 2)
                {
                    TempData["Error"] = "Suất này đã và đang công chiếu hoặc đã hết hạn!";
                    return RedirectToAction("ListShowTime");
                }

                var listtime = db.SuatChieu_KhungGio.Where(x => x.suat_chieu_id == sc.id).OrderByDescending(x => x.KhungGio.ThoiGian).FirstOrDefault();
                if (listtime == null)
                {
                    TempData["Error"] = "Chưa có giờ chiếu. Không thể công chiếu!";
                    return RedirectToAction("ListShowTime");
                }
                TimeSpan timecheck = new TimeSpan(0, 30, 0);
                //suất chiếu + giờ chiếu cuối cùng - đi 30 phút > giờ hiện tại thì công chiếu
                if (sc.ngay_chieu + listtime.KhungGio.ThoiGian - timecheck > DateTime.Now)
                {
                    //Chuyển sang công chiếu
                    sc.status = 1;
                    db.Entry(sc).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Cập nhật trạng thái thành công!";
                    return RedirectToAction("ListShowTime");
                }
                else
                {
                    TempData["Error"] = "Cập nhật trạng thái không thành công vì quá hạn!";
                    return RedirectToAction("ListShowTime");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Cập nhật trạng thái không thành công!";
                return RedirectToAction("ListShowTime");
            }
        }

        //Back suất chiếu về  trạng thái chưa công chiếu( status = 2) nếu chưa có ai đặt
        public ActionResult BackToReady(int? id)
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
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }

            try
            {
                SuatChieu suatChieu = db.SuatChieux.Find(id);
                if (suatChieu.status != 1)
                {
                    TempData["Error"] = "Đã xảy ra lỗi!";
                    return RedirectToAction("ListShowTime");
                }

                var check = db.Orders.Where(x => x.suatchieu_id == suatChieu.id).Count();
                if (check != 0)
                {
                    TempData["Error"] = "Không thể sửa trạng thái: chuẩn bị công chiếu, vì đã có người đặt vé ở suất này!";
                    return RedirectToAction("ListShowTime");
                }
                //Xóa giờ chiếu
                suatChieu.status = 2;
                db.Entry(suatChieu).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Message"] = "Đã chỉnh sửa trạng thái: chuẩn bị công chiếu!";
                return RedirectToAction("ListShowTime");

            }
            catch (Exception)
            {
                TempData["Error"] = "Đã xảy ra lỗi!";
                return RedirectToAction("ListShowTime");
            }
        }

        //Xóa khi chưa công chiếu.

        public ActionResult DeleteSuatChieu(int? id)
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
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }

            try
            {
                SuatChieu suatChieu = db.SuatChieux.Find(id);
                if (suatChieu.status != 2)
                {
                    TempData["Error"] = "Đã xảy ra lỗi!";
                    return RedirectToAction("ListShowTime");
                }

                //Xóa giờ chiếu
                var timesc = db.SuatChieu_KhungGio.Where(x => x.suat_chieu_id == id).ToList();
                foreach (var item in timesc)
                {
                    SuatChieu_KhungGio sctime = db.SuatChieu_KhungGio.Find(item.id);
                    db.SuatChieu_KhungGio.Remove(sctime);
                    db.SaveChanges();
                }

                db.SuatChieux.Remove(suatChieu);
                db.SaveChanges();

                TempData["Message"] = "Xóa suất chiếu thành công!";
                return RedirectToAction("ListShowTime");

            }
            catch (Exception)
            {
                TempData["Error"] = "Đã xảy ra lỗi!";
                return RedirectToAction("ListShowTime");
            }
        }


        //Khung Gio
        /// ---------------------------
        ///
        public ActionResult ListKhungGio(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            return View(db.KhungGios.Where(x => x.status == 1).ToList().OrderBy(n => n.ThoiGian).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CreateKhungGio()
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
            return View();
        }

        [HttpPost]
        public ActionResult CreateKhungGio(KhungGio khungGio)
        {
            var check = db.KhungGios.Where(x => x.ThoiGian == khungGio.ThoiGian && x.status == 1).Count();
            if (check != 0)
            {
                TempData["Warning"] = "Đã tồn tại thời gian trên!";
                return View();
            }
            if (ModelState.IsValid)
            {
                khungGio.status = 1;
                db.KhungGios.Add(khungGio);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListKhungGio");
            }
            return View(khungGio);
        }

        public ActionResult EditKhungGio(int? id)
        {
            var check = db.SuatChieu_KhungGio.Where(x => x.khung_gio_id == id);
            int dem = 0;
            foreach (var item in check)
            {
                var checksc = db.SuatChieux.Find(item.suat_chieu_id);
                if (checksc.status != 0)
                {
                    dem++;
                    break;
                }
            }
            if (dem > 0)
            {
                TempData["Warning"] = "Giờ này đang tồn tại trong suất chiếu đang có!";
                return RedirectToAction("ListShowTimeFrame");
            }

            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            KhungGio khungGio = db.KhungGios.Find(id);
            if (khungGio == null)
            {
                return HttpNotFound();
            }
            return View(khungGio);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditKhungGio(KhungGio khungGio)
        {
            var check = db.KhungGios.Where(x => x.ThoiGian == khungGio.ThoiGian && x.id != khungGio.id && x.status == 1).Count();
            if (check != 0)
            {
                TempData["Warning"] = "Đã tồn tại thời gian trên!";
                return RedirectToAction("EditKhungGio", "Showtime", new { id = khungGio.id });
            }
            if (ModelState.IsValid)
            {
                db.Entry(khungGio).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("ListKhungGio");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công!";
            }
            return View(khungGio);
        }

        public ActionResult DeleteKhungGio(int? id)
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
            var check = db.SuatChieu_KhungGio.Where(x => x.khung_gio_id == id);
            int dem = 0;
            foreach (var item in check)
            {
                var checksc = db.SuatChieux.Find(item.khung_gio_id);
                if (checksc.status != 0)
                {
                    dem++;
                    break;
                }
            }
            if (dem > 0)
            {
                TempData["Warning"] = "Giờ này đang tồn tại trong suất chiếu đang có!";
                return RedirectToAction("ListShowTimeFrame");
            }
            KhungGio khungGio = db.KhungGios.Find(id);
            khungGio.status = 0;
            TempData["Message"] = "Xóa thành công!";
            db.Entry(khungGio).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListKhungGio");
        }

    }
}