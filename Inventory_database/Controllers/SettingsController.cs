using Inventory_database.Models;
using Inventory_database.Services;
using Inventory_database.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Controllers
{
    public class SettingsController : Controller
    {
        IAuthenticationProvider AuthenticationProvider { get; }
        IRepository<User> UserRepository { get; }
        IHashingProvider HashingProvider { get; }

        public SettingsController(IAuthenticationProvider authenticationProvider, IRepository<User> userRepository, IHashingProvider hashingProvider)
        {
            AuthenticationProvider = authenticationProvider;
            UserRepository = userRepository;
            HashingProvider = hashingProvider;
        }

        [Route("User/Settings")]
        public async Task<IActionResult> Index()
        {
            User user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            SettingsViewModel vm = new SettingsViewModel
            {
                ChangeName = new ChangeNameViewModel
                {
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    ThirdName = user.ThirdName
                }
            };

            return View(vm);
        }

        [Route("User/Settings/ChangeName")]
        public async Task<IActionResult> ChangeName(SettingsViewModel model)
        {
            User user = await Authorize();

            if (user == null)
                return Unauthorized();

            if (ModelState.IsValid)
            {
                user = await UserRepository.Get(user.Id);
                user.FirstName = model.ChangeName.FirstName;
                user.SecondName = model.ChangeName.SecondName;
                user.ThirdName = model.ChangeName.ThirdName;

                await UserRepository.Update(user);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Index", model);
            }
        }

        [Route("User/Settings/ChangePassword")]
        public async Task<IActionResult> ChangePassword(SettingsViewModel model)
        {
            User user = await Authorize();

            if (user == null)
                return Unauthorized();

            if (ModelState.IsValid)
            {
                model.ChangeName = new ChangeNameViewModel
                {
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    ThirdName = user.ThirdName
                };

                if (!await AuthenticationProvider.IsLoginPasswordCorrect(user.Username, model.ChangePassword.OldPassword))
                {
                    ModelState.AddModelError("", "Неверный пароль");
                    return View("Index", model);
                }

                if (model.ChangePassword.NewPassword != model.ChangePassword.RepPassword)
                {
                    ModelState.AddModelError("", "Пароли не совпадают");
                    return View("Index", model);
                }

                user = await UserRepository.Get(user.Id);

                string token = HttpContext.Request.Cookies["auth_token"];
                await AuthenticationProvider.LogoutAsync(token);

                user.PasswordHash = HashingProvider.Hash(model.ChangePassword.NewPassword);
                await UserRepository.Update(user);

                token = await AuthenticationProvider.LoginAsync(user.Username, model.ChangePassword.NewPassword, TimeSpan.FromMinutes(30));
                HttpContext.Response.Cookies.Append("auth_token", token);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Index", model);
            }
        }

        protected async Task<User> Authorize()
        {
            string token = HttpContext.Request.Cookies["auth_token"];
            User user = await AuthenticationProvider.GetUserByTokenAsync(token);

            return user;
        }
    }
}
