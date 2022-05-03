using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IAuthorizationService authorizationService;

        public ClaimsController(UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }
        public IActionResult Index() => View(User?.Claims);

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string claimType, string claimValue)
        {
            User user = await userManager.GetUserAsync(HttpContext.User);
            Claim claim = new Claim(claimType, claimValue, ClaimValueTypes.String);
            IdentityResult result = await userManager.AddClaimAsync(user, claim);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
                Errors(result);
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Delete(string claimValues)
        {
            User user = await userManager.GetUserAsync(HttpContext.User);
            string[] claims = claimValues.Split(";");
            string claimType = claims[0], claimValue = claims[1], claimIssuer = claims[2];
            Claim claim = User.Claims.Where(t => t.Type == claimType && t.Value == claimValue && t.Issuer == claimIssuer).FirstOrDefault();
            IdentityResult result = await userManager.RemoveClaimAsync(user, claim);
            if (result.Succeeded)
                return RedirectToAction("Index");
            else
                Errors(result);
            return View("Index");

        }

        void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        [Authorize(Policy ="AspManager")]
        public IActionResult TestManager() => View("Index", User?.Claims);
        
        [Authorize(Policy = "LocalUsers")]
        public IActionResult TestPolicy2() => View("Index", User?.Claims);

        public async Task<IActionResult> TestPolicy3()
        {
            string[] allowedUsers = { "Serhii" };
            AuthorizationResult result = await authorizationService.AuthorizeAsync(User, allowedUsers, "PrivatePolicy");
            if (result.Succeeded)
                return View("Index", User?.Claims);
            return new ChallengeResult();
        }
       
    }
}
