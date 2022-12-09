using System.Web.Mvc;

namespace BookingCinema.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //trang login
            //context.MapRoute(
            //    "AuthLogin",
            //    "Admin/Login",
            //    new { Controller = "Auth", action = "Login", id = UrlParameter.Optional }
            //);
            //trang chu admin
            context.MapRoute(
                "Index",
                "Admin",
                new { Controller = "Admin", action = "Index", id = UrlParameter.Optional },
                new[] { "BookingCinema.Areas.Admin.Controllers" }
            );
            /* Login*/
            context.MapRoute(
                "AuthLogin",
                "Admin/Login",
                new { Controller = "Auth", action = "Login", id = UrlParameter.Optional }
            );


            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "BookingCinema.Areas.Admin.Controllers" }
            );
        }
    }
}