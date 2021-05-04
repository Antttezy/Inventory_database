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
    public class HomeController : Controller
    {
        IRepository<StorageItem> _repositoryItem;
        IRepository<Room> _repositoryRoom;
        IRepository<ItemType> _repositoryType;


        public HomeController(IRepository<StorageItem> repositoryItem, IRepository<Room> repositoryRoom, IRepository<ItemType> repositoryType)
        {
            _repositoryItem = repositoryItem;
            _repositoryRoom = repositoryRoom;
            _repositoryType = repositoryType;
        }

        [Route("{controller}/Items/View")]
        public async Task<IActionResult> Items(int page = 1)
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

        [Route("{controller}/Types/View")]
        public async Task<IActionResult> Types(int page = 1)
        {
            var types = _repositoryType.GetAll();
            var list = await types.Skip((page - 1) * 10).Take(10).OrderBy(t => t.Id).ToListAsync();

            TypesViewModel typesView = new TypesViewModel
            {
                Types = list,
                Page = page
            };

            return View(typesView);
        }

        [Route("{controller}/Rooms/View")]
        public async Task<IActionResult> Rooms(int page = 1)
        {
            var rooms = _repositoryRoom.GetAll();
            var list = await rooms.Skip((page - 1) * 10).Take(10).OrderBy(r => r.Id).ToListAsync();

            RoomsViewModel roomsView = new RoomsViewModel
            {
                Rooms = list,
                Page = page
            };

            return View(roomsView);
        }


        [Route("{controller}/Items/Create")]
        public IActionResult CreateItem()
        {
            var vm = GetItemData(null);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Items/Create")]
        public async Task<IActionResult> CreateItem(StorageItem item)
        {
            if (ModelState.IsValid)
            {
                await _repositoryItem.Add(item);
                return RedirectToAction(nameof(Items));
            }
            else
            {
                var vm = GetItemData(item);

                return View(vm);
            }
        }

        [Route("{controller}/Types/Create")]
        public IActionResult CreateType()
        {
            return View();
        }

        [Route("{controller}/Types/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateType(ItemType type)
        {
            if (ModelState.IsValid)
            {
                await _repositoryType.Add(type);
                return RedirectToAction(nameof(Types));
            }
            else
            {
                return View(type);
            }
        }

        [Route("{controller}/Rooms/Create")]
        public IActionResult CreateRoom()
        {
            return View();
        }

        [Route("{controller}/Rooms/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                await _repositoryRoom.Add(room);
                return RedirectToAction(nameof(Rooms));
            }
            else
            {
                return View(room);
            }
        }

        [Route("{controller}/Items/Edit")]
        public async Task<IActionResult> EditItem(int itemId)
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

        [Route("{controller}/Items/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(StorageItem item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repositoryItem.Update(item);
                    return RedirectToAction(nameof(Items));
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

        [Route("{controller}/Types/Edit")]
        public async Task<IActionResult> EditType(int typeId)
        {
            var type = await _repositoryType.Get(typeId);

            if (type != null)
            {
                return View(type);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("{controller}/Types/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditType(ItemType type)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repositoryType.Update(type);
                    return RedirectToAction(nameof(Types));
                }
                else
                {
                    return View(type);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{controller}/Rooms/Edit")]
        public async Task<IActionResult> EditRoom(int roomId)
        {
            var room = await _repositoryRoom.Get(roomId);

            if (room != null)
            {
                return View(room);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("{controller}/Rooms/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(Room room)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repositoryRoom.Update(room);
                    return RedirectToAction(nameof(Rooms));
                }
                else
                {
                    return View(room);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{controller}/Items/Delete")]
        public IActionResult DeleteItem(int itemId)
        {
            var deleteVM = new DeleteViewModel
            {
                ConfirmUrl = Url.Action(nameof(DeleteItemConfirm), new { itemId = itemId}),
                FallbackUrl = Url.Action(nameof(Items))
            };

            return View("_ConfirmDelete", deleteVM);
        }

        [Route("{controller}/Items/Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItemConfirm(int itemId)
        {
            var item = await _repositoryItem.Get(itemId);

            if (item != null)
            {
                await _repositoryItem.Remove(item);
                return RedirectToAction(nameof(Items));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("{controller}/Types/Delete")]
        public IActionResult DeleteType(int typeId)
        {
            var deleteVM = new DeleteViewModel
            {
                ConfirmUrl = Url.Action(nameof(DeleteTypeConfirm), new { typeId = typeId }),
                FallbackUrl = Url.Action(nameof(Types))
            };

            return View("_ConfirmDelete", deleteVM);
        }

        [Route("{controller}/Types/Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTypeConfirm(int typeId)
        {
            var type = await _repositoryType.Get(typeId);

            if (type != null)
            {
                await _repositoryType.Remove(type);
                return RedirectToAction(nameof(Types));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("{controller}/Rooms/Delete")]
        public IActionResult DeleteRoom(int roomId)
        {
            var deleteVM = new DeleteViewModel
            {
                ConfirmUrl = Url.Action(nameof(DeleteRoomConfirm), new { roomId = roomId }),
                FallbackUrl = Url.Action(nameof(Rooms))
            };

            return View("_ConfirmDelete", deleteVM);
        }

        [Route("{controller}/Rooms/Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoomConfirm(int roomId)
        {
            var room = await _repositoryRoom.Get(roomId);

            if (room != null)
            {
                await _repositoryRoom.Remove(room);
                return RedirectToAction(nameof(Rooms));
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
