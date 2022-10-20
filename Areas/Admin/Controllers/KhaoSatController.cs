using DanhGiaCanBo.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DanhGiaCanBo.Areas.Admin.Controllers
{
    public class KhaoSatController : Controller
    {
        // GET: Admin/KhaoSat
        private DanhGiaCBEntities db = new DanhGiaCBEntities();
        // GET: khaosat
        public ActionResult Index()
        {
            var model = db.tbl_KhaoSat.OrderByDescending(x => x.CreatedDate);
            return View(model.ToList());
        }

        public JsonResult Delete(int ID)
        {

            try
            {
                var model = db.tbl_KhaoSat.Find(ID);
                db.tbl_KhaoSat.Remove(model);
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
        public ActionResult frmAdd(tbl_KhaoSat entity)
        {
            try
            {
                entity.CreatedDate = DateTime.Now;
                db.tbl_KhaoSat.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm khảo sát thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/khaosat");

            }
            catch
            {
                TempData["message"] = "Thêm khảo sát KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/khaosat");
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmEdit(tbl_KhaoSat entity)
        {
            try
            {
                var model = db.tbl_KhaoSat.Find(entity.ID);
                model.NoiDung = entity.NoiDung;
                model.UpdateDate = DateTime.Now;
                db.SaveChanges();
                TempData["message"] = "Cập nhật khảo sát thành công";
                return Redirect("/admin/khaosat");
            }
            catch
            {
                TempData["message"] = "Cập nhật khảo sát KHÔNG thành công";
                return Redirect("/admin/khaosat");
            }
        }

        public JsonResult GetByID(int ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = db.tbl_KhaoSat.Find(ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}