using IdentityExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace IdentityExample.Controllers
{
    [Authorize(Roles = "manager,admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;

        public UserController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index() => View(userManager.Users.ToList());

        [HttpGet]
        public IActionResult Create() => View();


        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { UserName = model.Login, Email = model.Email, YearOfBirth = model.Year};
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            User user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(new EditUserViewModel {
                Login = user.UserName,
                Id = user.Id,
                Email = user.Email,
                Year = user.YearOfBirth
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }
                user.UserName = model.Login;
                user.Email = model.Email;
                user.YearOfBirth = model.Year;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index", "User");
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            User user = await userManager.FindByIdAsync(id);
            await userManager.DeleteAsync(user);
            return RedirectToAction("Index", "User");
        }


        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id) {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            User user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(new ChangePasswordViewModel
            {
                Login = user.UserName,
                Id = user.Id 
            });
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                IPasswordValidator<User> passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                IPasswordHasher<User> passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
                User user = await userManager.FindByIdAsync(model.Id);
                var validationResult = await passwordValidator.ValidateAsync(userManager, user, model.NewPassword);
                if (validationResult.Succeeded)
                {
                    user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "User");
                }
                else
                    ModelState.AddModelError(string.Empty, "Некоректний пароль!");
            }
            return View(model);
        }

    }
}
