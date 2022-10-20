using ClosedXML.Excel;
using DanhGiaCanBo.Models.DTO;
using DanhGiaCanBo.Models.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DanhGiaCanBo.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private DanhGiaCBEntities db = new DanhGiaCBEntities();
        // GET: Admin/Home
        public ActionResult Index(int? thang)
        {
            //Số cán bộ
            ViewBag.Count_cb = db.tbl_CanBo.Count();

            //số quầy
            ViewBag.Count_quay = db.tbl_Quay.Count();

            //Lượt đánh giá
            ViewBag.Count_danhgia = db.tbl_DanhGia.Count();

            //Câu khảo sat
            ViewBag.Count_khaosat = db.tbl_KhaoSat.Count();

            //Lượt đánh giá rất hài lòng
            ViewBag.Count_rat_hailong = db.tbl_DanhGia.Where(x => x.NoiDung == "Rất hài lòng").Count();

            //Lượt đánh giá rất hài lòng
            ViewBag.Count_chua_hailong = db.tbl_DanhGia.Where(x => x.NoiDung == "Chưa hài lòng").Count();


            //Thống kê danh sách đánh giá
            var lstcanbo_id = db.tbl_DanhGia.Select(x => x.CanBo_ID).Distinct();
            var listdanh_gia = new List<DanhGiaDTO>();
            
            foreach (var item in lstcanbo_id)
            {
                var canbo = db.tbl_CanBo.Find(item);
                var danhgia = new DanhGiaDTO();
                danhgia.HoTen = canbo.HoTen;
                danhgia.ChuaHaiLong = 0;
                danhgia.HaiLong = 0;
                danhgia.RatHaiLong = 0;
                
                if (thang != null)
                {
                    foreach (var jtem in db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == thang))
                    {
                        if (jtem.NoiDung == "Chưa hài lòng")
                            danhgia.ChuaHaiLong++;
                        if (jtem.NoiDung == "Hài lòng")
                            danhgia.HaiLong++;
                        if (jtem.NoiDung == "Rất hài lòng")
                            danhgia.RatHaiLong++;
                    }
                    
                    ViewBag.Thang = thang;
                    danhgia.tong = db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == thang).Count();
                }
                else
                {
                    int month = DateTime.Now.Month;
                    ViewBag.Thang = month;
                    danhgia.tong = db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == month).Count();
                    foreach (var jtem in db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == month))
                    {
                        if (jtem.NoiDung == "Chưa hài lòng")
                            danhgia.ChuaHaiLong++;
                        if (jtem.NoiDung == "Hài lòng")
                            danhgia.HaiLong++;
                        if (jtem.NoiDung == "Rất hài lòng")
                            danhgia.RatHaiLong++;
                    }
                }
                
                listdanh_gia.Add(danhgia);
            }
            ViewBag.lst_DanhGia = listdanh_gia.OrderByDescending(x => x.HoTen).ToList();

            //Lấy ý kiến khác
            var listghi_chu = new List<DanhGiaDTO>();
            foreach (var item in lstcanbo_id)
            {
                var canbo = db.tbl_CanBo.Find(item);
                var danhgia = new DanhGiaDTO();
                danhgia.HoTen = canbo.HoTen;
                danhgia.GhiChu = "";
                if (thang != null)
                {
                    foreach (var jtem in db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == thang).Select(x => x.GhiChu).Distinct())
                    {
                        if (!listghi_chu.Exists(x => x.GhiChu.Contains(jtem)))
                            danhgia.GhiChu += "<p>" + jtem + "</p>";
                    }
                }
                else
                {
                    int month = DateTime.Now.Month;
                    foreach (var jtem in db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == month).Select(x => x.GhiChu).Distinct())
                    {
                        if (!listghi_chu.Exists(x => x.GhiChu.Contains(jtem)))
                            danhgia.GhiChu += "<p>" + jtem + "</p>";
                    }
                }
                listghi_chu.Add(danhgia);
            }

            ViewBag.lst_GhiChu = listghi_chu.OrderByDescending(x => x.HoTen).ToList();


            //Thống kê đánh giá hôm nay
            ViewBag.Review_today = db.tbl_DanhGia.Where(x => DbFunctions.TruncateTime(x.NgayDG) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Thống kê đánh giá chưa hài lòng hôm nay
            ViewBag.chuahailong = db.tbl_DanhGia.Where(x => x.NoiDung == "Chưa hài lòng" && DbFunctions.TruncateTime(x.NgayDG) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Thống kê đánh giá  hài lòng hôm nay
            ViewBag.hailong = db.tbl_DanhGia.Where(x => x.NoiDung == "Hài lòng" && DbFunctions.TruncateTime(x.NgayDG) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Thống kê đánh giá rất hài lòng hôm nay
            ViewBag.rathailong = db.tbl_DanhGia.Where(x => x.NoiDung == "Rất hài lòng" && DbFunctions.TruncateTime(x.NgayDG) == DbFunctions.TruncateTime(DateTime.Now)).Count();


            return View();
        }

        public ActionResult DownloadExcel(int? thang)
        {
            var lstcanbo_id = db.tbl_DanhGia.Select(x => x.CanBo_ID).Distinct();
            var listdanh_gia = new List<DanhGiaDTO>();
            foreach (var item in lstcanbo_id)
            {
                var canbo = db.tbl_CanBo.Find(item);
                var danhgia = new DanhGiaDTO();
                danhgia.HoTen = canbo.HoTen;
                danhgia.Quay = canbo.tbl_Quay.Ten;
                danhgia.ChuaHaiLong = 0;
                danhgia.HaiLong = 0;
                danhgia.RatHaiLong = 0;
                foreach (var jtem in db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == thang))
                {
                    if (jtem.NoiDung == "Chưa hài lòng")
                        danhgia.ChuaHaiLong++;
                    if (jtem.NoiDung == "Hài lòng")
                        danhgia.HaiLong++;
                    if (jtem.NoiDung == "Rất hài lòng")
                        danhgia.RatHaiLong++;

                }
                if (thang != null)
                {
                    foreach (var jtem in db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == thang).Select(x => x.GhiChu).Distinct())
                    {
                        if (!listdanh_gia.Exists(x => x.GhiChu.Contains(jtem)))
                            danhgia.GhiChu += "•" + jtem + "\n";

                    }
                }
                else
                {
                    int month = DateTime.Now.Month;
                    foreach (var jtem in db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == month).Select(x => x.GhiChu).Distinct())
                    {
                        if (!listdanh_gia.Exists(x => x.GhiChu.Contains(jtem)))
                            danhgia.GhiChu += "•"+ jtem + "\n";
                    }
                }

                danhgia.tong = db.tbl_DanhGia.Where(x => x.CanBo_ID == item && x.NgayDG.Value.Month == thang).Count();
                listdanh_gia.Add(danhgia);
            }


            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("TT"),
                                            new DataColumn("Họ và tên"),
                                            new DataColumn("Quầy"),
                                            new DataColumn("Rất hài lòng (%)"),
                                            new DataColumn("Hài lòng (%)"),
                                            new DataColumn("Chưa hài lòng (%)"),
                                            new DataColumn("Ghi chú"),
        });;

            int dem = 1;
            foreach (var item in listdanh_gia)
            {
                int tong = (item.tong == 0 ? 1 : item.tong);
                var rathailong = (double)item.RatHaiLong / tong * 100;
                var hailong = (double)item.HaiLong / tong * 100;
                var chuahailong = (double)item.ChuaHaiLong / tong * 100;
                var Ghichu = item.GhiChu;
                dt.Rows.Add(dem, item.HoTen, item.Quay, rathailong, hailong, chuahailong, item.GhiChu);
                dem++;
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                      
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "thong-ke-luot-danh-gia-thang-" + thang.ToString() + ".xlsx");
                }
            }
        }
        public ActionResult TotalMonth()
        {
            int[] month = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var lstTotal = new List<DanhGiaDTO>();
            for (int i = 1; i <= 12; i++)
            {
                var danhgia = new DanhGiaDTO();
                danhgia.ChuaHaiLong = db.tbl_DanhGia.Where(x => x.NoiDung == "Chưa hài lòng" && x.NgayDG.Value.Month == i).Count();
                danhgia.HaiLong = db.tbl_DanhGia.Where(x => x.NoiDung == "Hài lòng" && x.NgayDG.Value.Month == i).Count();
                danhgia.RatHaiLong = db.tbl_DanhGia.Where(x => x.NoiDung == "Rất hài lòng" && x.NgayDG.Value.Month == i).Count();

                danhgia.thang = i;
                lstTotal.Add(danhgia);
            }
            return Json(lstTotal, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Total_Cirle()
        {
            var lstTotal = new List<DanhGiaDTO>();
           

            var lstCate = new List<string>();
            lstCate.Add("Chưa hài lòng");
            lstCate.Add("Hài lòng");
            lstCate.Add("Rất hài lòng");

            foreach(var item in lstCate)
            {
                var danhgia = new DanhGiaDTO();
                danhgia.danhgia = item;
                danhgia.tong = db.tbl_DanhGia.Where(x => x.NoiDung == item).Count();
                lstTotal.Add(danhgia);
            }
            return Json(lstTotal, JsonRequestBehavior.AllowGet);

        }
    }
}