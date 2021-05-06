using Inventory_database.Models;
using Inventory_database.Services;
using Inventory_database.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Controllers
{
    public class AdministrationController : Controller
    {
        public IRepository<User> UserRepository { get; }
        public IAuthenticationProvider AuthenticationProvider { get; }

        public AdministrationController(IRepository<User> userRepository, IAuthenticationProvider authenticationProvider)
        {
            UserRepository = userRepository;
            AuthenticationProvider = authenticationProvider;
        }


        [Route("Administration")]
        public async Task<IActionResult> AdminPanel(int page = 1)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Администратор"))
                return Unauthorized();

            List<User> users = await UserRepository.GetAll().OrderByDescending(u => u.Id).Skip((page - 1) * 10).Take(5).ToListAsync();

            var vm = new AdministrationViewModel
            {
                Users = users,
                Page = page
            };

            return View(vm);
        }

        protected async Task<User> Authorize()
        {
            string token = HttpContext.Request.Cookies["auth_token"];
            User user = await AuthenticationProvider.GetUserByTokenAsync(token);

            return user;
        }
    }
}
