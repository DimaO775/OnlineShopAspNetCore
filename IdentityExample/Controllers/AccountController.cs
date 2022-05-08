using Grpc.Core;
using IdentityExample.Models;
using IdentityExample.Services;
using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class AccountController : Controller
    {
        
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IEmailService emailService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Login, YearOfBirth = model.YearOfBearth };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    Redirect(model.ReturnUrl);
                    await ConfirmEmailButtonClick(user, model);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    Redirect(model.ReturnUrl);
                }
            }
            return View(model);
        }
        
        public async Task<IActionResult> ConfirmEmailButtonClick(User user, RegisterViewModel model)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(userManager.Users.Where(t => t.Id == user.Id).FirstOrDefault());
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = user.Id, code = code },
                protocol: HttpContext.Request.Scheme);

                await emailService.SendEmailAsync(model.Email, "Confirm your account",
                    $"<h3>Подтвердите, пожалуйста, ваш e-mail :)</h3>" +
                    $"<a style='color: white; height:50px; background-color: green; font-size:30px; " +
                    $"font-weight:600; padding:10px; text-decoration: none;' href='{callbackUrl}'>ПОДТВЕРДИТЬ</a>");
                return Redirect(model.ReturnUrl);           
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Users.Where(t=>t.UserName == model.Login).FirstOrDefault();
                /*if (user != null)
                {
                    if (!await userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "Вы не подтвердили свой email" + "");
                        return View(model);
                    }
                }*/

                //var user = await userManager.FindByNameAsync(model.Login);
                var result = await signInManager.PasswordSignInAsync(model.Login, model.Password, model.IsPersistent, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                        
                    }
                    else
                    {
                        return Redirect(model.ReturnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Користувача не знайдено і/або пароль не вірний");
                }
            }
            return PartialView("_Login", model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }

        [AllowAnonymous]
        public IActionResult GoogleAuth()
        {
            string redirectUrl = Url.Action("GoogleRedirect", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            
            return new ChallengeResult("Google", properties);
        }

        [AllowAnonymous]
        public IActionResult FbAuth()
        {
            string redirectUrl = Url.Action("GoogleRedirect", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GoogleRedirect()
        {
            ExternalLoginInfo loginInfo = await signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("Login");
            var loginResult = await signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, false);

            string[] userInfo = { loginInfo.Principal.FindFirst(ClaimTypes.Name).Value,
                    loginInfo.Principal.FindFirst(ClaimTypes.Email).Value};
            if (loginResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            User user = new User
            {
                UserName = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value,
                Email = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddLoginAsync(user, loginInfo);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return View(userInfo);
                }
            }
            return RedirectToAction("AccessDenied");
            
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    return View("ForgotPasswordConfirmation");
                }
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                StringBuilder builder = new StringBuilder();
                string emailTo = user.Email;
                builder.Append("<h3>Вы отправили запрос на сброс пароля!</h3>");
                builder.Append("<h3 style='color: red;'>Если это были не вы - срочно ответьте на ЭТО письмо</h3>");
                builder.Append($"<a style='color: white; height:50px; background-color: green; font-size:30px; " +
                    $"font-weight:600; padding:10px; text-decoration: none;' href='{callbackUrl}'>ПОДТВЕРДИТЬ</a>");                
                await emailService.SendEmailAsync(model.Email, "Reset Password", builder.ToString());
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
