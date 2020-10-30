using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnergoImport.Models
{
    public class PointsViewModel
    {
        public RegPoint Point { get; set; }
        public string Address { get => Point.Local + " " + Point.Address; }

        public string NetAddress { get {
                int dCount = 0;
                //Определим количество цифр в сетевом номере
                if (Point.InstallPlace.ToLower().Contains("ввод") || Point.InstallPlace.ToLower().Contains("ру")) dCount = 3;
                else
                    if (Point.TypeLink.ToLower().Contains("gsm")) dCount = 9;
                else
                    dCount = 5;
                return Point.Serial.Length > dCount ? Point.Serial.Substring(Point.Serial.Length - dCount, dCount) : "";
            }
        }
        public string TypePU { get
            {
                if (Point.TypePU.Length > 5)
                    return Point.TypePU.Substring(0, 5) + " " + Point.TypeLink;
                else
                    return Point.TypePU + " " + Point.TypeLink;
            }
        }

        public string InSmeta { get { return Point.InSmeta ? "+" : ""; } }

        public DateTime CheckedLinkIsOkDateTime
        {
            get
            {
                Models.Action action = Point.Actions.Where(a => a.ActionType == ActionType.CheckedLinkIsOk).OrderByDescending(a => a.Time).FirstOrDefault();
                if (action == null || !Point.LinkIsOk) return DateTime.MinValue;
                return action.Time;
            }
        }
        public string CheckedLinkIsOk {
            get
            {
                Models.Action action = Point.Actions.Where(a => a.ActionType == ActionType.CheckedLinkIsOk).OrderByDescending(a => a.Time).FirstOrDefault();
                if (action == null || !Point.LinkIsOk) return "-";
                return action.Time.ToString("dd MMMM HH:mm");

            }
        }
        public string CheckedAdded
        {
            get
            {
                Models.Action action = Point.Actions.Where(a => a.ActionType == ActionType.CheckedAdd).OrderByDescending(a => a.Time).FirstOrDefault();
                if (action == null || !Point.AddedInEnergo) return "-";
                return action.Time.ToString("dd MMMM HH:mm");
            }
        }
        public string CheckedInEnergo
        {
            get
            {
                Models.Action action = Point.Actions.Where(a => a.ActionType == ActionType.CheckedInEnergosphera).OrderByDescending(a => a.Time).FirstOrDefault();
                if (action == null || !Point.AcceptedInEnergo) return "-";
                return action.Time.ToString("dd MMMM HH:mm");
            }
        }

        public string RowColorCode
        {
            get
            {
                string colorCode = ";";
                if (Point.LinkIsOk) colorCode = "#fff4df;";
                if (Point.AddedInEnergo) colorCode = "#eef6ff;";
                if (Point.AcceptedInEnergo) colorCode = "#dfffe3;";
                if(TimeFromCheck > TimeSpan.FromDays(7)) colorCode = "#fdc8c8;";
                return colorCode;
            }
        }

        public TimeSpan TimeFromCheck{ get {
                DateTime tmp = CheckedLinkIsOkDateTime;
                if (Point.LinkIsOk && !Point.AcceptedInEnergo && tmp > new DateTime(2017, 1, 1))
                    return DateTime.Now - tmp;
                else
                    return TimeSpan.MinValue;
            }
        }
        public bool IsProblem { get {

                return true;
            }
        }
        public int Dublicates { get; set; }
        public PointsViewModel()
        {
            Point = new RegPoint();
        }
        public PointsViewModel(RegPoint point)
        {
            Point = point;
        }
    }


}