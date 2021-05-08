using Inventory_database.Models;
using Inventory_database.Services;
using Inventory_database.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Inventory_database.Controllers
{
    public class AuthController : Controller
    {
        static HashSet<string> AuthCodes { get; } = new HashSet<string>(); //Одноразовые коды для регистрации

        public IAuthenticationProvider AuthenticationProvider { get; }
        public IRepository<User> UserRepository { get; }
        public IRepository<Role> RoleRepository { get; }

        public IHashingProvider HashingProvider { get; }

        public AuthController(IAuthenticationProvider authenticationProvider, IRepository<User> userRepository, IHashingProvider hashingProvider, IRepository<Role> roleRepository)
        {
            AuthenticationProvider = authenticationProvider;
            UserRepository = userRepository;
            HashingProvider = hashingProvider;
            RoleRepository = roleRepository;
        }

        [Route("Auth/Login")]
        public IActionResult Login(string fallbackUrl = "/Home/Items")
        {
            if (!Url.IsLocalUrl(fallbackUrl))
                fallbackUrl = "Home/Items";

            return View(new AuthViewModel { Fallback = fallbackUrl });
        }

        [Route("Auth/Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthViewModel model)
        {
            if (ModelState.IsValid)
            {
                string token = await AuthenticationProvider.LoginAsync(model.Login, model.Password, TimeSpan.FromMinutes(30)); //Осуществляем вход в систему

                if (token == null) //Если не был получен токен доступа
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return View(model);
                }
                else
                {
                    HttpContext.Response.Cookies.Append("auth_token", token);
                    if (!Url.IsLocalUrl(model.Fallback)) //Проверка на локальность адреса переадресации
                        model.Fallback = "Home/Items";

                    return LocalRedirect(model.Fallback);
                }
            }
            else
            {
                return View(model);
            }
        }

        [Route("Auth/RegLinks")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRegistrationLink(int count)
        {
            var user = await Authorize();

            if (!user.Roles.Any(r => r.Name == "Администратор")) //Для генерации нужны права администратора
                return Unauthorized();

            List<string> links = new List<string>();

            if (count < 0) //Ограничение значкений между 0 и 100
                count = 0;
            else if (count > 100)
                count = 100;

            for (int i = 0; i < count; i++) //Генерируем указанное количество guid и ссылки на регистрацию с нужным параметром
            {
                string guid = Guid.NewGuid().ToString();
                string code = HttpUtility.UrlEncode(guid);
                string link = Url.ActionLink(nameof(Register), "Auth", new { code = code });

                lock (AuthCodes)
                {
                    AuthCodes.Add(code); //Добавляем код
                }

                links.Add(link);
            }

            return View(links);
        }

        [Route("Auth/Register")]
        public IActionResult Register(string code)
        {
            if (code == null || !AuthCodes.Contains(code))
                return Content("Недействительная ссылка");

            var vm = new RegisterViewModel
            {
                Code = code
            };

            return View(vm);
        }

        [Route("Auth/Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model?.Code == null || !AuthCodes.Contains(model.Code))
            {
                ModelState.AddModelError("", "Недействительная ссылка регистрации");
                return View(model);
            }


            if (UserRepository.GetAll().Any(u => u.Username.ToLower() == model.Login.ToLower()))
            {
                ModelState.AddModelError("", "Имя пользователя занято");
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают");
            }

            if (!ModelState.IsValid)
                return View(model);


            User user = new User //Создание пользователя
            {
                Username = model.Login,
                FirstName = "Имя",
                SecondName = "Фамилия",
                ThirdName = "Отчество",
                PasswordHash = HashingProvider.Hash(model.Password)
            };

            var role = await RoleRepository.GetAll().FirstAsync(r => r.Name == "Пользователь");
            user.Roles.Add(role);

            lock (AuthCodes) //После создания пользователя удаляем одноразовый код
            {
                AuthCodes.Remove(model.Code);
            }

            await UserRepository.Add(user);
            string token = await AuthenticationProvider.LoginAsync(model.Login, model.Password, TimeSpan.FromMinutes(30));      //Добавляем пользователя в хранилище и генерируем ему токен доступа
            HttpContext.Response.Cookies.Append("auth_token", token);

            return RedirectToAction("Index", "Items");
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

        protected async Task<User> Authorize() //Получение пользователя по токену доступа
        {
            string token = HttpContext.Request.Cookies["auth_token"];
            User user = await AuthenticationProvider.GetUserByTokenAsync(token);

            return user;
        }
    }
}
