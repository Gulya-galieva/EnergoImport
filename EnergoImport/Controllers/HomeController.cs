using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EnergoImport.Models;

namespace EnergoImport.Controllers
{
    public class HomeController : Controller
    {
        RegPointsContext db = new RegPointsContext();

        private void GetAccessLevel()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if(user != null)
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
                //Добавить разрешения
            }
        }
        private User GetUser()
        {
            return db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
        }

        public ActionResult About()
        {
            return View();
        }

        [Authorize]
        public ActionResult Index()
        {
            GetAccessLevel();

            List<PieceOfPieChartModel> pieDataListCES = new List<PieceOfPieChartModel>();
            pieDataListCES.Add(new PieceOfPieChartModel() { Name = "Принято в ЭС", Value = db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО ЦЭС" && p.AcceptedInEnergo) });
            pieDataListCES.Add(new PieceOfPieChartModel() { Name = "Добавили", Value = db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО ЦЭС" && p.AddedInEnergo && !p.AcceptedInEnergo) });
            pieDataListCES.Add(new PieceOfPieChartModel() { Name = "Заявки", Value = db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО ЦЭС" && p.LinkIsOk && !p.AddedInEnergo && !p.AcceptedInEnergo) });
            pieDataListCES.Add(new PieceOfPieChartModel() { Name = "Не обработано", Value = db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО ЦЭС" && !p.LinkIsOk && !p.AddedInEnergo && !p.AcceptedInEnergo) });
            pieDataListCES.Add(new PieceOfPieChartModel() { Name = "Осталось выполнить", Value = 14281 - db.RegPoints.Count(p => p.ESubstation.NetRegion.ContractId == 1) });
            ViewBag.PieDataListCES = pieDataListCES;

            List<PieceOfPieChartModel> pieDataListUGES = new List<PieceOfPieChartModel>();
            pieDataListUGES.Add(new PieceOfPieChartModel() { Name = "Принято в ЭС", Value = db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО УГЭС" && p.AcceptedInEnergo) });
            pieDataListUGES.Add(new PieceOfPieChartModel() { Name = "Добавили", Value = db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО УГЭС" && p.AddedInEnergo && !p.AcceptedInEnergo) });
            pieDataListUGES.Add(new PieceOfPieChartModel() { Name = "Заявки", Value = db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО УГЭС" && p.LinkIsOk && !p.AddedInEnergo && !p.AcceptedInEnergo) });
            pieDataListUGES.Add(new PieceOfPieChartModel() { Name = "Не обработано", Value = db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО УГЭС" && !p.LinkIsOk && !p.AddedInEnergo && !p.AcceptedInEnergo) });
            pieDataListUGES.Add(new PieceOfPieChartModel() { Name = "Осталось выполнить", Value = 26950 - db.RegPoints.Count(p => p.ESubstation.NetRegion.ContractId == 2) });
            ViewBag.PieDataListUGES = pieDataListUGES;

            return View();
        }

        [Authorize]
        public ActionResult NetRegions(int Id)
        {
            GetAccessLevel();
            Contract contract = db.Contracts.Find(Id);
            if (contract == null) return RedirectToAction("Index");
            ViewBag.Message = "Сети";
            ViewBag.ContractName = contract.Name;
            if(contract != null)
            {
                ViewBag.RegionsList = contract.NetRegions;
                int eSubCount = 0;
                int inDbSumm = 0;
                int requestToAddSumm = 0;
                int inEnergoSumm = 0;
                int readyForSmetaSumm = 0;
                foreach (var region in contract.NetRegions)
                {
                    /*Добавлено 30.10.2020 Гуля*/
                    int eSub = db.ESubstations.Where(p => p.NetRegionId == region.Id && p.RegPoints.Count != 0).Count();
                    eSubCount += eSub;
                    //Передадим количество добавленных точек
                    ViewData[region.Name + "allSub"] = eSub;

                    int inDb = db.RegPoints.Where(p => p.ESubstation.NetRegionId == region.Id).Count();
                    inDbSumm += inDb;
                    //Передадим количество добавленных точек
                    ViewData[region.Name + "allPoints"] = inDb;

                    int requestToAdd = db.RegPoints.Count(p => p.ESubstation.NetRegionId == region.Id && p.LinkIsOk && !p.AcceptedInEnergo && !p.AddedInEnergo);
                    requestToAddSumm += requestToAdd;
                    //Передадим количество опрошеных точек
                    ViewData[region.Name + "requestToAdd"] = requestToAdd;

                    int readyForSmeta = db.RegPoints.Count(p => p.ESubstation.NetRegionId == region.Id && p.AcceptedInEnergo && !p.InSmeta);
                    readyForSmetaSumm += readyForSmeta;
                    //Передадим количество точек которые можно внести в сметы
                    ViewData[region.Name + "readyForSmeta"] = readyForSmeta;


                    int inEnergo = db.RegPoints.Where(p => p.ESubstation.NetRegionId == region.Id && p.AcceptedInEnergo).Count();
                    inEnergoSumm += inEnergo;
                    //Передадим количество добавленных в Энергосферу точек
                    ViewData[region.Name + "inEnergoCount"] = inEnergo;

                    Models.Action lastAction = db.Actions.Where(a => a.ESubstation.NetRegionId == region.Id).OrderByDescending(a => a.Time).FirstOrDefault();
                    ViewData[region.Name + "lastUpdate"] = (lastAction == null)? "-" : lastAction.Time.ToString("dd MMMM в HH:mm"); //"dd MMMM, yyyy в HH:mm" с годом
                }
                ViewBag.eSubCount = eSubCount;
                ViewBag.InDbSumm = inDbSumm;
                ViewBag.RequestsToAddSumm = requestToAddSumm;
                ViewBag.InEnergoSumm = inEnergoSumm;
                ViewBag.ReadyForSmetaSumm = readyForSmetaSumm;
                if (contract.Name == "ПО ЦЭС")
                {
                    ViewBag.ContractCount = 14281;
                    ViewBag.SMR = 14281 - db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО ЦЭС");
                }
                if (contract.Name == "ПО УГЭС")
                {
                    ViewBag.ContractCount = 26950;
                    ViewBag.SMR = 26950 - db.RegPoints.Count(p => p.ESubstation.NetRegion.Contract.Name == "ПО УГЭС");
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult ESubstations(int Id)
        {
            GetAccessLevel();
            List<ESubstationViewModel> eSubList = new List<ESubstationViewModel>();
            var netRegion = db.NetRegions.Find(Id);
            if (netRegion != null)
            {
                ViewBag.NetRegionName = netRegion.Name;
                ViewBag.NetRegionId = netRegion.Id;
                ViewBag.NetRegionFullName = netRegion.LongName;
                ViewBag.SustationsList = netRegion.ESubstatons;
                ViewBag.ContractName = netRegion.Contract.Name;
                ViewBag.ContractId = netRegion.Contract.Id;

                netRegion.ESubstatons.ForEach(eSub =>
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
        [HttpPost]
        public string EditESubStatus(int Id)
        {
            int newStatusId = Convert.ToInt32(Request.Form["newStatusId"]);
            string statusText = db.StatusesESub.Find(newStatusId).Text;
            ESubstation eSub = db.ESubstations.Find(Id);
            if(statusText != null && eSub != null)
            {
                eSub.StatusESubId = newStatusId;
                eSub.Actions.Add(new Models.Action()
                {
                    ActionType = ActionType.ChangeESubStatus,
                    Comment = statusText,
                    Time = DateTime.Now,
                    UserId = GetUser().Id
                });
                db.SaveChanges();
            }
            
            return eSub.StatusESub.Text;
        }


        [Authorize]
        [HttpGet]
        public ActionResult Points(int Id)
        {
            GetAccessLevel();
            List<PointsViewModel> viewModel = new List<PointsViewModel>();
            var eSubstation = db.ESubstations.Find(Id);
            if(eSubstation != null)
            {
                //В очереди для опроса или нет
                ViewBag.PollRequestBtnClass = eSubstation.PollRequest ? "btn-success" : "btn-default";
                ViewBag.PollRequestBtnText = eSubstation.PollRequest ? "Убрать из очереди опроса" : "Добавить в очередь опроса";
                //Статус ТП/РП
                SelectList statuses = new SelectList(db.StatusesESub, "Id", "Text", eSubstation.StatusESubId);
                ViewBag.Statuses = statuses;
                ViewBag.CurrentStatus = eSubstation.StatusESub.Text;
                //Общее
                ViewBag.ContractName = eSubstation.NetRegion.Contract.Name;
                ViewBag.ContractId = eSubstation.NetRegion.Contract.Id;
                ViewBag.RegionName = eSubstation.NetRegion.Name;
                ViewBag.RegionId = eSubstation.NetRegion.Id;
                ViewBag.SubstationName = eSubstation.Name;
                ViewBag.ESubId = eSubstation.Id;
                //Комментарии
                ViewBag.Comments = eSubstation.Comments.OrderByDescending(c => c.Time).ToList();
                ViewBag.UserId = GetUser().Id;
                //
                Models.Action lastAction = eSubstation.Actions.OrderByDescending(a => a.Time).FirstOrDefault();
                if (lastAction != null)
                    ViewBag.LastActionTime = lastAction.Time.ToString("dd MMMM в HH:mm");
                else
                    ViewBag.LastActionTime = "-";

                //Загрузим список точек
                //eSubstation.RegPoints.ForEach(p => { viewModel.Add(new PointsViewModel(p)); });
                //Сортируем
                //viewModel = (from vm in viewModel
                //             orderby vm.Point.Serial
                //             select vm).ToList();
                viewModel = (from rp in eSubstation.RegPoints
                             let dublicates = db.RegPoints.Count(p => p.OldId == rp.OldId && p.ESubstation.NetRegion.ContractId == rp.ESubstation.NetRegion.ContractId)
                             select new PointsViewModel(rp)
                             {
                                 Dublicates = dublicates
                             }).ToList();

                //Статистика ЦЭС
                //Всего
                ViewBag.CountPLC1 = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("1ф"));
                ViewBag.CountPLC3 = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("3ф"));
                ViewBag.CountGSM = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("GSM"));
                ViewBag.CountINPUT = viewModel.Count() - (int)ViewBag.CountPLC1 - (int)ViewBag.CountPLC3 - (int)ViewBag.CountGSM;
                //Принято в ЭС
                ViewBag.CountPLC1InEnergo = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("1ф") && pvm.Point.AcceptedInEnergo);
                ViewBag.CountPLC3InEnergo = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("3ф") && pvm.Point.AcceptedInEnergo);
                ViewBag.CountGSMInEnergo = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("GSM") && pvm.Point.AcceptedInEnergo);
                ViewBag.CountINPUTInEnergo = viewModel.Count(pvm => pvm.Point.AcceptedInEnergo) - (int)ViewBag.CountPLC1InEnergo - (int)ViewBag.CountPLC3InEnergo - (int)ViewBag.CountGSMInEnergo;
                //Запрос на добавление
                ViewBag.CountPLC1Request = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("1ф") && pvm.Point.LinkIsOk && !pvm.Point.AcceptedInEnergo);
                ViewBag.CountPLC3Request = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("3ф") && pvm.Point.LinkIsOk && !pvm.Point.AcceptedInEnergo);
                ViewBag.CountGSMRequest = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("GSM") && pvm.Point.LinkIsOk && !pvm.Point.AcceptedInEnergo);
                ViewBag.CountINPUTRequest = viewModel.Count(pvm => pvm.Point.LinkIsOk && !pvm.Point.AcceptedInEnergo) - (int)ViewBag.CountPLC1Request - (int)ViewBag.CountPLC3Request - (int)ViewBag.CountGSMRequest;
                //В Смете
                ViewBag.CountPLC1Smeta = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("1ф") && pvm.Point.InSmeta);
                ViewBag.CountPLC3Smeta = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("3ф") && pvm.Point.InSmeta);
                ViewBag.CountGSMSmeta = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("GSM") && pvm.Point.InSmeta);
                ViewBag.CountINPUTSmeta = viewModel.Count(pvm => pvm.Point.InSmeta) - (int)ViewBag.CountPLC1Smeta - (int)ViewBag.CountPLC3Smeta - (int)ViewBag.CountGSMSmeta;
                //
                ViewBag.CountPLC1Added = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("1ф") && pvm.Point.AddedInEnergo);
                ViewBag.CountPLC3Added = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("PLC") && pvm.Point.TypeLink.Contains("3ф") && pvm.Point.AddedInEnergo);
                ViewBag.CountGSMAdded = viewModel.Count(pvm => pvm.Point.TypeLink.Contains("GSM") && pvm.Point.AddedInEnergo);
                ViewBag.CountINPUTAdded = viewModel.Count(pvm => pvm.Point.AddedInEnergo) - (int)ViewBag.CountPLC1Added - (int)ViewBag.CountPLC3Added - (int)ViewBag.CountGSMAdded;

                int allCount = viewModel.Count();
                int okCount = viewModel.Count(pvm => pvm.Point.AcceptedInEnergo);
                ViewBag.RegPointsCount = allCount;
                ViewBag.RegPointsOk = okCount;
                ViewBag.RegPointsProgress = (allCount > 0)? (okCount*100 / allCount).ToString() : "0";
            }
            else
            {
                ViewBag.ContractName = " ";
                ViewBag.RegionName = " ";
                ViewBag.SubstationName = "Объект не найден";
                ViewBag.RegPoinsList = new List<RegPoint>();
                ViewBag.RegPoinsCount = 1;
                ViewBag.RegPoinsOk = 1;
                ViewBag.RegPoinsProgress = "100";
            }
            ViewBag.PointsList = viewModel;
            return View();
        }

        [HttpPost]
        public ActionResult EditPointStatus(CheckButtonModel model)
        {
            var rp = db.RegPoints.Find(model.Id);
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (rp != null && user != null)
            {
                model.Status = !model.Status;
                if (model.Data == CheckButtonModel.DataType.AcceptedInEnergo)
                {
                    ActionType acType = model.Status ? ActionType.CheckedInEnergosphera : ActionType.UncheckedInEnergosphera;
                    rp.Actions.Add(new Models.Action() { ActionType = acType, ESubstationId = rp.ESubstation.Id, RegPointId = rp.Id, Time = DateTime.Now, UserId = user.Id });
                    rp.AcceptedInEnergo = model.Status;
                }
                if (model.Data == CheckButtonModel.DataType.AddedInEnergo)
                {
                    ActionType acType = model.Status ? ActionType.CheckedAdd : ActionType.UncheckedAdd;
                    rp.Actions.Add(new Models.Action() { ActionType = acType, ESubstationId = rp.ESubstation.Id, RegPointId = rp.Id, Time = DateTime.Now, UserId = user.Id });
                    rp.AddedInEnergo = model.Status;
                }
                if (model.Data == CheckButtonModel.DataType.LinkIsOk)
                {
                    ActionType acType = model.Status ? ActionType.CheckedLinkIsOk : ActionType.UncheckedLinkIsOk;
                    rp.Actions.Add(new Models.Action() { ActionType = acType, ESubstationId = rp.ESubstation.Id, RegPointId = rp.Id, Time = DateTime.Now, UserId = user.Id });
                    rp.LinkIsOk = model.Status;
                }
                db.SaveChanges();
            }
            return PartialView("_checkButtonAjax", model);
        }

        [Authorize]
        public ActionResult EditPollRequestStatus(int Id)
        {
            var eSub = db.ESubstations.Find(Id);
            var user = GetUser();
            if (eSub != null && user != null)
            {
                eSub.PollRequest = !eSub.PollRequest;
                ActionType acType = eSub.PollRequest ? ActionType.CheckedPollRequest : ActionType.UncheckedPollRequest;
                eSub.Actions.Add(new Models.Action() {
                    ActionType = acType,
                    Time = DateTime.Now,
                    UserId = user.Id
                });
                db.SaveChanges();
                return RedirectToAction("Points", "Home", new { eSub.Id });
            }
            return RedirectToAction("Points", "Home", new { Id = -1 });
        }

        [Authorize]
        public ActionResult SetAddedInEnergoAllPLC(int Id)
        {
            var eSub = db.ESubstations.Find(Id);
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if(eSub != null && user != null)
            {
                foreach (var p in eSub.RegPoints)
                {
                    if (p.TypeLink.Contains("PLC") && p.LinkIsOk && !p.AddedInEnergo)
                    {
                        p.AddedInEnergo = true;
                        p.Actions.Add(new Models.Action() { ActionType = ActionType.CheckedAdd, ESubstation = p.ESubstation, RegPointId = p.Id, Time = DateTime.Now, UserId = user.Id });
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Points", "Home", new { eSub.Id });
            }
            return RedirectToAction("Points", "Home", new { Id = -1 });
        }

        [Authorize]
        public ActionResult HistoryActions(int Id)
        {
            List<ActionViewModel> model = new List<ActionViewModel>();
            ESubstation eSub = db.ESubstations.Find(Id);
            if(eSub != null)
            {
                ViewBag.ESubName = eSub.Name;
                ViewBag.ESubId = eSub.Id;
                foreach (var a in eSub.Actions)
                {
                    model.Add(new ActionViewModel()
                    {
                        Action = a
                    });
                }
                //Сортировка
                model = model.OrderByDescending(a => a.Action.Time).ToList();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CommentAdd(Comment comment)
        {
            if(comment != null && comment.Text != null)
            if(comment.Text.Trim() != "")
            {
                db.Actions.Add(new Models.Action()
                    {
                        ActionType = ActionType.AddComment,
                        ESubstationId = comment.ESubstationId,
                        Time = DateTime.Now,
                        UserId = comment.UserId
                    });
                comment.Time = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                ViewBag.CommentsCount = db.ESubstations.FirstOrDefault(eSub => eSub.Id == comment.ESubstationId).Comments.Count();
            }
            return RedirectToAction("Points", new { Id = comment.ESubstationId });
        }
        [Authorize]
        public ActionResult CommentDelete(int Id)
        {
            Comment c = db.Comments.Find(Id);
            if (c == null) return RedirectToAction("Index");
            int eSubId = c.ESubstationId;
            if (GetUser().Id == c.UserId || GetUser().Id == 1)
            {
                db.Actions.Add(new Models.Action()
                {
                    ActionType = ActionType.DeleteComment,
                    ESubstationId = c.ESubstationId,
                    Time = DateTime.Now,
                    UserId = c.UserId
                });
                db.Comments.Remove(c);
                db.SaveChanges();
            }
            return RedirectToAction("Points", new { Id = eSubId });
        }
    }
}