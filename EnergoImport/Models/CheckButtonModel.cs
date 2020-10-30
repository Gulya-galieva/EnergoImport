using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnergoImport.Models
{
    
    public class CheckButtonModel
    {
        public int Id { get; set; }
        public DataType Data { get; set; }
        public bool Status { get; set; }

        public string Text
        {
            get
            {
                if (Data == DataType.None) return "-";
                if (Status) return "✓"; else return "Нет";
            }
        }

        public string CSSClass { get { if (Status) return "btn btn-success"; else return "btn btn-default"; } }

        public enum DataType : int
        {
            None,
            LinkIsOk,
            AddedInEnergo,
            AcceptedInEnergo
        }

        public CheckButtonModel()
        {
            Id = 0;
            Data = DataType.None;
            Status = false;
        }
    }
}