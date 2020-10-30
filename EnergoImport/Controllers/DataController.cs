using EnergoImport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ClosedXML.Excel;

namespace EnergoImport.Controllers
{
    public class DataController : Controller
    {
        bool GetAccessToImport()
        {
            using (RegPointsContext db = new RegPointsContext())
            {
                User usr = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
                if (usr == null) return false;
                return usr.AccessImport;
            }
        }

        [HttpGet]
        public ActionResult Index(int importCount = 0)
        {
            ViewBag.ImportCount = importCount;
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if(!GetAccessToImport()) return RedirectToAction("Index", new { importCount = -1 });
            int importCnt = 0;
            List<string> lostPoints = new List<string>();
            if (upload != null)
            {
                // получаем имя файла
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                string ext = Path.GetExtension(fileName);
                if (ext != ".txt") return RedirectToAction("Index", new { importCount = -1 });
                // сохраняем файл в папку Files в проекте
                System.IO.Directory.CreateDirectory(Server.MapPath("~/App_Data/ImportEnergo/"));
                string patth = Server.MapPath("~/App_Data/ImportEnergo/" + "ImportEnegrosphera_");
                patth += DateTime.Now.ToString("dd_MM_yyyy_HH_mm") + "_" + fileName;
                upload.SaveAs(patth);
                List<string> serialsFromFile = System.IO.File.ReadAllLines(patth).ToList();
                using (RegPointsContext db = new RegPointsContext())
                {
                    User user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
                    if (user != null)
                    {
                        List<string> serialsListFromDb = (from p in db.RegPoints select p.Serial).ToList();
                        serialsFromFile.ForEach(s =>
                        {
                            var findSerial = serialsListFromDb.FirstOrDefault(str => str == s);
                            if(findSerial != null)
                            {
                                RegPoint point = db.RegPoints.FirstOrDefault(p => p.Serial == s);
                                if (point != null)
                                {
                                    if (!point.AcceptedInEnergo)
                                    {
                                        point.AcceptedInEnergo = true;
                                        point.Actions.Add(new Models.Action() { ActionType = ActionType.CheckedInEnergosphera, ESubstationId = point.ESubstationId, Time = DateTime.Now, UserId = user.Id });
                                        importCnt++;
                                    }
                                }
                            }
                            else
                            {
                                lostPoints.Add(s);
                            }
                        });
                    }
                    db.SaveChanges();
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/ShareFiles/"));
                    System.IO.File.WriteAllLines(Server.MapPath("~/ShareFiles/LostPoints.txt"), lostPoints.ToArray());
                }
            }
            return RedirectToAction("Index", new { importCount = importCnt });
        }

        [HttpPost]
        public ActionResult UploadDSS(HttpPostedFileBase uploadDSS)
        {
            if (!GetAccessToImport()) return RedirectToAction("Index", new { importCount = -1 });
            int importCnt = 0;
            List<string> lostPoints = new List<string>();
            if (uploadDSS != null)
            {
                //Получаем имя файла
                string fileName = System.IO.Path.GetFileName(uploadDSS.FileName);
                //Получаем расширение файла
                string ext = Path.GetExtension(fileName);
                if(ext != ".dss") return RedirectToAction("Index", new { importCount = -1 });

                //Cохраняем файл в папку Files в проекте
                System.IO.Directory.CreateDirectory(Server.MapPath("~/App_Data/USPDConfigs/"));
                string patth = Server.MapPath("~/App_Data/USPDConfigs/" + "USPDConfig_");
                patth += DateTime.Now.ToString("dd_MM_yyyy_HH_mm") + "_" + fileName;
                uploadDSS.SaveAs(patth);
                //Получаем строки из файла в виде массива
                List<string> serialsFromFile = System.IO.File.ReadAllLines(patth).ToList();
                serialsFromFile = serialsFromFile.Where(s => s.Length > 14).ToList(); //Убираем все лишнее
                using (RegPointsContext db = new RegPointsContext())
                {
                    User user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
                    if (user != null)
                    {
                        List<string> serialsListFromDb = (from p in db.RegPoints select p.Serial).ToList();
                        serialsFromFile.ForEach(s =>
                        {
                            var findSerial = serialsListFromDb.FirstOrDefault(str => str == s);
                            if(findSerial != null)
                            {
                                RegPoint point = db.RegPoints.FirstOrDefault(p => p.Serial == s);
                                if (point != null)
                                {
                                    if (!point.LinkIsOk)
                                    {
                                        point.LinkIsOk = true;
                                        point.Actions.Add(new Models.Action() { ActionType = ActionType.CheckedLinkIsOk, ESubstationId = point.ESubstationId, Time = DateTime.Now, UserId = user.Id });
                                        importCnt++;
                                    }
                                }
                            }
                            else
                            {
                                lostPoints.Add(s);
                            }
                        });
                    }
                    db.SaveChanges();
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/ShareFiles/"));
                    System.IO.File.WriteAllLines(Server.MapPath("~/ShareFiles/LostPoints.txt"), lostPoints.ToArray());
                }
            }
            return RedirectToAction("Index", new { importCount = importCnt });
        }

        [HttpPost]
        public ActionResult UploadSmeta(HttpPostedFileBase uploadSmeta)
        {
            if (!GetAccessToImport()) return RedirectToAction("Index", new { importCount = -1 });
            int importCnt = 0;
            List<string> lostPoints = new List<string>();
            if (uploadSmeta != null)
            {
                //Получаем имя файла
                string fileName = System.IO.Path.GetFileName(uploadSmeta.FileName);
                //Получаем расширение файла
                string ext = Path.GetExtension(fileName);
                if (ext != ".blin") return RedirectToAction("Index", new { importCount = -1 });

                //Cохраняем файл в папку Files в проекте
                System.IO.Directory.CreateDirectory(Server.MapPath("~/App_Data/ImportSmet/"));
                string patth = Server.MapPath("~/App_Data/ImportSmet/" + "Smeti_");
                patth += DateTime.Now.ToString("dd_MM_yyyy_HH_mm") + "_" + fileName;
                uploadSmeta.SaveAs(patth);
                //Получаем строки из файла в виде массива
                List<string> serialsFromFile = System.IO.File.ReadAllLines(patth).ToList();
                serialsFromFile = serialsFromFile.Where(s => s.Length > 14).ToList(); //Убираем все лишнее
                using (RegPointsContext db = new RegPointsContext())
                {
                    User user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
                    if (user != null)
                    {
                        List<string> serialsListFromDb = (from p in db.RegPoints select p.Serial).ToList();
                        serialsFromFile.ForEach(s =>
                        {
                            var findSerial = serialsListFromDb.FirstOrDefault(str => str == s);
                            if (findSerial != null)
                            {
                                RegPoint point = db.RegPoints.FirstOrDefault(p => p.Serial == s);
                                if (point != null)
                                {
                                    if (!point.InSmeta)
                                    {
                                        point.InSmeta = true;
                                        point.Actions.Add(new Models.Action() {
                                            ActionType = ActionType.CheckedInSmeta,
                                            ESubstationId = point.ESubstationId,
                                            Time = DateTime.Now,
                                            UserId = user.Id });
                                        importCnt++;
                                    }
                                }
                            }
                            else
                            {
                                lostPoints.Add(s);
                            }
                        });
                    }
                    db.SaveChanges();
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/ShareFiles/"));
                    System.IO.File.WriteAllLines(Server.MapPath("~/ShareFiles/LostPoints.txt"), lostPoints.ToArray());
                }
            }
            return RedirectToAction("Index", new { importCount = importCnt });
        }
        public FileResult GetExcelPoints(int? Id)
        {
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Лист1");
            string fileName = "notFound";
            using (RegPointsContext db = new RegPointsContext())
            {
                var eSub = db.ESubstations.Find(Id);
                if(eSub != null)
                {
                    fileName = eSub.Name + "_" + DateTime.Now;
                    //создадим заголовки у столбцов
                    ws.Cell(1, 1).Value = "№";
                    ws.Cell(1, 2).Value = "Адрес";
                    ws.Cell(1, 3).Value = "ФИО";
                    ws.Cell(1, 4).Value = "Место установки";
                    ws.Cell(1, 5).Value = "Тип ПУ";
                    ws.Cell(1, 6).Value = "Тип связи";
                    ws.Cell(1, 7).Value = "Зав.№";
                    ws.Cell(1, 8).Value = "Сетевой №";
                    ws.Cell(1, 9).Value = "Номер тел.";
                    ws.Cell(1, 10).Value = "Связь";
                    ws.Cell(1, 11).Value = "Отправлен в ЭС";
                    ws.Cell(1, 12).Value = "В Энергосфере";
                    ws.Range(ws.Cell(1, 1), ws.Cell(1, 12)).Style.Font.Bold = true;
                    ws.Range(ws.Cell(1, 1), ws.Cell(1, 12)).Style.Fill.BackgroundColor = XLColor.LightGray;
                    // 
                    List<RegPoint> pointsList = (from p in eSub.RegPoints
                                     orderby p.Serial
                                     select p).ToList();
                    int n = 1;
                    foreach (RegPoint p in pointsList)
                    {
                        ws.Cell(n + 1, 1).Value = n;
                        ws.Cell(n + 1, 2).Value = p.Address;
                        ws.Cell(n + 1, 3).Value = p.FIO;
                        ws.Cell(n + 1, 4).Value = p.InstallPlace + " " + p.TTKoefficient;
                        ws.Cell(n + 1, 5).Value = (p.TypePU.Length > 5) ? p.TypePU.Substring(0, 5) : p.TypePU;
                        ws.Cell(n + 1, 6).Value = p.TypeLink;
                        ws.Cell(n + 1, 7).Value = p.Serial;
                        ws.Cell(n + 1, 7).Style.NumberFormat.Format = "000000000000000";
                        ws.Cell(n + 1, 8).Value = (p.Serial.Length > 5) ? p.Serial.Substring(p.Serial.Length - 5, 5) : "";
                        ws.Cell(n + 1, 8).Style.NumberFormat.Format = "00000";
                        if (p.InstallPlace.ToLower().Contains("ввод") || p.InstallPlace.ToLower().Contains("ру")) { 
                            ws.Cell(n + 1, 8).Value = (p.Serial.Length > 3) ? p.Serial.Substring(p.Serial.Length - 3, 3) : "";
                            ws.Cell(n + 1, 8).Style.NumberFormat.Format = "000";
                        }
                        ws.Cell(n + 1, 9).Value = p.PhoneNumber;
                        ws.Cell(n + 1, 10).Value = (p.LinkIsOk)? "Проверен" : "";
                        ws.Cell(n + 1, 11).Value = (p.AddedInEnergo) ? "Отправлен" : "";
                        ws.Cell(n + 1, 12).Value = (p.AcceptedInEnergo) ? "Да" : "";
                        //Красим
                        if (p.AddedInEnergo) { ws.Range(ws.Cell(n + 1, 1), ws.Cell(n + 1, 12)).Style.Fill.BackgroundColor = XLColor.LightBlue; }
                        if (p.LinkIsOk) { ws.Range(ws.Cell(n + 1, 1), ws.Cell(n + 1, 12)).Style.Fill.BackgroundColor = XLColor.LightYellow; }
                        if (p.AcceptedInEnergo) { ws.Range(ws.Cell(n + 1, 1), ws.Cell(n + 1, 12)).Style.Fill.BackgroundColor = XLColor.LightGreen; }
                        n++;
                    }
                    // пример создания сетки в диапазоне
                    var rngTable = ws.Range("A1:L" + n);
                    rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    ws.Columns().AdjustToContents(); //ширина столбца по содержимому
                }
            }
                
            // вернем пользователю файл без сохранения его на сервере
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
            }
        }
    }
}