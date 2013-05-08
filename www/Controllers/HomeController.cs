using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using www.Models;

namespace www.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext db = new DatabaseContext();

        // GET: /Home/
        [Authorize]
        public ActionResult Index()
        {
            List<Company> permittedCompanies =  new List<Company>();
            foreach (Company company in db.Companies)
            {
                if (Roles.IsUserInRole(company.CompanyName))
                {
                    permittedCompanies.Add(company);
                }
            }


            // Send user directly to reports if they are only permitted to view one company
            if (permittedCompanies.Count == 1)
            {
                string dbName = permittedCompanies.First().CompanyName + "Connection";

                if (Request.Cookies["MyUserSettings"] != null)
                {
                    HttpCookie contextCookie = new HttpCookie("MyUserSettings");
                    contextCookie.Values["crmContextValue"] = dbName;
                    contextCookie.Values["companyName"] = permittedCompanies.First().CompanyName;
                    contextCookie.Expires = DateTime.Now.AddDays(1d);
                    Response.Cookies.Add(contextCookie);
                }
                return RedirectToAction("Index", "Report");
            }

            return View(permittedCompanies);
        }


        // POST: /Home/
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            string dbName = form["item.CompanyName"] + "Connection";

            if (Request.Cookies["MyUserSettings"] != null)
            {
                HttpCookie contextCookie = new HttpCookie("MyUserSettings");
                contextCookie.Values["crmContextValue"] = dbName;
                contextCookie.Values["companyName"] = form["item.CompanyName"];
                contextCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(contextCookie);
            }

            return RedirectToAction("Index", "Report");
        }


        // GET: /Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";            
                
            return View();
        }


        // GET: /Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        // GET: /Home/CompanyDropDown
        [ChildActionOnly]
        public ActionResult CompanyDropdown()
        {
            List<SelectListItem> permittedCompanies = new List<SelectListItem>();
            permittedCompanies.Add(new SelectListItem { Text = "Select...", Value = "" });
            
            IEnumerable<Company> companies = db.Companies.OrderBy(o => o.CompanyName);
            foreach (Company company in companies)
            {
                if (Roles.IsUserInRole(company.CompanyName))
                {
                    permittedCompanies.Add(new SelectListItem
                    {
                        Text = company.CompanyName,
                        Value = company.CompanyName,
                        Selected = (company.CompanyName == Request.Cookies["MyUserSettings"]["companyName"].ToString() ? true : false)
                    });
                }
            }

            ViewBag.Companies = permittedCompanies;
            ViewBag.NumberOfCompanies = permittedCompanies.Count();

            return PartialView();
        }


        // POST: /Home/CompanyDropDown
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyDropdown(FormCollection form)
        {
            string dbName = form["Companies"] + "Connection";

            if (Request.Cookies["MyUserSettings"] != null)
            {
                HttpCookie contextCookie = new HttpCookie("MyUserSettings");
                contextCookie.Values["crmContextValue"] = dbName;
                contextCookie.Values["companyName"] = form["Companies"];
                contextCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(contextCookie);
            }

            return RedirectToAction(form["Action"], form["Controller"]);
        }
    }
}
