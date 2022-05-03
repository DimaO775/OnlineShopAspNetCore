using IdentityExample.Models;
using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index() => View(await roleManager.Roles.ToListAsync());
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError(string.Empty, "Назва ролі не може бути порожньою");
                return View();
            }
            if (await roleManager.RoleExistsAsync(name))
            {
                ModelState.AddModelError(string.Empty, "Така роль вже існує!");
                return View();
            }
            await roleManager.CreateAsync(new IdentityRole { Name = name });
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            await roleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserList() => View(await userManager.Users.ToListAsync());

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            User user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            ChangeUserRolesDTO viewModel = new ChangeUserRolesDTO
            {
                Id = user.Id,
                AllRoles = await roleManager.Roles.ToListAsync(),
                UserRoles = await userManager.GetRolesAsync(user),
                Login = user.UserName,
                Email = user.Email
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUserRoles(ChangeUserRolesDTO model)
        {
            if(ModelState.IsValid)
            {
                User user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return NotFound();
                var oldUserRoles = await userManager.GetRolesAsync(user);
                var addedRoles = model.UserRoles.Except(oldUserRoles);
                var deletedRoles = oldUserRoles.Except(model.UserRoles);
                await userManager.AddToRolesAsync(user, addedRoles);
                await userManager.RemoveFromRolesAsync(user, deletedRoles);
                return RedirectToAction("UserList");
            }
            return View(model);
        }
    }
}
