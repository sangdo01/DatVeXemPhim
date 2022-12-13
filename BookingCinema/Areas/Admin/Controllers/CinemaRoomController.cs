using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Areas.Admin.Controllers
{
    public class CinemaRoomController : Controller
    {
        DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: Admin/CinemaRoom

        //Danh sach rap chieu
        public ActionResult ListCinema(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt16(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }

            return View(db.RapChieux.Where(x => x.status == 1).OrderByDescending(model => model.id).ToPagedList(pageNumber, pageSize));
        }

        //Tao rap chieu phim
        public ActionResult CreateCinema()
        {
            if (Session["HoTen"] == null)
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
        public ActionResult CreateCinema(RapChieu rapChieu)
        {
            if (ModelState.IsValid)
            {
                rapChieu.status = 1;
                db.RapChieux.Add(rapChieu);
                TempData["Message"] = "Tạo rạp chiếu thành công!";
                db.SaveChanges();
                return RedirectToAction("ListCinema");
            }
            return View(rapChieu);
        }

        //Sua rap chieu
        public ActionResult EditCinema(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt16(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AERROR404", "Admin");
            }
            try
            {
                RapChieu rapChieu = db.RapChieux.Find(id);
                if (rapChieu == null)
                {
                    return RedirectToAction("AERROR404", "Admin");
                }
                return View(rapChieu);
            }
            catch (Exception)
            {
                return RedirectToAction("AERROR404", "Admin");
            }
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditCinema(RapChieu rapChieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rapChieu).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListCinema");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công";
            }
            return View(rapChieu);
        }

        //xoa rap chieu
        public ActionResult DeleteCinema(int? id)
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

            if (db.PhongChieux.Where(x => x.status != 0 && x.rap_chieu_id == id).Count() != 0)
            {
                TempData["Error"] = "Không thể xóa vì đang có phòng chiếu tại nơi này!";
                return RedirectToAction("ListCinema");
            }
            else
            {
                RapChieu rapChieu = db.RapChieux.Find(id);
                rapChieu.status = 0;
                db.Entry(rapChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Xóa thành công!";
                return RedirectToAction("ListCinema");
                //RapChieu rapChieu = db.RapChieux.Find(id);
                //db.RapChieux.Remove(rapChieu);
                //db.SaveChanges();
                //TempData["Message"] = "Xóa thành công!";
                //return RedirectToAction("ListCinema");
            }
        }


        //Danh sach phong chieu
        public ActionResult ListCinemaRoom(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Index", "Admin");
            }
            return View(db.PhongChieux.OrderByDescending(model => model.id).ToPagedList(pageNumber, pageSize));
        }

        //them phong chieu moi
        public ActionResult CreateCinemaRoom()
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

            //ViewBag.id_rapchieu = new SelectList(db.RapChieux.Where(x => x.status == 1).ToList().OrderBy(n => n.id), "id", "ten_rap");
            ViewBag.rap_chieu_id = new SelectList(db.RapChieux.Where(x => x.status == 1).ToList().OrderBy(n => n.id), "id", "ten_rap");
            return View();
        }
        [HttpPost]
        public ActionResult CreateCinemaRoom(PhongChieu phongChieu)
        {
            if (ModelState.IsValid)
            {
                ViewBag.rap_chieu_id = new SelectList(db.RapChieux.Where(x => x.status == 1).ToList().OrderBy(n => n.id), "id", "ten_rap");
                //Lấy ra phụ thu của loại ghế nhỏ nhất để tạo phòng có giá default
                var loaighe = db.LoaiGhes.OrderBy(x => x.phu_thu).FirstOrDefault();
                string[] room = new string[5] { "A", "B", "C", "D", "E" };
                GheNgoi ghe = new GheNgoi();
                phongChieu.so_ghe = 50;
                phongChieu.status = 1;
                var checkopc = db.PhongChieux.Where(x => x.ten_phong_chieu == phongChieu.ten_phong_chieu && x.rap_chieu_id == phongChieu.rap_chieu_id).Count();
                if (checkopc != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Đã tồn 1 phòng chiếu giống phòng chiếu này. Vui lòng kiểm tra lại!";
                    return View();
                }
                db.PhongChieux.Add(phongChieu);
                db.SaveChanges();
                var id = phongChieu.id;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        ghe.hang = room[i];
                        ghe.cot = j + 1;
                        ghe.phong_chieu_id = id;
                        ghe.gia_ghe = 60000;
                        ghe.loai_ghe_id = loaighe.id;
                        ghe.status = 1;
                        db.GheNgois.Add(ghe);
                        db.SaveChanges();

                    }
                }
                TempData["Message"] = "Thêm thành công!";
            }
            else
            {
                TempData["Warning"] = "Thêm không thành công!";
            }
            return RedirectToAction("ListCinemaRoom");
        }

        // Sua phong chieu
        public ActionResult EditCinemaRoom(int? id)
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
                ViewBag.id_rapc = new SelectList(db.RapChieux.Where(x => x.status == 1).ToList().OrderBy(n => n.id), "id", "ten_rap");
                PhongChieu phongChieu = db.PhongChieux.Find(id);
                // status = 0 là đã xóa
                if (phongChieu == null || phongChieu.status == 0)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                var check = db.SuatChieux.Where(x => x.phong_chieu_id == id && x.status == 1);
                TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
                int dem = 0;
                foreach (var item in check)
                {
                    if (item.ngay_chieu + timecheck > DateTime.Now)
                    {
                        dem++;
                    }

                }

                if (dem > 0)
                {
                    TempData["Warning"] = "Hiện tại không thể chỉnh sửa vì phòng này tồn tại suất chiếu hoặc chưa hết ngày chiếu!";
                    return RedirectToAction("ListCinemaRoom");
                }
                return View(phongChieu);
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }

        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditCinemaRoom(PhongChieu phongChieu)
        {
            ViewBag.id_rapc = new SelectList(db.RapChieux.Where(x => x.status == 1).ToList().OrderBy(n => n.id), "id", "ten_rap");
            if (ModelState.IsValid)
            {
                var checkTenPC = db.PhongChieux.Where(model => model.id == phongChieu.id && model.rap_chieu_id == phongChieu.rap_chieu_id).Count();
                if (checkTenPC > 0)
                {
                    TempData["Warning"] = "Đã tồn tại phòng chiếu này trong rạp chiếu! Vui lòng kiểm tra lại ! ";
                }
                db.Entry(phongChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Đã cập nhật thành công";
                return RedirectToAction("ListCinemaRoom");
                
            }
            else
            {
                TempData["Warning"] = "Cập nhật không thành công ! ";
            }
            return View(phongChieu);
        }


        //Xoa phong chieu
        public ActionResult DeleteCinemaRoom(int? id)
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
                PhongChieu phongChieu = db.PhongChieux.Find(id);
                if (phongChieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }

                if (phongChieu.status != 2)
                {
                    TempData["Warning"] = "Phòng chiếu có tình trạng dừng hoạt động mới được xóa !";
                    return RedirectToAction("ListCinemaRoom");

                }
                // bằng 2 thì đã dừng hoạt động
                var check = db.SuatChieux.Where(x => x.phong_chieu_id == id && x.status == 1);
                TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
                int dem = 0;
                foreach (var item in check)
                {
                    if (item.ngay_chieu + timecheck > DateTime.Now)
                    {
                        dem++;
                    }

                }

                if (dem > 0)
                {
                    TempData["Warning"] = "Hiện tại không thể xóa vì phòng này tồn tại suất chiếu hoặc chưa hết ngày chiếu!";
                    return RedirectToAction("ListCinemaRoom");
                }

                //Xóa = ẩn nó đi tại vì nó ràng buộc nhiều, không thể xóa
                phongChieu.status = 0;
                db.Entry(phongChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Xóa thành công!";
                return RedirectToAction("ListCinemaRoom");
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }



        //sửa status room
        public ActionResult changeStatusRoom(int? id)
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

                PhongChieu phongChieu = db.PhongChieux.Find(id);
                var check = db.SuatChieux.Where(x => x.phong_chieu_id == id && x.status == 1);
                if (phongChieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
                int dem = 0;
                foreach (var item in check)
                {
                    if (item.ngay_chieu + timecheck > DateTime.Now)
                    {
                        dem++;
                    }

                }

                if (dem > 0)
                {
                    TempData["Warning"] = "Hiện tại không thể dừng hoạt động vì phòng này tồn tại suất chiếu hoặc chưa hết ngày chiếu!";
                    return RedirectToAction("ListCinemaRoom");
                }

                //Nếu không phải = 1 là sai
                if (phongChieu.status != 1)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                //status = 2 là dừng hoạt động
                phongChieu.status = 2;
                db.Entry(phongChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Dừng hoạt động thành công";
                return RedirectToAction("ListCinemaRoom");
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }

        //Undo phòng không hoạt động
        public ActionResult UndoStatusRoom(int? id)
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

                PhongChieu phongChieu = db.PhongChieux.Find(id);
                if (phongChieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }

                //Nếu không phải = 2 là sai
                if (phongChieu.status != 2)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                //status = 1 là hoạt động
                phongChieu.status = 1;
                db.Entry(phongChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Đã bật trạng thái hoạt động";
                return RedirectToAction("ListCinemaRoom");
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }


        public ActionResult SeatRoom(int? id)
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
                PhongChieu phongChieu = db.PhongChieux.Find(id);
                if (phongChieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                if (phongChieu.status == 0)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                ViewBag.loai_ghe_id = db.LoaiGhes.ToList().OrderBy(n => n.id);
                ViewBag.pc = phongChieu;
                var ghengoi = db.GheNgois.Where(x => x.phong_chieu_id == id & x.status == 1).OrderBy(x => x.hang);
                ViewBag.ghe = ghengoi;
                return View(ghengoi);
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }

        }


        [HttpPost]
        public ActionResult CreateSeat(string hang, int ghe, int id, int idloaighe)
        {
            if (ghe > 20)
            {
                return Json(new { checkslg = false });
            }
            if (hang == null || ghe < 1 || id < 1)
            {
                return Json(new { checkr = false });
            }

            var check = db.SuatChieux.Where(x => x.phong_chieu_id == id && x.status == 1);
            TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
            int dem = 0;
            foreach (var item in check)
            {
                if (item.ngay_chieu + timecheck > DateTime.Now)
                {
                    dem++;
                }

            }
            //không cho thêm thêm nếu có suất chiếu
            if (dem > 0)
            {
                return Json(new { checkr = false });
            }
            //check xem phòng đã xóa chưa
            PhongChieu phongChieu = db.PhongChieux.Find(id);
            if (phongChieu.status == 0)
            {
                return Json(new { checkr = false });
            }
            var ghn = db.GheNgois.Where(n => n.hang == hang && n.phong_chieu_id == id && n.status == 1).Count();
            GheNgoi ghengoi = new GheNgoi();
            if (ghn != 0)
            {
                return Json(new { success = false });
            }
            else
            {
                var checkghe = db.GheNgois.Where(x => x.phong_chieu_id == id && x.status == 1).ToList();
                var pc = db.PhongChieux.Find(id);
                pc.so_ghe = checkghe.Count() + ghe;
                db.Entry(pc).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    for (int i = 0; i < ghe; i++)
                    {
                        ghengoi.hang = hang;
                        ghengoi.cot = i + 1;
                        ghengoi.phong_chieu_id = id;
                        ghengoi.gia_ghe = 75000;
                        //chưa sửa chọn ghế : VIP| Thường ...
                        ghengoi.loai_ghe_id = idloaighe;
                        ghengoi.status = 1;
                        db.GheNgois.Add(ghengoi);
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    return Json(new { checkr = false });
                }
            }
            return Json(new { success = true });
        }






        public ActionResult GetByRow(string Row, int? pcid)
        {
            var seat = db.GheNgois.Where(x => x.hang == Row && x.phong_chieu_id == pcid && x.status == 1).Count();
            var idLoaiGhe = db.GheNgois.Where(x => x.hang == Row && x.phong_chieu_id == pcid && x.status == 1).FirstOrDefault();
            List<int?> idloaig = new List<int?>();
            List<string> tenloaig = new List<string>();
            idloaig.Add(idLoaiGhe.loai_ghe_id);
            tenloaig.Add(idLoaiGhe.LoaiGhe.ten_ghe);
            foreach (var item in db.LoaiGhes.ToList())
            {
                idloaig.Add(item.id);
                tenloaig.Add(item.ten_ghe);
            }
            var count = idloaig.Count();
            return Json(data: new { count, idloaig, tenloaig, seat, Row }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult EditSeat(string hang, int ghe, int id, int idloaighe)
        {
            if (ghe > 20)
            {
                return Json(new { checkslg = false });
            }
            var ghn = db.GheNgois.Where(n => n.hang == hang && n.phong_chieu_id == id && n.status == 1).ToList();
            var tong = ghn.Count();
            GheNgoi ghengoi = new GheNgoi();
            var checkghe = db.GheNgois.Where(x => x.phong_chieu_id == id && x.status == 1).ToList();
            var checkpc = db.PhongChieux.Find(id);
            if (checkpc.status == 0)
            {
                return Json(new { success = false });
            }

            var check = db.SuatChieux.Where(x => x.phong_chieu_id == id && x.status == 1);
            TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
            int dem = 0;
            foreach (var item in check)
            {
                // check suất chiếu xem hết chưa
                if (item.ngay_chieu + timecheck > DateTime.Now)
                {
                    dem++;
                }

            }
            //không cho thêm sửa nếu có suất chiếu
            if (dem > 0)
            {
                return Json(new { success = false });
            }


            try
            {
                if (tong < ghe)
                {
                    checkpc.so_ghe = checkghe.Count() + (ghe - tong);
                    db.Entry(checkpc).State = EntityState.Modified;
                    //Sửa ghế trước rồi tạo mới ghế sau
                    foreach (var gheht in ghn)
                    {
                        GheNgoi seat = db.GheNgois.Find(gheht.id);
                        seat.loai_ghe_id = idloaighe;
                        db.Entry(seat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    for (int i = tong; i < ghe; i++)
                    {
                        ghengoi.hang = hang;
                        ghengoi.cot = i + 1;
                        ghengoi.phong_chieu_id = id;
                        ghengoi.gia_ghe = 75000;
                        //chưa sửa chọn ghế : VIP| Thường ...
                        ghengoi.loai_ghe_id = idloaighe;
                        ghengoi.status = 1;
                        db.GheNgois.Add(ghengoi);
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                }
                else if (tong > ghe)
                {
                    checkpc.so_ghe = checkghe.Count() - (tong - ghe);
                    db.Entry(checkpc).State = EntityState.Modified;
                    foreach (var item in ghn)
                    {
                        if (item.cot > ghe)
                        {
                            GheNgoi seat = db.GheNgois.Find(item.id);
                            seat.status = 0;
                            db.Entry(seat).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    var ghehientai = db.GheNgois.Where(n => n.hang == hang && n.phong_chieu_id == id && n.status == 1).ToList();
                    foreach (var gheht in ghehientai)
                    {
                        GheNgoi seat = db.GheNgois.Find(gheht.id);
                        seat.loai_ghe_id = idloaighe;
                        db.Entry(seat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                }
                else if (tong == ghe)
                {
                    foreach (var idghe in ghn)
                    {
                        GheNgoi seat = db.GheNgois.Find(idghe.id);
                        seat.loai_ghe_id = idloaighe;
                        db.Entry(seat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult DeleteSeat(string hang, int id)
        {

            var check = db.SuatChieux.Where(x => x.phong_chieu_id == id && x.status == 1);
            TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
            int dem = 0;
            foreach (var item in check)
            {
                // check suất chiếu xem hết chưa
                if (item.ngay_chieu + timecheck > DateTime.Now)
                {
                    dem++;
                }

            }
            //không cho thêm sửa nếu có suất chiếu
            if (dem > 0)
            {
                return Json(new { success = false });
            }

            var ghn = db.GheNgois.Where(n => n.hang == hang && n.phong_chieu_id == id && n.status == 1).ToList();
            GheNgoi ghengoi = new GheNgoi();
            var checkghe = db.GheNgois.Where(x => x.phong_chieu_id == id && x.status == 1).ToList();
            var checkpc = db.PhongChieux.Find(id);
            if (checkpc.status == 0)
            {
                return Json(new { success = false });
            }
            try
            {
                foreach (var item in ghn)
                {
                    GheNgoi seat = db.GheNgois.Find(item.id);
                    seat.status = 0;
                    db.Entry(seat).State = EntityState.Modified;
                    db.SaveChanges();
                }
                checkpc.so_ghe = checkghe.Count() - ghn.Count();
                db.Entry(checkpc).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }


        //Tạo loại ghế
        public ActionResult ListSeatType(int? page)
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
            return View(db.LoaiGhes.ToList().OrderBy(n => n.id).ToPagedList(pageNumber, pageSize));
        }

        //them loai ghe
        public ActionResult CreateSeatType()
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
        public ActionResult CreateSeatType(LoaiGhe loaiGhe)
        {
            if (ModelState.IsValid)
            {

                var file = Request.Files["anh_loai_ghe"];
                if (file != null && file.ContentLength > 0)
                {
                    Random rd = new Random();
                    var numrd = rd.Next(1, 100).ToString();
                    string filename = "seattype" + DateTime.Now.ToString("dd-MM-yyyy") + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    loaiGhe.anh_loai_ghe = filename;
                    string path = Server.MapPath("~/images/seattype/");
                    string StrPath = Path.Combine(path, filename);
                    file.SaveAs(StrPath);
                }
                loaiGhe.status = 1;
                db.LoaiGhes.Add(loaiGhe);
                TempData["Message"] = "Tạo loại ghế thành công!";
                db.SaveChanges();
                return RedirectToAction("ListSeatType");
            }

            return View(loaiGhe);
        }

        public ActionResult EditSeatType(int? id)
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
            if (db.GheNgois.Where(n => n.loai_ghe_id == id && n.status != 0 && n.PhongChieu.status != 0).Count() != 0)
            {
                TempData["Warning"] = "Không thể sửa vì có phòng đang dùng loại ghế này!";
                return RedirectToAction("ListSeatType");
            }
            try
            {

                LoaiGhe loaiGhe = db.LoaiGhes.Find(id);
                if (loaiGhe == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                return View(loaiGhe);
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditSeatType(LoaiGhe loaiGhe)
        {
            if (db.GheNgois.Where(n => n.loai_ghe_id == loaiGhe.id && n.status != 0 && n.PhongChieu.status != 0).ToList().Count() != 0)
            {
                TempData["Warning"] = "Không thể sửa vì có phòng đang dùng loại ghế này!";
                return RedirectToAction("ListSeatType");
            }
            if (ModelState.IsValid)
            {
                var file = Request.Files["anh_loai_ghe"];
                if (file != null && file.ContentLength > 0)
                {
                    Random rd = new Random();
                    var numrd = rd.Next(1, 100).ToString();
                    string filename = "seattype" + DateTime.Now.ToString("dd-MM-yyyy") + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    loaiGhe.anh_loai_ghe = filename;
                    string path = Server.MapPath("~/images/seattype/");
                    string StrPath = Path.Combine(path, filename);
                    file.SaveAs(StrPath);
                }
                db.Entry(loaiGhe).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật loại ghế thành công!";
                db.SaveChanges();
                return RedirectToAction("ListSeatType");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công!";
            }
            return View(loaiGhe);
        }

        public ActionResult DeleteSeatType(int? id)
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
            if (db.GheNgois.Where(n => n.loai_ghe_id == id && n.status != 0 && n.PhongChieu.status != 0).ToList().Count() == 0)
            {
                LoaiGhe loaiGhe = db.LoaiGhes.Find(id);
                db.LoaiGhes.Remove(loaiGhe);
                TempData["Message"] = "Xóa thành công!";
                db.SaveChanges();
                return RedirectToAction("ListSeatType");
            }
            else
            {
                TempData["Error"] = "Không thể xóa vì loại ghế đang tồn tại trong rạp chiếu!";
                return RedirectToAction("ListSeatType");
            }
        }


    }
}