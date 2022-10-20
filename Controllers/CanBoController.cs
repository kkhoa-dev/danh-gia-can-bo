using DanhGiaCanBo.Models;
using DanhGiaCanBo.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DanhGiaCanBo.Controllers
{
    public class CanBoController : Controller
    {
        private DanhGiaCBEntities db = new DanhGiaCBEntities();
        // GET: CanBo
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmLogin(tbl_CanBo entity)
        {
            var res = db.tbl_CanBo.Count(x => x.MaCB.Trim() == entity.MaCB && x.MatKhau == entity.MatKhau);

            if (res > 0)
            {
                var tbl_CanBo = db.tbl_CanBo.FirstOrDefault(x => x.MaCB.Trim() == entity.MaCB && x.MatKhau == entity.MatKhau);
                if (tbl_CanBo.TrangThai == false)
                {
                    TempData["message"] = "Tài khoản của bạn đã bị quản trị viên khóa.";
                    return RedirectToAction("login");
                }
                else
                {

                    string cookieclient = tbl_CanBo.MaCB.Trim();
                    string decodecookieclient = CryptorEngine.Encrypt(cookieclient, true);
                    HttpCookie tbl_CanBocookie = new HttpCookie("canbo")
                    {
                        Value = decodecookieclient,
                        Expires = DateTime.Now.AddDays(5)
                    };
                    HttpContext.Response.Cookies.Add(tbl_CanBocookie);

                    return Redirect("/");

                }
            }
            else
            {
                TempData["message"] = "Tài khoản hoặc mật khẩu không đúng.";
                return RedirectToAction("login");
            }
        }

        public ActionResult Logout()
        {
            var myCookie = new HttpCookie("canbo")
            {
                Expires = DateTime.Now.AddDays(-1d)
            };
            Response.Cookies.Add(myCookie);

            return RedirectToAction("login");
        }

        public ActionResult ChangePass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmChangePass(string Old_Pass, string New_Pass)
        {
            var tbl_CanBo = DanhGiaCanBo.Models.CookiesManage.GetUser();
            if (tbl_CanBo.MatKhau.Trim() == Old_Pass)
            {
                var tbl_CanBos = db.tbl_CanBo.Find(tbl_CanBo.ID);
                tbl_CanBos.MatKhau = New_Pass;
                db.SaveChanges();

                var myCookie = new HttpCookie("canbo")
                {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                Response.Cookies.Add(myCookie);
                TempData["message"] = "Đổi mật khẩu thành công. Bạn vui lòng đăng nhập lại.";
                return RedirectToAction("login");
            }
            else
            {
                TempData["message"] = "Mật khẩu cũ không đúng. Vui lòng nhập lại";
                return RedirectToAction("changePass");
            }


        }
    }
}