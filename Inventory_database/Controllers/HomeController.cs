using Inventory_database.Models;
using Inventory_database.Services;
using Inventory_database.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var typeQuery = _repositoryType
                .GetAll()
                .OrderBy(t => t.Name);

            var roomQuery = _repositoryRoom
                .GetAll()
                .OrderBy(r => r.Name);

            var createItemVM = new CreateItemViewModel
            {
                Types = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(typeQuery, nameof(ItemType.Id), nameof(ItemType.Name)),
                Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roomQuery, nameof(Room.Id), nameof(Room.Name))
            };

            return View(createItemVM);
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
                var typeQuery = _repositoryType
                .GetAll()
                .OrderBy(t => t.Name);

                var roomQuery = _repositoryRoom
                    .GetAll()
                    .OrderBy(r => r.Name);

                var createItemVM = new CreateItemViewModel
                {
                    Types = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(typeQuery, nameof(ItemType.Id), nameof(ItemType.Name)),
                    Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roomQuery, nameof(Room.Id), nameof(Room.Name)),
                    InnerModel = item
                };

                return View(createItemVM);
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

        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Items));
        }
    }
}
