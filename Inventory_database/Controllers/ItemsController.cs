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
    public class ItemsController : Controller
    {
        IRepository<StorageItem> _repositoryItem;
        IRepository<Room> _repositoryRoom;
        IRepository<ItemType> _repositoryType;

        IAuthenticationProvider AuthenticationProvider { get; }

        public ItemsController(IRepository<StorageItem> repositoryItem, IRepository<Room> repositoryRoom, IRepository<ItemType> repositoryType, IAuthenticationProvider authenticationProvider)
        {
            _repositoryItem = repositoryItem;
            _repositoryRoom = repositoryRoom;
            _repositoryType = repositoryType;
            AuthenticationProvider = authenticationProvider;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ItemsViewModel itemsView = new ItemsViewModel
            {
                Items = new List<StorageItem>()
            };

            var user = await Authorize();

            if (user != null && user.Roles.Any(r => r.Name == "Пользователь"))
            {
                var items = _repositoryItem.GetAll();
                var list = await items.OrderByDescending(i => i.Id).Skip((page - 1) * 10).Take(10).ToListAsync();

                itemsView.Items = list;
                itemsView.Page = new PagingViewModel(page, await items.CountAsync(), 10);
            }

            return View(itemsView);
        }

        public async Task<IActionResult> Create()
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Пользователь"))
                return Unauthorized();

            var vm = GetItemData(null);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StorageItem item)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Пользователь"))
                return Unauthorized();

            if (ModelState.IsValid)
            {
                await _repositoryItem.Add(item);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var vm = GetItemData(item);

                return View(vm);
            }
        }

        public async Task<IActionResult> Edit(int itemId)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Пользователь"))
                return Unauthorized();

            var item = await _repositoryItem.Get(itemId);

            if (item != null)
            {
                var vm = GetItemData(item);

                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StorageItem item)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Пользователь"))
                return Unauthorized();

            try
            {
                if (ModelState.IsValid)
                {
                    await _repositoryItem.Update(item);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(GetItemData(item));
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Delete(int itemId)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Пользователь"))
                return Unauthorized();

            var deleteVM = new DeleteViewModel
            {
                ConfirmUrl = Url.Action(nameof(DeleteConfirm), new { itemId = itemId }),
                FallbackUrl = Url.Action(nameof(Index))
            };

            return View("_ConfirmDelete", deleteVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int itemId)
        {
            var user = await Authorize();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            if (!user.Roles.Any(r => r.Name == "Пользователь"))
                return Unauthorized();

            var item = await _repositoryItem.Get(itemId);

            if (item != null)
            {
                await _repositoryItem.Remove(item);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        private CreateItemViewModel GetItemData(StorageItem item)
        {
            var typeQuery = _repositoryType
                .GetAll()
                .OrderBy(t => t.Name);

            var roomQuery = _repositoryRoom
                .GetAll()
                .OrderBy(r => r.Name);

            return new CreateItemViewModel
            {
                Types = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(typeQuery, nameof(ItemType.Id), nameof(ItemType.Name)),
                Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roomQuery, nameof(Room.Id), nameof(Room.Name)),
                InnerModel = item
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
