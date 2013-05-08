using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using www.Models;
using System.Web;

namespace www.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DatabaseContext db = new DatabaseContext();

        // GET: /Account/
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "The password has been changed."
                : message == ManageMessageId.ChangePasswordFailed ? "The current password is incorrect or the new password is invalid."
                : "";
            ViewBag.CurrentUserId = db.UserProfiles.Where(u => u.UserEmail == User.Identity.Name).First().UserId;
            ViewBag.FirstUserId = db.UserProfiles.First().UserId;
            return View(db.UserProfiles.ToList());
        }


        // GET: /Account/Details/5
        public ActionResult Details(int id = 0)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }


        // GET: /Account/Edit/5
        public ActionResult Edit(int id = 0)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }

        
        // POST: /User/Edit/5
        [HttpPost]
        public ActionResult Edit(UserProfile userprofile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userprofile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userprofile);
        }


        // GET: /Account/Delete/5
        public ActionResult Delete(int id = 0)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }

        
        // POST: /Account/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(userprofile.UserEmail);
            ((SimpleMembershipProvider)Membership.Provider).DeleteUser(userprofile.UserEmail, true);

            return RedirectToAction("Index");
        }


        // GET: /Account/ChangePassword/5
        public ActionResult ChangePassword(int id = 0)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = userprofile.UserId.ToString();
            ViewBag.UserEmail = userprofile.UserEmail.ToString();
            return View();
        }


        // POST: /Account/ChangePassword/5
        [HttpPost]
        public ActionResult ChangePassword(LocalPasswordModel model, int userId)
        {
            if (ModelState.IsValid)
            {
                // Get the UserId for the requested account
                UserProfile userProfile = db.UserProfiles.Find(userId);

                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(userProfile.UserEmail, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordFailed });
        }


        // GET: /Account/SetPermissions/5
        [Authorize(Roles = "CanManagePermissions")]
        public ActionResult SetPermissions(int id = 0)
        {
            // Retrieve the users information
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }

            // Retrieve the complete list of Roles
            string[] roles = Roles.GetAllRoles();

            SetPermissionViewModel model = new SetPermissionViewModel { User = userprofile, Roles = roles };

            return View(model);
        }


        // POST: /Account/SetPermissions/5
        [HttpPost]
        [Authorize(Roles = "CanManagePermissions")]
        public ActionResult SetPermissions(FormCollection permissions)
        {
            // Retrieve the users information
            UserProfile userprofile = db.UserProfiles.Find(Convert.ToInt32(permissions["userId"]));
            if (userprofile == null)
            {
                return HttpNotFound();
            }

            // Retrieve the complete list of Roles
            string[] roles = Roles.GetAllRoles();

            // Verify is the user has permissions and assign accordingly
            foreach (string role in roles)
            {
                //Add user to role if checkbox selected and user not already in role
                if (permissions[role].Contains("true") && !Roles.IsUserInRole(userprofile.UserEmail, role))
                {
                    Roles.AddUserToRole(userprofile.UserEmail, role);
                }
                
                //Remove user from role if checkbox not selected and user already in role
                if (!permissions[role].Contains("true") && Roles.IsUserInRole(userprofile.UserEmail, role))
                {
                    Roles.RemoveUserFromRole(userprofile.UserEmail, role);
                }
            }

            return RedirectToAction("Index");
        }



        
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.Cookies["MyUserSettings"] == null)
            {
                HttpCookie contextCookie = new HttpCookie("MyUserSettings");
                contextCookie.Values["crmContextValue"] = "";
                contextCookie.Values["companyName"] = "";
                contextCookie.Expires = DateTime.Now.AddDays(5d);
                Response.Cookies.Add(contextCookie);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserEmail, model.Password, persistCookie: model.RememberMe))
            {
                //return RedirectToLocal(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            // Delete users cookies
            if (Request.Cookies["MyUserSettings"] != null)
            {
                HttpCookie contextCookie = new HttpCookie("MyUserSettings");
                contextCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(contextCookie);
            }

            return RedirectToAction("Index", "Home");
        }

        
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserEmail, model.Password, new { UserFirstName = model.UserFirstName, UserLastName = model.UserLastName });
                    //WebSecurity.Login(model.UserEmail, model.Password);
                    return RedirectToAction("Index");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region Helpers

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            ChangePasswordFailed,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
