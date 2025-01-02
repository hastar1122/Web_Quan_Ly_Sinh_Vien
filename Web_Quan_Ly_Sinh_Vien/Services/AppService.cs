using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.Data;
using Web_Quan_Ly_Sinh_Vien.Services;

namespace Web_Quan_Ly_Sinh_Vien.Services
{
    public class AppService
    {
        Web_Quan_Ly_Sinh_VienEntities db;

        public AppService()
        {
            db = new Web_Quan_Ly_Sinh_VienEntities();
        }

        public tblUser Login(LoginData loginData)
        {
            string matKhau = Functions.Encrypt(loginData.MatKhau);
            var currentUser = db.tblUsers.FirstOrDefault(m => m.Email == loginData.Email && m.MatKhau == matKhau && m.TrangThai == true);
            if (currentUser != null)
            {
                return currentUser;
            }
            else
            {
                return null;
            }
        }
    }
}