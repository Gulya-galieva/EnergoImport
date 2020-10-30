using EnergoImport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnergoImport.Controllers
{
    public class SearchController : Controller
    {
        private RegPointsContext db = new RegPointsContext();
        private void GetAccessLevel()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user != null)
            {
                ViewData["AccessImport"] = user.AccessImport;
                ViewData["EditStatusInEnergo"] = user.EditStatusInEnergo;
                ViewData["EditStatusLinkIsOk"] = user.EditStatusLinkIsOk;
                ViewData["EditStatusAdded"] = user.EditStatusAdded;
                ViewData["AccessComments"] = user.AccessComments;
                ViewData["AccessDbUGES"] = user.AccessDbUGES;
                ViewData["AccessDbCES"] = user.AccessDbCES;
                ViewData["EditESubStatus"] = user.EditESubStatus;
                ViewData["DeleteRegPoint"] = user.DeleteRegPoint;
                //Добавить разрешения
            }
            else
            {
                ViewData["AccessImport"] = false;
                ViewData["EditStatusInEnergo"] = false;
                ViewData["EditStatusLinkIsOk"] = false;
                ViewData["EditStatusAdded"] = false;
                ViewData["AccessComments"] = false;
                ViewData["AccessDbUGES"] = false;
                ViewData["AccessDbCES"] = false;
                ViewData["EditESubStatus"] = false;
                ViewData["DeleteRegPoint"] = false;
                //Добавить разрешения
            }
        }
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string searchText)
        {
            if(searchText.ToLower().Contains("тп") || searchText.ToLower().Contains("рп"))
            {
                return RedirectToAction("ESubSearch", new { searchText });
            }
            else
            {
                return RedirectToAction("PointsSearch", new { searchText });
            }
            //return View();
        }

        [Authorize]
        public ActionResult ESubSearch(string searchText)
        {
            string SearchText = searchText == null ? "" : searchText;
            ViewBag.SearchText = searchText == null ? "" : searchText;
            string lowerText = searchText == null ? "" : searchText.ToLower();

            List<ESubstationViewModel> eSubList = new List<ESubstationViewModel>();

            var eSubs = (from eSub in db.ESubstations
                           where eSub.Name.ToLower().Contains(lowerText) && lowerText != ""
                           orderby eSub.NetRegionId
                           select eSub).ToList();
            if (eSubs != null)
            {
                eSubs.ForEach(eSub =>
                {
                    Models.Action lastAction = eSub.Actions.OrderByDescending(a => a.Time).FirstOrDefault();
                    ESubstationViewModel m = new ESubstationViewModel()
                    {
                        ESubstation = eSub
                    };
                    if (lastAction != null) m.LastAction = lastAction.Time.ToString("dd MMMM в HH:mm");
                    eSubList.Add(m);
                });
            }
            //Сортировка
            eSubList = (from vm in eSubList
                        orderby vm.RequestsToAddCount descending, vm.LastAction descending
                        select vm).ToList();
            ViewBag.ESubList = eSubList;

            return View();
        }

        [Authorize]
        public ActionResult PointsSearch(string searchText)
        {
            GetAccessLevel();
            ViewBag.SearchText = searchText;
            var points = (from p in db.RegPoints
                            where p.Serial.Contains(searchText) && searchText != ""
                            orderby p.ESubstation.Name
                            select p).ToList();
            List<PointsViewModel> tmp = new List<PointsViewModel>();
            Dictionary<string, List<PointsViewModel>> eSubsPoints = new Dictionary<string, List<PointsViewModel>>();
            //List<List<PointsViewModel>> eSubsPoints = new List<List<PointsViewModel>>();
            string eSubName = "";
            int pointsCounter = 0;
            points.ForEach(p =>
            {
                if(eSubName != p.ESubstation.Name)
                {
                    eSubName = p.ESubstation.Name;
                    tmp = new List<PointsViewModel>();
                    eSubsPoints.Add(eSubName, tmp);
                }
                tmp.Add(new PointsViewModel(p));
                pointsCounter++;
            });
            
            ViewBag.ESubsPoints = eSubsPoints;
            ViewBag.PointsCount = pointsCounter;
            return View();
        }
    }
}