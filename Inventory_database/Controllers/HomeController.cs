using Inventory_database.Models;
using Inventory_database.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Controllers
{
    public class HomeController : Controller
    {
        IRepository<StorageItem> _repository;

        public HomeController(IRepository<StorageItem> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll());
        }
    }
}
