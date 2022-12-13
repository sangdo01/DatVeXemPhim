    using BookingCinema.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public ActionResult BookTicket(int? id)
        {
            if (id != null)
            {
                if (Session["TenKH"] == null)
                {
                    TempData["Warning"] = "Vui lòng đăng nhập";
                    return RedirectToAction("Login", "User");
                }

                try
                {
                    var checkfilm = db.Phims.Find(id);
                    if (checkfilm == null)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                    if (checkfilm.comingsoon != 1 || checkfilm.status != 1)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
                ViewBag.tenphim = db.Phims.Find(id);
                //ViewBag.tenphim = db.Phims.Find(id);
                //Lấy ra list suất chiếu với id phim
                var ngay = db.SuatChieux.Where(n => n.phim_id == id && n.status == 1).OrderBy(n => n.ngay_chieu).ToList();
                List<DateTime?> ng = new List<DateTime?>();
                ViewBag.idphim = id;

                //Tạo biến congngay để cộng thêm vào ngày suất chiếu 1 ngày
                TimeSpan congngay = new TimeSpan(1, 0, 0, 0);

                foreach (var item in ngay)
                {
                    var checknull = db.SuatChieu_KhungGio.Where(x => x.suat_chieu_id == item.id).Count(); //Kiểm tra xem ngày chiếu này có giờ chiếu nào không
                    if (checknull > 0) //Nếu có giờ chiếu thì dô, không thì khỏi dô :)))
                    {
                        var ngaychieu = Convert.ToDateTime(item.ngay_chieu);
                        var datenow = DateTime.UtcNow.ToString("d");

                        if ((ngaychieu + congngay) > DateTime.Now)  //Nếu ngày chiếu của suất chiếu cộng thêm 1 ngày mà lớn hơn ngày hiện tại thì thêm vào list.
                        {
                            var dem = 0;
                            if (ngaychieu == Convert.ToDateTime(datenow))
                            {
                                //Lấy ra suất chiếu có trong item.ngay_chieu của mảng ngay
                                var suatchieu = db.SuatChieux.Where(n => n.ngay_chieu == item.ngay_chieu && n.phim_id == id && n.status == 1).ToList();
                                foreach (var sctoday in suatchieu)
                                {
                                    //Lấy ra danh sách giờ chiếu theo id suất chiếu của mảng suatchieu
                                    var suatChieuTime = db.SuatChieu_KhungGio.Where(n => n.suat_chieu_id == sctoday.id).OrderBy(x => x.khung_gio_id).ToList();
                                    TimeSpan timenow = DateTime.Now.TimeOfDay;
                                    foreach (var time in suatChieuTime)
                                    {
                                        if (time.KhungGio.ThoiGian > timenow) // Nếu giờ của suất chiếu lớn giờ của hiện tại thì có suất chiếu giờ đó, tăng biến đếm lên 1.
                                        {
                                            dem++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                dem = -1; // Nếu ngày chiếu là ngày mơi ngày mốt gì đó thì cho biết đếm = -1 để nó add thẳng dô list luôn :D
                            }

                            if (dem > 0)   // Nếu biến đếm > 0 thì add ngày vào (vì có suất chiếu trong ngày)
                                           // nếu = 0 thì không add (vì không có suất chiếu nào trong ngày ).
                            {
                                ng.Add(item.ngay_chieu);
                            }
                            if (dem < 0)
                            {
                                ng.Add(item.ngay_chieu);
                            }
                        }
                    }
                }
                //Lấy ra ViewBag ngày với điều kiện ngày không được trùng.
                ViewBag.date = ng.Distinct();


                ViewBag.rap = db.RapChieux.Where(n => n.status == 1).Distinct();
                var checkdate = ng.Distinct().Count();

                if (checkdate <= 0)
                {
                    TempData["Warning"] = "Phim này đã hết hoặc chưa có suất chiếu !";
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View();
        }

        public ActionResult ShowCinema(string ngay, string idphim)
        {
            if (ngay != "")
            {
                var ng = Convert.ToDateTime(ngay);
                int idfilm = Convert.ToInt32(idphim);
                var suatchieu = db.SuatChieux.Where(n => n.ngay_chieu == ng && n.phim_id == idfilm && n.status == 1).ToList();
                List<String> tenrap = new List<String>();


                foreach (var item in suatchieu)
                {
                    var suatChieuTime = db.SuatChieu_KhungGio.Where(n => n.suat_chieu_id == item.id).OrderBy(x => x.KhungGio.ThoiGian).ToList(); //OrderBy theo giờ chiếu của bảng Time

                    //Chạy vòng lặp trong list suất chiếu timeframe để thêm từ suất chiếu vào list.
                    foreach (var time in suatChieuTime)
                    {
                        var datenow = DateTime.UtcNow.ToString("d");
                        if (ng == Convert.ToDateTime(datenow))
                        {
                            //Tạo biến thời gian hiện tại để so sánh với giờ của suất chiếu.
                            TimeSpan timenow = DateTime.Now.TimeOfDay;
                            if (time.KhungGio.ThoiGian > timenow) // Nếu giờ của suất chiếu lớn hơn giờ của hiện tại thì thêm vào, nếu không thì không thêm.
                            {
                                tenrap.Add(item.PhongChieu.RapChieu.ten_rap);
                            }
                        }
                        else // Nếu ngày chiếu cũng là ngày mơi ngày mốt gì đó thì cho biết đếm -- để nó add dô list luôn
                             // vì suất chiếu ngày = ngày hiện tại thì mới cho ++ :D
                        {
                            tenrap.Add(item.PhongChieu.RapChieu.ten_rap);
                        }
                    }

                    //Chạy xong vòng lặp, Nếu biến dem > 0 thì thêm dấu - vào để phân biệt              

                }
                var rapphim = tenrap.Distinct();
                var Count = rapphim.Count();

                return Json(data: new { Count, rapphim }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ShowTime(string ngay, string idphim)
        {
            if (ngay != "")
            {
                var ng = Convert.ToDateTime(ngay);
                int idfilm = Convert.ToInt32(idphim);
                var suatchieu = db.SuatChieux.Where(n => n.ngay_chieu == ng && n.phim_id == idfilm && n.status == 1).ToList();

                //Tạo list để add giờ, id giờ, id suất chiếu
                List<String> times = new List<String>();
                List<String> idtimes = new List<String>();
                List<String> idsc = new List<String>();
                List<String> tenrap = new List<String>();
                string nhay = "-";
                times.Add(nhay);
                idtimes.Add(nhay);
                idsc.Add(nhay);
                tenrap.Add(nhay);

                //Chạy vòng lặp trong list suất chiếu lấy từ ngày chiếu và id phim.
                foreach (var item in suatchieu)
                {
                    var dem = 0;
                    var suatChieuTime = db.SuatChieu_KhungGio.Where(n => n.suat_chieu_id == item.id).OrderBy(x => x.KhungGio.ThoiGian).ToList(); //OrderBy theo giờ chiếu của bảng Time

                    //Chạy vòng lặp trong list suất chiếu timeframe để thêm từ suất chiếu vào list.
                    foreach (var time in suatChieuTime)
                    {
                        var datenow = DateTime.UtcNow.ToString("d");
                        if (ng == Convert.ToDateTime(datenow))
                        {
                            //Tạo biến thời gian hiện tại để so sánh với giờ của suất chiếu.
                            TimeSpan timeplus = new TimeSpan(0, 15, 0);
                            TimeSpan timenow = DateTime.Now.TimeOfDay;
                            if (time.KhungGio.ThoiGian + timeplus > timenow) // Nếu giờ của suất chiếu lớn hơn giờ của hiện tại thì thêm vào, nếu không thì không thêm.
                            {
                                times.Add((time.KhungGio.ThoiGian).ToString());
                                idtimes.Add(time.KhungGio.id.ToString());
                                idsc.Add(item.id.ToString());
                                tenrap.Add(item.PhongChieu.RapChieu.ten_rap);
                                dem++;
                            }
                        }
                        else // Nếu ngày chiếu cũng là ngày mơi ngày mốt gì đó thì cho biết đếm -- để nó add dô list luôn
                             // vì suất chiếu ngày = ngày hiện tại thì mới cho ++ :D
                        {
                            times.Add((time.KhungGio.ThoiGian).ToString());
                            idtimes.Add(time.KhungGio.id.ToString());
                            idsc.Add(item.id.ToString());
                            tenrap.Add(item.PhongChieu.RapChieu.ten_rap);
                            dem--;
                        }
                    }

                    //Chạy xong vòng lặp, Nếu biến dem > 0 thì thêm dấu - vào để phân biệt              
                    if (dem > 0)
                    {
                        times.Add(nhay);
                        idtimes.Add(nhay);
                        idsc.Add(nhay);
                        tenrap.Add(nhay);
                    }
                    else if (dem < 0)
                    {
                        times.Add(nhay);
                        idtimes.Add(nhay);
                        idsc.Add(nhay);
                        tenrap.Add(nhay);
                    }

                }

                var Count = times.Count();

                return Json(data: new { times, idtimes, Count, idsc, tenrap }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["Warning"] = "Phim này đã hết hoặc chưa có suất chiếu !";
                var slug = db.Phims.Find(Convert.ToInt32(idphim));
                return RedirectToAction("MovieDetail", "Movie", new { id = slug.id });
            }
        }


        //Chọn ghế
        public ActionResult BookSeat(string idd, string idtimee)
        {
            try
            {
                if (Session["TenKH"] == null)
                {
                    TempData["Warning"] = "Vui lòng đăng nhập";
                    return RedirectToAction("Login", "User");
                }

                if (idd == null || idtimee == null || idd == "" || idtimee == "")
                {
                    TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                    return RedirectToAction("Error", "Home");
                }

                var id = Convert.ToInt32(idd);
                var idtime = Convert.ToInt32(idtimee);
                var idsc = db.SuatChieux.Find(id);
                var time = db.SuatChieu_KhungGio.Where(x => x.suat_chieu_id == id && x.khung_gio_id == idtime).FirstOrDefault();

                var checktime = db.KhungGios.Find(idtime);
                if (idsc.ngay_chieu + checktime.ThoiGian < DateTime.Now)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                    return RedirectToAction("Error", "Home");
                }
                var checkfilm = db.Phims.Find(idsc.phim_id);
                //ràng buộc
                if (time == null || idsc == null || checkfilm == null)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                    return RedirectToAction("Error", "Home");
                }

                if (checkfilm.status != 1 || checkfilm.comingsoon != 1)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                    return RedirectToAction("Error", "Home");
                }

                if (idsc.status != 1)
                {
                    return RedirectToAction("Error", "Home");
                }

                ViewBag.ngaychieu = idsc.ngay_chieu;
                string time1 = time.KhungGio.ThoiGian.ToString();
                string[] time2 = time1.Split(':');
                string timef = time2[0] + ':' + time2[1];
                ViewBag.giochieu = timef;

                ViewBag.tenphim = checkfilm;
                PhongChieu phongChieu = db.PhongChieux.Find(idsc.phong_chieu_id);
                ViewBag.pc = phongChieu;
                var ghengoi = db.GheNgois.Where(x => x.phong_chieu_id == phongChieu.id && x.status == 1).OrderBy(x => x.hang);
                ViewBag.ghe = ghengoi;
                ViewBag.idtime = idtime;
                ViewBag.idsc = id;
                //ghế đang chờ thanh toán
                TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
                                                           //Status 2: đang chờ thanh toán tại quầy
                var orderss = db.Orders.Where(n => n.suatchieu_id == idsc.id && n.idtime == idtime && n.status == 2);
                var counttt = orderss.Count();
                List<int> idgheddss = new List<int>();
                foreach (var itemm in orderss)
                {
                    if (itemm.ngay_mua + tinhgio <= DateTime.Now)
                    {                      
                        itemm.status = 0;
                        db.Entry(itemm).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();

                var order = db.Orders.Where(n => n.suatchieu_id == idsc.id && n.idtime == idtime && n.status == 1 ||
                n.suatchieu_id == idsc.id && n.idtime == idtime && n.status == 2);
                List<int> idghedd = new List<int>();
                foreach (var item in order)
                {
                    var idghe = db.CT_Orders.Where(n => n.id == item.id);
                    foreach (var i in idghe)
                    {
                        idghedd.Add((int)i.ghe_id);
                    }
                }

                ViewBag.idghedat = idghedd;
                ViewBag.loaighe = db.LoaiGhes.ToList();
                return View(ghengoi);
            }
            catch (Exception)
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error", "Home");
            }
        }


        //Thanh toán
        public ActionResult CheckOut(int? id, int? idtime, string idg)
        {
            if (Session["TenKH"] == null)
            {
                string CurrentURL = HttpContext.Request.Url.AbsoluteUri;
                TempData["Warning"] = "Vui lòng đăng nhập";
                return RedirectToAction("Login", "User", new { url = CurrentURL });
            }

            if (id == null || idtime == null || idg == null)
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error", "Home");
            }

            if (id < 1 || idtime < 1 || idg == "")
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error", "Home");
            }
            var sc = db.SuatChieux.Find(id);
            var mkh = Convert.ToInt32(Session["MaKH"]);
            var kh = db.KhachHangs.Find(mkh);
            var checkfilm = db.Phims.Find(sc.phim_id);
            var timefr = db.KhungGios.Find(idtime);

            if (sc == null || kh == null || timefr == null || checkfilm == null)
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error", "Home");
            }


            //ràng buộc kiểm tra xem phim này đang công chiếu với có ẩn hay không
            if (checkfilm.status != 1 || checkfilm.comingsoon != 1)
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error", "Home");
            }

            // stt = 1 là suất chiếu đang công chiếu 
            if (sc.status != 1)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewBag.time = timefr;
            ViewBag.kh = kh;
            List<String> idghe = new List<String>();
            string[] listid = idg.Split(',');
            for (int i = 0; i < listid.Length; i++)
            {
                if (listid[i] != "")
                {
                    idghe.Add(listid[i]);
                }
            }
            //tính tiền
            decimal? price = 0;
            foreach (var i in idghe)
            {
                var priceghe = db.GheNgois.Find(Convert.ToInt32(i));
                price += (priceghe.gia_ghe + priceghe.LoaiGhe.phu_thu);
            }
            ViewBag.giatien = price;
            ViewBag.idghengoi = idghe;
            ViewBag.soluongh = idghe.Count();
            return View(sc);
        }

        public ActionResult ReceptionPay(FormCollection f)
        {
            TempData["idghe"] = Request.Form["idghe"];
            TempData["idsuatc"] = Request.Form["idsuatc"];
            TempData["idtime"] = Request.Form["idtime"];
            TempData["idkh"] = int.Parse(Session["MaKH"].ToString());
            return RedirectToAction("withReceptionPay", "ReceptionPayment");
        }



    }
}