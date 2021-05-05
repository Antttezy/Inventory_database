using Inventory_database.Models;
using Inventory_database.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Controllers
{
    public class SettingsController : Controller
    {
        public IAuthenticationProvider AuthenticationProvider { get; }

        public SettingsController(IAuthenticationProvider authenticationProvider)
        {
            AuthenticationProvider = authenticationProvider;
        }

        [Route("User/Settings")]
        public async Task<IActionResult> Index()
        {
            if (await Authorize() == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            return View();
        }



        protected async Task<User> Authorize()
        {
            string token = HttpContext.Request.Cookies["auth_token"];
            User user = await AuthenticationProvider.GetUserByTokenAsync(token);

            return user;
        }
    }
}
