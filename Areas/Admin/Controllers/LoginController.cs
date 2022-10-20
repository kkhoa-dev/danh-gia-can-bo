using DanhGiaCanBo.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DanhGiaCanBo.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private DanhGiaCBEntities db = new DanhGiaCBEntities();
        // GET: Admin/Login
        public ActionResult Index()
        {
            var model = db.tbl_Admin.Where(x => x.TaiKhoan != "admin").OrderByDescending(x => x.ID);
            return View(model.ToList());
        }

        public JsonResult Delete(int ID)
        {

            try
            {
                var model = db.tbl_Admin.Find(ID);
                db.tbl_Admin.Remove(model);
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

        public JsonResult ChangeStatus(int ID)
        {

            try
            {
                var model = db.tbl_Admin.Find(ID);
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


        [HttpPost]
        public ActionResult frmAdd(DanhGiaCanBo.Models.EF.tbl_Admin entity)
        {
            try
            {
                entity.TrangThai = true;
                db.tbl_Admin.Add(entity);
                db.SaveChanges();

                TempData["message"] = "Thêm tài khoản thành công";
                return Redirect("/admin/login");
            }
            catch
            {
                TempData["message"] = "Thêm tài khoản KHÔNG thành công";
                return Redirect("/admin/login");
            }
        }


        [HttpPost]
        public ActionResult frmEdit(DanhGiaCanBo.Models.EF.tbl_Admin entity)
        {
            try
            {
                var nv = db.tbl_Admin.Find(entity.ID);
                nv.HoTen = entity.HoTen;
               
                db.SaveChanges();
                TempData["message"] = "Cập nhật tài khoản thành công";
                return Redirect("/admin/login");
            }
            catch
            {
                TempData["message"] = "Cập nhật tài khoản KHÔNG thành công";
                return Redirect("/admin/login");
            }
        }

        public JsonResult GetByID(int ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = db.tbl_Admin.Find(ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmLogin(DanhGiaCanBo.Models.EF.tbl_Admin model)
        {
            if (db.tbl_Admin.Count(x => x.TaiKhoan == model.TaiKhoan && x.MatKhau == model.MatKhau) > 0)
            {
                Session["admin"] = db.tbl_Admin.SingleOrDefault(x => x.TaiKhoan == model.TaiKhoan && x.MatKhau == model.MatKhau);
                return Redirect("/admin/home");
            }
            else
            {
                TempData["error"] = "Tài khoản hoặc mật khẩu không chính xác";
                return Redirect("/admin/login/login");
            }
        }

        public ActionResult ChangePass()
        {
            return View();
        }


        [HttpPost]
        public ActionResult frmChangePass(string Old_Pass, string New_Pass)
        {
            var nv = Session["admin"] as DanhGiaCanBo.Models.EF.tbl_Admin;
            if (nv.MatKhau.Trim() != Old_Pass)
            {
                TempData["error"] = "Mật khẩu cũ không chính xác, vui lòng nhập lại";
                return Redirect("/admin/login/changepass");
            }
            else
            {
                var tk = db.tbl_Admin.Find(nv.ID);
                tk.MatKhau = New_Pass;
                db.SaveChanges();
                Session["admin"] = null;
                TempData["error"] = "Đổi mật khẩu thành công, vui lòng đăng nhập lại để tiếp tục.";
                return Redirect("/admin/login/login");
            }
        }


        public ActionResult Logout()
        {
            Session["admin"] = null;
            return Redirect("/admin/login/login");
        }
    }
}