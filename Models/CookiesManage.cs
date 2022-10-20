using DanhGiaCanBo.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DanhGiaCanBo.Models
{
    public class CookiesManage
    {
        private static DanhGiaCBEntities db = new DanhGiaCBEntities();
        public static bool Logined()
        {
            var cookiesClient = HttpContext.Current.Request.Cookies.Get("canbo");
            if (cookiesClient != null)
            {
                var decodeCookie = CryptorEngine.Decrypt(cookiesClient.Value, true);
                var us = db.tbl_CanBo.FirstOrDefault(x => x.MaCB == decodeCookie);
                if (us != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static tbl_CanBo GetUser()
        {
            var cookiesClient = HttpContext.Current.Request.Cookies.Get("canbo");
            if (cookiesClient != null)
            {
                var decodeCookie = CryptorEngine.Decrypt(cookiesClient.Value, true);

                var us = db.tbl_CanBo.FirstOrDefault(x => x.MaCB == decodeCookie);
                if (us != null)
                {
                    return us;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

       
    }
}