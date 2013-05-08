using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using www.Models;
using www.Models.ViewModels;
using System.Web.Security;

namespace www.Controllers
{
    public class CompanyController : Controller
    {
        private readonly DatabaseContext db = new DatabaseContext();

        // GET: /Company/
        public ActionResult Index()
        {
            return View(db.Companies.ToList());
        }

        
        // GET: /Company/Details/5
        public ActionResult Details(int id = 0)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        
        // GET: /Company/Create
        public ActionResult Create()
        {
            return View();
        }

        
        // POST: /Company/Create
        [HttpPost]
        public ActionResult Create(CompanyView company)
        {
            if (ModelState.IsValid)
            {
                string fileName = "logo_" + company.CompanyName.Replace(" ", "").ToLower();
                // Verify that the user selected a file
                if (company.CompanyLogo != null && company.CompanyLogo.ContentLength > 0)
                {
                    // extract only the file extension
                    fileName = fileName + Path.GetExtension(company.CompanyLogo.FileName);
                    // store the file inside ~/Content/images/CompanyLogos folder
                    var path = Path.Combine(Server.MapPath("~/Content/images/CompanyLogos"), fileName);
                    company.CompanyLogo.SaveAs(path);
                }

                var newCompany = new Company { CompanyName = company.CompanyName, CompanyLogo = fileName};

                db.Companies.Add(newCompany);
                db.SaveChanges();

                // Add new Authorization Roles for this New Company
                Roles.CreateRole(company.CompanyName);
                Roles.CreateRole(company.CompanyName + "_CanViewCrmReports");
                Roles.CreateRole(company.CompanyName + "_CanViewContestReports");

                return RedirectToAction("Index");
            }

            return View(company);
        }

        
        // GET: /Company/Edit/5
        public ActionResult Edit(int id = 0)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
                return HttpNotFound();

            CompanyView companyView = new CompanyView {CompanyId = company.CompanyId, CompanyName = company.CompanyName};
            ViewBag.logoFileName = company.CompanyLogo;
            return View(companyView);
        }

        
        // POST: /Company/Edit/5
        [HttpPost]
        public ActionResult Edit(CompanyView company)
        {
            if (ModelState.IsValid)
            {
                string fileName = "logo_" + company.CompanyName.Replace(" ", "").ToLower();
                // Verify that the user selected a file
                if (company.CompanyLogo != null && company.CompanyLogo.ContentLength > 0)
                {
                    // extract only the file extension
                    fileName = fileName + Path.GetExtension(company.CompanyLogo.FileName);
                    // store the file inside ~/Content/images/CompanyLogos folder
                    var path = Path.Combine(Server.MapPath("~/Content/images/CompanyLogos"), fileName);
                    company.CompanyLogo.SaveAs(path);
                }

                var updatedCompany = new Company { CompanyId = company.CompanyId, CompanyName = company.CompanyName, CompanyLogo = fileName };

                db.Entry(updatedCompany).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        
        // GET: /Company/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        
        // POST: /Company/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            
            // Delete all user permissions in associated company roles
            if (Roles.GetUsersInRole(company.CompanyName).Count() > 0)
                Roles.RemoveUsersFromRole(Roles.GetUsersInRole(company.CompanyName), company.CompanyName);
            if (Roles.GetUsersInRole(company.CompanyName + "_CanViewCrmReports").Count() > 0)
                Roles.RemoveUsersFromRole(Roles.GetUsersInRole(company.CompanyName + "_CanViewCrmReports"), company.CompanyName + "_CanViewCrmReports");
            if (Roles.GetUsersInRole(company.CompanyName + "_CanViewContestReports").Count() > 0)
                Roles.RemoveUsersFromRole(Roles.GetUsersInRole(company.CompanyName + "_CanViewContestReports"), company.CompanyName + "_CanViewContestReports");

            // Delete associated Roles from the company
            Roles.DeleteRole(company.CompanyName);
            Roles.DeleteRole(company.CompanyName + "_CanViewCrmReports");
            Roles.DeleteRole(company.CompanyName + "_CanViewContestReports");

            // Delete company
            db.Companies.Remove(company);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}