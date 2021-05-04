using Inventory_database.Models;
using Inventory_database.Services;
using Inventory_database.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Controllers
{
    public class ItemsController : Controller
    {
        IRepository<StorageItem> _repositoryItem;
        IRepository<Room> _repositoryRoom;
        IRepository<ItemType> _repositoryType;

        public ItemsController(IRepository<StorageItem> repositoryItem, IRepository<Room> repositoryRoom, IRepository<ItemType> repositoryType)
        {
            _repositoryItem = repositoryItem;
            _repositoryRoom = repositoryRoom;
            _repositoryType = repositoryType;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var items = _repositoryItem.GetAll();
            var list = await items.Skip((page - 1) * 10).Take(10).OrderBy(i => i.Id).ToListAsync();

            ItemsViewModel itemsView = new ItemsViewModel
            {
                Items = list,
                Page = page
            };

            return View(itemsView);
        }

        public IActionResult Create()
        {
            var vm = GetItemData(null);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StorageItem item)
        {
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

        public IActionResult Delete(int itemId)
        {
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
    }
}
