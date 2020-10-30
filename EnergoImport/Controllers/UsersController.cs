using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnergoImport.Models;

namespace EnergoImport.Controllers
{
    public class UsersController : Controller
    {
        private RegPointsContext db = new RegPointsContext();

        bool IsHaveAccess()
        {
            User us = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (us == null) return false;
            return us.EditUsers;
        }

        // GET: Users
        public ActionResult Index()
        {
            if(!IsHaveAccess()) return RedirectToAction("Index", "Home");
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (!IsHaveAccess()) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            if (!IsHaveAccess()) return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Login,Pass,AccessDbUGES,AccessDbCES,AccessImport,EditStatusInEnergo,EditStatusLinkIsOk,EditStatusAdded,AccessComments")] User user)
        {
            if (!IsHaveAccess()) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!IsHaveAccess()) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Login,Pass,AccessDbUGES,AccessDbCES,AccessImport,EditStatusInEnergo,EditStatusLinkIsOk,EditStatusAdded,AccessComments")] User user)
        {
            if (!IsHaveAccess()) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!IsHaveAccess()) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsHaveAccess()) return RedirectToAction("Index", "Home");
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
