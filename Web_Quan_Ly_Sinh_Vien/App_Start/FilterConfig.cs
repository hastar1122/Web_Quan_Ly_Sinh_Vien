using System.Web;
using System.Web.Mvc;

namespace Web_Quan_Ly_Sinh_Vien
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
