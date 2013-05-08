using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

namespace www
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            //WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserEmail", autoCreateTables: true);
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfiles", "UserId", "UserEmail", autoCreateTables: false);
        }

        protected void Session_Start()
        {
            // SET crmContext cookie value to a default value
            if (Request.Cookies["MyUserSettings"] == null)
            {
                HttpCookie contextCookie = new HttpCookie("MyUserSettings");
                contextCookie.Values["crmContextValue"] = "";
                contextCookie.Values["companyName"] = "";
                contextCookie.Expires = DateTime.Now.AddDays(5d);
                Response.Cookies.Add(contextCookie);
            }
        }
    }
}