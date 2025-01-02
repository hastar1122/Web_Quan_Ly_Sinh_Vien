using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_Quan_Ly_Sinh_Vien.Areas.GiangVien.Controllers
{
    [Authorize(Roles = "Giảng Viên")]
    public class ChatController : Controller
    {
        // GET: GiangVien/Chat
        public ActionResult Index()
        {
            return View();
        }
    }
}