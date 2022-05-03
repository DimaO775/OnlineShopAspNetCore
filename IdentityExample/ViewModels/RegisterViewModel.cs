using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="Логін")]
        public string Login { get; set; }
        [Required]
        [Display(Name = "Електронна  пошта")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Рік народження")]
        public int YearOfBearth { get; set; }



        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Паролі повинні співпадати!")]
        [Display(Name = "Підтвердіть пароль")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
