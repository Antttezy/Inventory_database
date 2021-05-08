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
        public IRepository<Role> RoleRepository { get; }
        public IHashingProvider HashingProvider { get; }
        public IAuthenticationProvider AuthenticationProvider { get; }

        public AdministrationController(IRepository<User> userRepository, IAuthenticationProvider authenticationProvider, IRepository<Role> roleRepository, IHashingProvider hashingProvider)
        {
            UserRepository = userRepository;
            AuthenticationProvider = authenticationProvider;
            RoleRepository = roleRepository;
            HashingProvider = hashingProvider;
        }


        [Route("Administration")]
        public async Task<IActionResult> AdminPanel(int page = 1)
        {
            var user = await Authorize();
            if (user == null)                                                                               //Если пользователь не вошел, перенаправляем на страницу авторизации
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Администратор"))                                            //Если не соответствует роли, выдаем сообщение
                return Unauthorized();

            List<User> users = await UserRepository.GetAll().OrderByDescending(u => u.Id).Skip((page - 1) * 5).Take(5).ToListAsync(); //Список пользователей по страницам

            var vm = new AdministrationViewModel
            {
                Users = users,
                Page = new PagingViewModel(page, await UserRepository.GetAll().CountAsync(), 5) //Задание модели постраничного просмотра
            };

            return View(vm);
        }


        [Route("Administration/Edit")]
        public async Task<IActionResult> Edit(int userId)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Администратор"))
                return Unauthorized();

            var edit = await UserRepository.Get(userId);

            if (edit != null)
            {
                var vm = await GetEditUserViewModelAsync(edit);

                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("Administration/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Администратор"))
                return Unauthorized();

            if (ModelState.IsValid)
            {
                try
                {
                    User edit = await UserRepository.Get(model.User);

                    edit.Roles = await RoleRepository.GetAll() //Получение ролей по переданному списку
                        .Where(r => model.RolesId
                        .Any(i => r.Id == i))
                        .ToListAsync();

                    await UserRepository.Update(edit); //Обновление ролей пользователя
                    return RedirectToAction("Index", "Settings");
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            else
            {
                return View(model);
            }
        }


        [Route("Administration/Restore")]
        public async Task<IActionResult> Restore(int userId) //Восстановление пароля администратором
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Администратор"))
                return Unauthorized();

            if (UserRepository.GetAll().Any(u => u.Id == userId))
            {
                return View(new RestoreUserViewModel
                {
                    Id = userId
                });
            }
            else
            {
                return NotFound();
            }
        }


        [Route("Administration/Restore")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(RestoreUserViewModel model)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Администратор"))        //Для этого действия нужны права администратора
                return Unauthorized();

            if (ModelState.IsValid)
            {
                var restoreUser = await UserRepository.Get(model.Id);   //Получение пользователя, для которого надо восстановить пароль

                if (restoreUser != null)
                {
                    restoreUser.PasswordHash = HashingProvider.Hash(model.Password);                                    //Создание нового пароля
                    await UserRepository.Update(restoreUser);                                                           //Обновление пользователя в репозитории
                    var token = await AuthenticationProvider.GenerateTokenAsync(restoreUser.Username, model.Password);  //После смены пароля выбрасываем пользователя из всех сессий
                    await AuthenticationProvider.LogoutFromAllSessionsAsync(token);

                    return RedirectToAction(nameof(AdminPanel));
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return View(model);
            }
        }


        [Route("Administration/Delete")]
        public async Task<IActionResult> Delete(int userId)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Администратор"))
                return Unauthorized();

            if (UserRepository.Get(userId) != null)
            {
                var deleteVM = new DeleteViewModel
                {
                    ConfirmUrl = Url.Action(nameof(DeleteConfirm), new { userId }),
                    FallbackUrl = Url.Action(nameof(AdminPanel))
                };

                return View("_ConfirmDelete", deleteVM); //Страница подтверждения удаления
            }
            else
            {
                return NotFound();
            }
        }

        [Route("Administration/Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int userId)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Администратор"))
                return Unauthorized();

            var delUser = await UserRepository.Get(userId);

            if (delUser != null)
            {
                await UserRepository.Remove(delUser);       //Удаляем нужного пользователя
                return RedirectToAction(nameof(AdminPanel));
            }
            else
            {
                return NotFound();
            }
        }

        protected async Task<EditUserViewModel> GetEditUserViewModelAsync(User user) //Получает модель редактирования пользователя
        {
            return new EditUserViewModel //id и множественный список ролей
            {
                User = user.Id,
                UserRoles = new Microsoft.AspNetCore.Mvc.Rendering.MultiSelectList(await RoleRepository.GetAll().ToListAsync(), nameof(Role.Id), nameof(Role.Name), user.Roles)
            };
        }

        protected async Task<User> Authorize()
        {
            string token = HttpContext.Request.Cookies["auth_token"];
            User user = await AuthenticationProvider.GetUserByTokenAsync(token);

            return user;
        }
    }
}
