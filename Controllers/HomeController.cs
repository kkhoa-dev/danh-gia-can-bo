using DanhGiaCanBo.Models;
using DanhGiaCanBo.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DanhGiaCanBo.Controllers
{
    public class HomeController : Controller
    {
        private DanhGiaCBEntities db = new DanhGiaCBEntities();
        public ActionResult Index(string keyword = null, int page = 1, int pagesize = 8)
        {
            if(keyword != null)
            {
                var model = db.tbl_CanBo.Where(x => x.HoTen.Contains(keyword) || x.MaCB.Contains(keyword)).OrderByDescending(x => x.HoTen).ToPagedList(page, pagesize);
                ViewBag.keyword = keyword;
                return View(model);
            }
            else
            {
                var model = db.tbl_CanBo.OrderByDescending(x => x.HoTen).ToPagedList(page, pagesize);
                return View(model);
            }
            
            
        }

        public ActionResult Detail(int ID)
        {
            ViewBag.lstKhaosat = db.tbl_KhaoSat.ToList();
            ViewBag.CanBo = db.tbl_CanBo.Find(ID);
            return View();
        }

        public JsonResult addDanhGia(string json_danhgia)
        {
            var Str_Json = new JavaScriptSerializer().Deserialize<List<tbl_DanhGia>>(json_danhgia);

            try
            {
                foreach (var item in Str_Json)
                {
                    var danhgia = new tbl_DanhGia();
                    danhgia.CanBo_ID = item.CanBo_ID;
                    danhgia.KhaoSat_ID = item.KhaoSat_ID;
                    danhgia.NoiDung = item.NoiDung;
                    danhgia.GhiChu = item.GhiChu != "" || item.GhiChu != null ? item.GhiChu : null;
                    danhgia.NgayDG = DateTime.Now;
                    db.tbl_DanhGia.Add(danhgia);
                    db.SaveChanges();
                }

                return Json(new
                {
                    status = true
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }
            
        }

        public JsonResult ListName(string q)
        {
            return Json(new
            {
                data = db.tbl_CanBo.Where(x => x.HoTen.Contains(q) || x.MaCB.Contains(q)).Select(x => new { x.HoTen, x.MaCB, x.Anh}).ToList(),
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Success()
        {
            return View();
        }

    }
}