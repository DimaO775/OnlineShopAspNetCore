using IdentityExample.Models;
using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Components
{
    public class LogIn : ViewComponent
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        public LogIn(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IViewComponentResult> InvokeAsync(string returnUrl = null)
        {
            LoginViewModel model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);

        }

        /*[HttpPost]
        public async Task<IViewComponentResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await userManager.FindByNameAsync(model.Login);
                var result = await signInManager.PasswordSignInAsync(model.Login, model.Password, model.IsPersistent, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return View(model.ReturnUrl);
                    else
                    {
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Користувача не знайдено і/або пароль не вірний");
                }
            }
            return View(model.ReturnUrl);
        }*/


    }
}
