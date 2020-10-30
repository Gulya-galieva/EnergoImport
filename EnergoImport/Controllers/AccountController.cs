using EnergoImport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EnergoImport.Controllers
{
    public class AccountController : Controller
    {
        private RegPointsContext db = new RegPointsContext();
        private void GetAccessLevel()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            
            ViewData["EditUsers"] = user.EditUsers;
            ViewData["AccessImport"] = user.AccessImport;
            ViewData["EditStatusInEnergo"] = user.EditStatusInEnergo;
            ViewData["EditStatusLinkIsOk"] = user.EditStatusLinkIsOk;
            ViewData["EditStatusAdded"] = user.EditStatusAdded;
            ViewData["AccessComments"] = user.AccessComments;
            ViewData["AccessDbUGES"] = user.AccessDbUGES;
            ViewData["AccessDbCES"] = user.AccessDbCES;
            user = null;
            //Добавить разрешения
        }

        // GET: Account
        public ActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User userData)
        {
            if(ModelState.IsValid)
            {
                User userdb = null;
                using (RegPointsContext db = new RegPointsContext())
                {
                    userdb = db.Users.FirstOrDefault(u => u.Login == userData.Login && u.Pass == userData.Pass);
                }
                if(userdb != null)
                {
                    FormsAuthentication.SetAuthCookie(userData.Login, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет.");
                }
            }
            return View(userData);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/Home/Index/");
        }

        [Authorize]
        public ActionResult PersonalArea()
        {
            GetAccessLevel();
            User usr = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            ViewBag.UserName = usr.Name;
            return View();
        }
    }
}