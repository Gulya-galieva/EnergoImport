using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnergoImport.Models
{
    public class ActionViewModel
    {
        public Action Action { get; set; }

        public string ColorRowCode
        {
            get
            {
                switch (Action.ActionType)
                {
                    case ActionType.Import:                 return ";";
                    case ActionType.Rename:                 return ";";
                    case ActionType.Delete:                 return ";";
                    case ActionType.RegPointAdd:            return "#c4fff2;";
                    case ActionType.RegPointUpdate:         return "#f3fffc;";
                    case ActionType.RegPointDelete:         return "#ff9898;";
                    case ActionType.CheckedInEnergosphera:  return "#dfffe3;";
                    case ActionType.UncheckedInEnergosphera:return "#fff0f0;";
                    case ActionType.CheckedLinkIsOk:        return "#fff4df;";
                    case ActionType.UncheckedLinkIsOk:      return "#fff0f0;";
                    case ActionType.CheckedAdd:             return "#eef6ff;";
                    case ActionType.UncheckedAdd:           return "#fff0f0;";
                    case ActionType.AddComment:             return "#C0C0C0;";
                    case ActionType.DeleteComment:          return ";";
                    case ActionType.ChangeESubStatus:       return ";";
                    case ActionType.CheckedPollRequest:     return ";";
                    case ActionType.UncheckedPollRequest:   return ";";
                }
                return ";";
            }
        }

        public string Message {
            get
            {
                string msg = "";
                switch (Action.ActionType)
                {
                    case ActionType.Import:
                        msg = "Объект добавлен в систему";
                        break;
                    case ActionType.Rename:
                        msg = "Объект переименован: " + Action.Comment;
                        break;
                    case ActionType.Delete:
                        msg = "Объект удален: " + Action.Comment;
                        break;
                    case ActionType.RegPointAdd:
                        msg = "Точка добавлена";
                        break;
                    case ActionType.RegPointUpdate:
                        msg = "Точка учета обновлена: " + Action.Comment;
                        break;
                    case ActionType.RegPointDelete:
                        msg = "Точка удалена: " + Action.Comment;
                        break;
                    case ActionType.CheckedInEnergosphera:
                        msg = "Отметил 'В Энергосфере'";
                        break;
                    case ActionType.UncheckedInEnergosphera:
                        msg = "Убрал 'В Энергосфере'";
                        break;
                    case ActionType.CheckedLinkIsOk:
                        msg = "Отметил 'Проверена связь'";
                        break;
                    case ActionType.UncheckedLinkIsOk:
                        msg = "Убрал 'Проверена связь'";
                        break;
                    case ActionType.CheckedAdd:
                        msg = "Отметил 'Отправлено в ЭС'";
                        break;
                    case ActionType.UncheckedAdd:
                        msg = "Убрал 'Отправлено в ЭС'";
                        break;
                    case ActionType.AddComment:
                        msg = "Добавил комментарий";
                        break;
                    case ActionType.DeleteComment:
                        msg = "Удалил комментарий";
                        break;
                    case ActionType.ChangeESubStatus:
                        msg = "Изменил статус на: " + Action.Comment;
                        break;
                    case ActionType.CheckedInSmeta:
                        msg = "Отметил 'В Смете'";
                        break;
                    case ActionType.UncheckedInSmeta:
                        msg = "Убрал 'В Смете'";
                        break;
                    case ActionType.CheckedPollRequest:
                        msg = "Добавил в очередь опроса";
                        break;
                    case ActionType.UncheckedPollRequest:
                        msg = "Удалил из очереди опроса";
                        break;
                }
                return msg;
            }
        }

        public string TypePU
        {
            get
            {
                if (Action.RegPoint != null)
                {
                    if (Action.RegPoint.TypePU.Length > 5)
                        return Action.RegPoint.TypePU.Substring(0, 5) + " " + Action.RegPoint.TypeLink;
                    else
                        return Action.RegPoint.TypePU + " " + Action.RegPoint.TypeLink;
                }
                else return "";
            }
        }
        public string Serial { get {
                if (Action.RegPoint != null)
                {
                    return Action.RegPoint.Serial;
                }
                else return "";
            }
        }
        public string Address { get {
                if (Action.RegPoint != null)
                {
                    return Action.RegPoint.Address;
                }
                else return "";
            } }
        public string UserName {
            get
            {
                string name = "Аноним";
                if(Action.User != null) name = Action.User.Name;
                return name;
            }
        }

    }
}