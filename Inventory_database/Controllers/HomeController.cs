using Inventory_database.Models;
using Inventory_database.Services;
using Inventory_database.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Items(int page = 1)
        {
            var items = _repositoryItem.GetAll();
            var list = await items.Skip((page - 1) * 10).Take(10).ToListAsync();

            ItemsViewModel viewModel = new ItemsViewModel
            {
                Items = list,
                Page = page
            };

            return View(viewModel);
        }

        public IActionResult Index()
        {
            return RedirectToActionPermanent("Items");
        }
    }
}
