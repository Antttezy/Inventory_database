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
    public class AuthController : Controller
    {
        public IAuthenticationProvider AuthenticationProvider { get; }
        public IRepository<User> UserRepository { get; }

        public AuthController(IAuthenticationProvider authenticationProvider, IRepository<User> userRepository)
        {
            AuthenticationProvider = authenticationProvider;
            UserRepository = userRepository;
        }

        [Route("Auth/Login")]
        public IActionResult Login()
        {
            return View(new AuthViewModel());
        }

        [Route("Auth/Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthViewModel model)
        {
            if (ModelState.IsValid)
            {
                string token = await AuthenticationProvider.LoginAsync(model.Login, model.Password, TimeSpan.FromMinutes(30));

                if (token == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return View(model);
                }
                else
                {
                    HttpContext.Response.Cookies.Append("auth_token", token);
                    return RedirectToAction(nameof(ItemsController.Index), "Items");
                }
            }
            else
            {
                return View(model);
            }
        }

        [Route("Auth/Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            string token = HttpContext.Request.Cookies["auth_token"];

            if (token != null)
                await AuthenticationProvider.LogoutAsync(token);

            return RedirectToAction(nameof(ItemsController.Index), "Items");
        }
    }
}
