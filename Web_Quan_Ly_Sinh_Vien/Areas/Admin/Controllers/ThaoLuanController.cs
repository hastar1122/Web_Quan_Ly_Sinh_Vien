using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Areas.Admin.Controllers
{
    public class ThaoLuanController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "LopHocPhan");
            }
            ViewBag.MaLHP = id;
            return View();
        }
    }
}