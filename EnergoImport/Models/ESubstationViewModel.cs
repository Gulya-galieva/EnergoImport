using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnergoImport.Models
{
    public class ESubstationViewModel
    {
        public ESubstation ESubstation { get; set; }
        public string LastAction { get; set; }
        public bool InBookmarks { get; set; }
        //Stats
        public int PointsCount { get { return ESubstation.RegPoints.Count(); } }
        //Запросов на добавление в Энергосферу
        public int RequestsToAddCount { get { return ESubstation.RegPoints.Count(p => p.LinkIsOk && !p.AcceptedInEnergo); } }
        //Обработано заявок
        public int AddedCount { get { return ESubstation.RegPoints.Count(p => p.AddedInEnergo && !p.AcceptedInEnergo); } }
        //Добавлено в Энергосферу
        public int InEnergoCount { get { return ESubstation.RegPoints.Count(p => p.AcceptedInEnergo); } }
        //Осталось добавить в сметы
        public int ReadyForSmeta { get { return ESubstation.RegPoints.Count(p => p.AcceptedInEnergo && !p.InSmeta); } }
        //Время последнего коммента
        public string LastComment { get
            {
                Comment lastComment = ESubstation.Comments.OrderByDescending(c => c.Time).FirstOrDefault();
                if (lastComment != null)
                {
                    return lastComment.Time.ToString("dd MMMM в HH:mm");                    
                }
                else return "";
            }
        }
        public int CommentsCount { get => ESubstation.Comments.Count(); }

        public string PhoneNumber { get {
                return ESubstation.RegPoints.FirstOrDefault(p => p.TypeLink.ToLower().Contains("plc"))?.PhoneNumber;
            }
        }

        public string OperatorName
        {
            get
            {
                if (PhoneNumber == null) return "";
                string phoneNum = PhoneNumber;
                if (phoneNum.StartsWith("8917") || phoneNum.StartsWith("8986") || phoneNum.StartsWith("8987"))
                    return "МТС";
                if (phoneNum.StartsWith("8937"))
                    return "Мегафон";
                if (phoneNum.StartsWith("8967") || phoneNum.StartsWith("8963"))
                    return "Билайн";
                return "";
            }
        }
        public string OperatorIcon
        {
            get
            {
                string phoneNum = PhoneNumber;
                if (phoneNum == null || phoneNum == string.Empty) return "";
                if (OperatorName == "МТС")
                    return "<a data-toggle=\"tooltip\" data-placement=\"top\" title=\"[" + OperatorName + "] " + phoneNum + "\" style=\"color: red\">&#9679;</a>";
                if (OperatorName == "Мегафон")
                    return "<a data-toggle=\"tooltip\" data-placement=\"top\" title=\"[" + OperatorName + "] " + phoneNum + "\" style=\"color: forestgreen\">&#9679;</a>";
                if (OperatorName == "Билайн")
                    return "<a data-toggle=\"tooltip\" data-placement=\"top\" title=\"[" + OperatorName + "] " + phoneNum + "\" style=\"color: orange\">&#9679;</a>";
                return "<a style=\"color: gray\">&#9679;</a>";
            }
        }
        
        //Цвет строки
        public string RowColorCode
        {
            get
            {
                string colorCode = ";";
                if (RequestsToAddCount > 0) colorCode = "#fff4df;";
                if (AddedCount > 0 && AddedCount == RequestsToAddCount) colorCode = "#eef6ff;";
                if (InEnergoCount == PointsCount) colorCode = "#dfffe3;";
                if (HaveProblems) colorCode = "#fdc8c8;";
                //if (LastComment == LastAction) colorCode = "#C0C0C0";
                return colorCode;
            }
        }
        public bool HaveProblems { get
            {
                if(RequestsToAddCount > 0 && AddedCount < RequestsToAddCount)
                {
                    bool isHaveProblem = false;
                    ESubstation.RegPoints.Where(p => p.LinkIsOk && !p.AcceptedInEnergo && !p.AddedInEnergo).ToList().ForEach(p =>
                    {
                        Models.Action action = p.Actions.Where(a => a.ActionType == ActionType.CheckedLinkIsOk).OrderByDescending(a => a.Time).FirstOrDefault();
                        if (action != null)
                        {
                            TimeSpan diff = DateTime.Now - action.Time;
                            if (diff > TimeSpan.FromDays(7)) { isHaveProblem = true; return; }
                        }
                    });
                    if (isHaveProblem) return true;
                }
                return false;
            }
        }

        public ESubstationViewModel()
        {
            ESubstation = new ESubstation();
            LastAction = "-";
        }
    }
}