using DanhGiaCanBo.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DanhGiaCanBo.Areas.Admin.Controllers
{
    public class CanBoController : Controller
    {
        // GET: Admin/CanBo
        private DanhGiaCBEntities db = new DanhGiaCBEntities();
        public ActionResult Index(int page = 1, int pagesize = 15)
        {
            var model = db.tbl_CanBo.OrderByDescending(x => x.ID);
            ViewBag.lstQuay = db.tbl_Quay.ToList();
            return View(model.ToPagedList(page, pagesize));
        }

        public JsonResult Delete(int ID)
        {

            try
            {
                var model = db.tbl_CanBo.Find(ID);
                var model1 = db.tbl_DanhGia.Where(m => m.CanBo_ID == ID).ToList();
                
                //Xóa file ảnh
                if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Assets/Client/img/canbo"), model.Anh)))
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/canbo"), model.Anh));
                }

                db.tbl_DanhGia.RemoveRange(model1);
                db.tbl_CanBo.Remove(model);
                db.SaveChanges();
               // db.SaveChanges();
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

        public JsonResult ChangeStatus(int ID)
        {
            try
            {
                var model = db.tbl_CanBo.Find(ID);
                if (model.TrangThai == true)
                    model.TrangThai = false;
                else
                    model.TrangThai = true;
                db.SaveChanges();
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

        public ActionResult Add()
        {
            ViewBag.lstQuay = db.tbl_Quay.ToList();
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmAdd(tbl_CanBo entity, HttpPostedFileBase Anh)
        {
            try
            {
                entity.TrangThai = true;

                var path = Path.Combine(Server.MapPath("~/Assets/Client/img/canbo"), Anh.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Anh.FileName);
                    string filename = Path.GetFileNameWithoutExtension(path) + DateTime.Now.ToString("hhmmssddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/Client/img/canbo"), filename);
                    Anh.SaveAs(path);
                    entity.Anh = Anh.FileName;
                }
                else
                {
                    Anh.SaveAs(path);
                    entity.Anh = Anh.FileName;
                }

                db.tbl_CanBo.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm cán bộ thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/canbo");

            }
            catch
            {
                TempData["message"] = "Thêm cán bộ KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/canbo");
            }
        }


        public ActionResult Edit(int ID)
        {

            ViewBag.lstQuay = db.tbl_Quay.ToList();
            ViewBag.CanBo = db.tbl_CanBo.Find(ID);
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmEdit(tbl_CanBo entity, HttpPostedFileBase Anh)
        {
            try
            {
                var model = db.tbl_CanBo.Find(entity.ID);
                model.HoTen = entity.HoTen;
                model.NgaySinh = entity.NgaySinh;
                model.QueQuan = entity.QueQuan;
                model.GioiTinh = entity.GioiTinh;
                model.SDT = entity.SDT;
                model.NgayLamViec = entity.NgayLamViec;
                model.GioiThieu = entity.GioiThieu;
                model.Quay_ID = entity.Quay_ID;

                if (Anh != null && model.Anh != Anh.FileName)
                {
                    //Xóa file cũ
                    if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Assets/Client/img/canbo"), model.Anh)))
                    {
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/canbo"), model.Anh));
                    }
                    //Thêm hình ảnh
                    var path = Path.Combine(Server.MapPath("~/Assets/Client/img/canbo"), Anh.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        string extensionName = Path.GetExtension(Anh.FileName);
                        string filename = Path.GetFileNameWithoutExtension(path) + DateTime.Now.ToString("hhmmssddMMyyyy") + extensionName;
                        path = Path.Combine(Server.MapPath("~/Assets/Client/img/canbo/"), filename);
                        Anh.SaveAs(path);
                        model.Anh = Anh.FileName;
                    }
                    else
                    {
                        Anh.SaveAs(path);
                        model.Anh = Anh.FileName;
                    }
                }

                db.SaveChanges();
                TempData["message"] = "Cập nhật cán bộ thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/canbo");
            }
            catch
            {
                TempData["message"] = "Cập nhật cán bộ KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/canbo");
            }
        }

        public JsonResult Check_Exist_MaCB(string MaCB)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cb = db.tbl_CanBo.Count(x => x.MaCB == MaCB);
            if (cb > 0)
            {
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }


        }

    }
}