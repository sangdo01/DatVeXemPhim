using BookingCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingCinema.Controllers
{
    public class ReceptionPaymentController : Controller
    {
        private DatVeXemPhimDBContext db = new DatVeXemPhimDBContext();
        // GET: ReceptionPayment

        public ActionResult withReceptionPay()
        {
            string idghe = TempData["idghe"].ToString();
            string idsc = TempData["idsuatc"].ToString();
            string idtime = TempData["idtime"].ToString();
            string idkh = TempData["idkh"].ToString();

            int idsuatchieu = Convert.ToInt32(idsc);
            int idtimechieu = Convert.ToInt32(idtime);
            //check
            var checkorder = db.Orders.Where(x => x.suatchieu_id == idsuatchieu && x.idtime == idtimechieu).ToList();
            List<int> idghengoi = new List<int>();
            string[] listid = idghe.Split(',');
            for (int i = 0; i < listid.Length; i++)
            {
                if (listid[i] != "")
                {
                    idghengoi.Add(Convert.ToInt32(listid[i]));
                }
            }

            //tổng tiền
            decimal? price = 0;
            foreach (var i in idghengoi)
            {
                var priceghe = db.GheNgois.Find(Convert.ToInt32(i));
                price += (priceghe.gia_ghe + priceghe.LoaiGhe.phu_thu);
            }

            int dem = 0;
            foreach (var item in idghengoi)
            {
                foreach (var i in checkorder)
                {
                    var checkdetails = db.CT_Orders.Where(x => x.ghe_id == item && x.orders_id == i.id && i.status >= 1);
                    if (checkdetails.Count() != 0)
                    {
                        dem++;
                    }
                }
            }
            if (dem > 0)
            {
                TempData["Error"] = "Ghế đã có người vừa đặt, Vui lòng chọn ghế khác!";
                return RedirectToAction("Index", "Home");
            }
            //thanh toan
            var sc = db.SuatChieux.Find(idsuatchieu);
            Order addorder = new Order();
            addorder.khachhang_id = Convert.ToInt32(TempData["idkh"].ToString());
            addorder.phim_id = sc.phim_id;
            //addorder.ten_phim = sc.phim.ten_phim;
            addorder.phong_chieu_id = sc.phong_chieu_id;
            //addorder.ten_phong_chieu = sc.phong_chieu.ten_phong;
            addorder.suatchieu_id = idsuatchieu;
            addorder.ngay_mua = DateTime.Now;
            addorder.idtime = idtimechieu;
            addorder.phuong_thuc_thanh_toan = "Thanh toán tại quầy";
            //Chờ thanh toán
            addorder.status = 2;
            addorder.tong_tien = price;
            addorder.so_luong_ve = idghengoi.Count();


            db.Orders.Add(addorder);
            db.SaveChanges();
            //add order details
            int idorder = addorder.id;
            CT_Orders addorderdetails = new CT_Orders();
            foreach (var gheid in idghengoi)
            {
                var tiendetail = db.GheNgois.Find(gheid);
                addorderdetails.ghe_id = gheid;
                addorderdetails.orders_id = idorder;
                addorderdetails.giave = tiendetail.gia_ghe + tiendetail.LoaiGhe.phu_thu;
                db.CT_Orders.Add(addorderdetails);
                db.SaveChanges();
            }
            //send
            var kh = db.KhachHangs.Find(Convert.ToInt32(TempData["idkh"]));
            var timechieu = db.KhungGios.Find(idtimechieu);
            var orderformail = db.Orders.Find(idorder);
            var phimfind = db.Phims.Find(orderformail.phim_id);
            var pcfind = db.PhongChieux.Find(orderformail.phong_chieu_id);
            string phim = phimfind.ten_phim;
            string phongchieu = pcfind.ten_phong_chieu;
            string ghee = "Vé ";
            var demm = 0;
            foreach (var j in idghengoi)
            {
                demm++;
                var tenghh = db.GheNgois.Find(Convert.ToInt32(j));
                if (demm == idghengoi.Count())
                {
                    ghee += tenghh.hang + tenghh.cot;
                }
                else
                {
                    ghee += tenghh.hang + tenghh.cot + ", ";
                }
            }

            try
            {
                //string mail = System.IO.File.ReadAllText(Server.MapPath("~/Library/ReplyMail.html"));
                //string dt = DateTime.Now.ToString();
                //mail = mail.Replace("{{Name}}", kh.ho_ten.ToString());
                //mail = mail.Replace("{{Phone}}", kh.sdt.ToString());
                //mail = mail.Replace("{{Email}}", kh.email.ToString());
                //mail = mail.Replace("{{Phim}}", phim);
                //mail = mail.Replace("{{Suatchieu}}", timechieu.Time.ToString());
                //mail = mail.Replace("{{Rap}}", phongchieu);
                //mail = mail.Replace("{{Ve}}", ghee);
                //mail = mail.Replace("{{Amount}}", String.Format("{0:0,0}", orderformail.tong_tien));
                //mail = mail.Replace("{{date}}", dt);
                //var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                //Random rd = new Random();
                //var numrd = rd.Next(1, 1000000).ToString();
                //new SendMail().SendMailTo(kh.email.ToString(), "Xác nhận thanh toán [CINEMA" + numrd + "]", mail);
                TempData["Message"] = "Đặt vé thành công!";
                return RedirectToAction("Success", "ReceptionPayment", new { idord = addorder.id });
            }
            catch (Exception)
            {

                TempData["Message"] = "Đặt vé thành công!";
                return RedirectToAction("Success", "ReceptionPayment", new { idord = addorder.id });
            }
        }

        public ActionResult Success(int? idord)
        {
            if (Session["TenKH"] == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập";
                return RedirectToAction("Login", "User");
            }

            if (idord == null)
            {
                TempData["Warning"] = "Không phải tài khoản của bạn";
                return RedirectToAction("Index", "Home");
            }

            var makh = Convert.ToInt32(Session["MaKH"]);
            var ord = db.Orders.Find(idord);
            if (ord.khachhang_id != makh)
            {
                TempData["Warning"] = "Không phải tài khoản của bạn";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.kh = db.KhachHangs.Find(ord.khachhang_id);
            ViewBag.film = db.Phims.Find(ord.phim_id);
            var dsghe = db.CT_Orders.Where(x => x.orders_id == ord.id);
            int count = dsghe.Count();
            int dem = 0;
            string dayghe = "";
            foreach (var i in dsghe)
            {
                dem++;
                var ghe = db.GheNgois.Find(i.ghe_id);
                if (dem == count)
                {
                    dayghe += ghe.hang + ghe.cot;
                }
                else
                {
                    dayghe += ghe.hang + ghe.cot + ", ";
                }
            }


            string qRCode = "Vé xem phim: ghế " + dayghe + ". Trạng thái chưa thanh toán!";


            //string path = Server.MapPath("~/images/qrcode/");

            //QRCodeData qrCodeData1 = new QRCodeData(path + ord.code_ticket + ".qrr", QRCodeData.Compression.Uncompressed);
            //QRCode QrCode = new QRCode(qrCodeData1);
            //Bitmap QrBitmap = QrCode.GetGraphic(60);
            //byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            //string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));

            //ViewBag.QrCodeUri = QrUri;
            ViewBag.ghe = dayghe;
            TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
            var tinhthem = ord.ngay_mua + tinhgio;
            ViewBag.timetopay = Convert.ToDateTime(tinhthem).ToString("HH:mm");
            ViewBag.time = Convert.ToDateTime(tinhthem).ToString("MM/dd/yyyy HH:mm:ss");
            return View(ord);
        }

    }

}
