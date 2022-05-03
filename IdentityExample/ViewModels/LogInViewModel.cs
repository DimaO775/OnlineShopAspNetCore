using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="Логін")]
        public string Login { get; set; }
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Залишатись в системі?")]
        public bool IsPersistent { get; set; }

        public string ReturnUrl { get; set; }
    }
}
