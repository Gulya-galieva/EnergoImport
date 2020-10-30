using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnergoImport.Models
{
    public class LoginModel
    {
        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }
    }
}