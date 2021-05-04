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
    public class TypesController : Controller
    {
        IRepository<ItemType> _repositoryType;

        public TypesController(IRepository<ItemType> repositoryType)
        {
            _repositoryType = repositoryType;
        }

        public async Task<IActionResult> Index(int page = 1)
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemType type)
        {
            if (ModelState.IsValid)
            {
                await _repositoryType.Add(type);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(type);
            }
        }


        public async Task<IActionResult> Edit(int typeId)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemType type)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repositoryType.Update(type);
                    return RedirectToAction(nameof(Index));
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

        public IActionResult Delete(int typeId)
        {
            var deleteVM = new DeleteViewModel
            {
                ConfirmUrl = Url.Action(nameof(DeleteConfirm), new { typeId = typeId }),
                FallbackUrl = Url.Action(nameof(Index))
            };

            return View("_ConfirmDelete", deleteVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int typeId)
        {
            var type = await _repositoryType.Get(typeId);

            if (type != null)
            {
                await _repositoryType.Remove(type);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
