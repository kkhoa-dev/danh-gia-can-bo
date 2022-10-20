using DanhGiaCanBo.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DanhGiaCanBo.Areas.Admin.Controllers
{
    public class QuayController : Controller
    {
        // GET: Admin/Quay
        private DanhGiaCBEntities db = new DanhGiaCBEntities();
        // GET: quay
        public ActionResult Index()
        {
            var model = db.tbl_Quay.OrderByDescending(x => x.ID);
            return View(model.ToList());
        }

        public JsonResult Delete(int ID)
        {

            try
            {
                var model = db.tbl_Quay.Find(ID);
                db.tbl_Quay.Remove(model);
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


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmAdd(tbl_Quay entity)
        {
            try
            {
                db.tbl_Quay.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm quầy thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/quay");

            }
            catch
            {
                TempData["message"] = "Thêm quầy KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/quay");
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmEdit(tbl_Quay entity)
        {
            try
            {
                var model = db.tbl_Quay.Find(entity.ID);
                model.Ten = entity.Ten;
                model.ND_HoTro = entity.ND_HoTro;
                db.SaveChanges();
                TempData["message"] = "Cập nhật quầy thành công";
                return Redirect("/admin/quay");
            }
            catch
            {
                TempData["message"] = "Cập nhật quầy KHÔNG thành công";
                return Redirect("/admin/quay");
            }
        }

        public JsonResult GetByID(int ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = db.tbl_Quay.Find(ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}