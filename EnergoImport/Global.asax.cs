using EnergoImport.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EnergoImport
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Если база пустая инициализируем ее
            //DataImportManager import = new DataImportManager();
            //import.InitContractsAndRegions();
            //import = null;
            DeletingPoints(DataImportManager.ContractType.CES);
            DeletingPoints(DataImportManager.ContractType.UGES);

            //Стартуем поток для периодического обновления базы
            Task.Factory.StartNew(UpdateDatabase);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void UpdateDatabase()
        {
            DataImportManager import = new DataImportManager();
            while(true)
            {
                import.UpdateAllPoints(1); //1 админ
                //Пауза на час
                Thread.Sleep(3600000);
            }
        }

        void DeletingPoints(DataImportManager.ContractType contractType)
        {
            //Подготовим списки в которых будут храниться объекты для удаления из БД
            List<RegPoint> regPointsForDelete = new List<RegPoint>();
            List<ESubstation> eSubsForDelete = new List<ESubstation>();
            
            DataImportManager import = new DataImportManager();
            //Выгрузка всех данных из старых баз (тест)
            var oldContract = import.LoadAllPointsFromContractDb(contractType);
            using (RegPointsContext db = new RegPointsContext())
            {
                //Определим имя контракта
                string contractName = "";
                if (contractType == DataImportManager.ContractType.CES) contractName = "ПО ЦЭС";
                if (contractType == DataImportManager.ContractType.UGES) contractName = "ПО УГЭС";
                //Выгрузим контракт из БД
                var contract = db.Contracts.FirstOrDefault(c => c.Name == contractName);
                //Выгрузим нужные регионы из БД
                if (contract != null)
                {
                    //Список всех подстанций
                    //List<RegPoint> oldRegPoints = new List<RegPoint>();
                    //List<ESubstation> oldESubs = new List<ESubstation>();

                    var netRegions = contract.NetRegions.Where(r => r.Contract.Name == contractName);
                    foreach (var region in netRegions) //Пробег по регионам определенного контракта
                    {
                        //Берем аналогичный регион из старой базы
                        var oldRegion = oldContract.NetRegions.Find(r => r.Name == region.Name);
                        if (oldRegion != null) //Если нашли регион
                        {
                            foreach (var eSub in region.ESubstatons)
                            {
                                //Ищем каждую подстанцию по oldId в старой базе
                                var oldESub = oldRegion.ESubstatons.FirstOrDefault(s => s.OldId == eSub.OldId);
                                if(oldESub != null)
                                {
                                    foreach(var regPoint in eSub.RegPoints)
                                    {
                                        var oldRegPoint = oldESub.RegPoints.FirstOrDefault(r => r.OldId == regPoint.OldId && r.Serial == regPoint.Serial);
                                        if(oldRegPoint == null)
                                        {
                                            regPointsForDelete.Add(regPoint);
                                            db.Actions.RemoveRange(regPoint.Actions);
                                        }
                                    }
                                }
                                else
                                {
                                    eSubsForDelete.Add(eSub);
                                    db.RegPoints.RemoveRange(eSub.RegPoints);
                                    db.Actions.RemoveRange(eSub.Actions);
                                }
                            }
                        }
                    }
                }
                db.ESubstations.RemoveRange(eSubsForDelete);
                db.RegPoints.RemoveRange(regPointsForDelete);
                db.SaveChanges();
                contractName = "";
                
            }
        }



        /*void CheckActions()
        {
            using (RegPointsContext db = new RegPointsContext())
            {
                foreach (var point in db.RegPoints)
                {
                    Models.Action act;
                    //Заявка принята
                    act = point.Actions.FirstOrDefault(a => a.ActionType == ActionType.CheckedAdd);
                    if (act == null && point.AddedInEnergo)
                        point.Actions.Add(new Models.Action() { ActionType = ActionType.CheckedAdd, ESubstationId = point.ESubstationId, Time = DateTime.Now, UserId = 1 });
                    //Заявка подана
                    act = point.Actions.FirstOrDefault(a => a.ActionType == ActionType.CheckedLinkIsOk);
                    if (act == null && point.LinkIsOk)
                        point.Actions.Add(new Models.Action() { ActionType = ActionType.CheckedLinkIsOk, ESubstationId = point.ESubstationId, Time = DateTime.Now, UserId = 1 });
                }
                db.SaveChanges();
            } 
        }*/
    }
}
