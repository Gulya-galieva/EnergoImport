using EnergoImport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnergoImport.Controllers
{
    public class RegPointController : Controller
    {
        private readonly RegPointsContext db;
        public RegPointController(RegPointsContext context)
        {
            db = context;
        }
        public RegPointController()
        {
            db = new RegPointsContext();
        }

        [HttpPost]
        //[Authorize]
        public void Delete(int id)
        {
            var p = db.RegPoints.Find(id);
            if (p != null)
            {
                db.RegPoints.Remove(p);
                db.SaveChanges();
            }
        }
    }
}