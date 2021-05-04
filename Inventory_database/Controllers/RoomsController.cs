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
    public class RoomsController : Controller
    {
        IRepository<Room> _repositoryRoom;

        public RoomsController(IRepository<Room> repositoryRoom)
        {
            _repositoryRoom = repositoryRoom;
        }

        public async Task<IActionResult> Index(int page = 1)
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                await _repositoryRoom.Add(room);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(room);
            }
        }

        public async Task<IActionResult> Edit(int roomId)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Room room)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repositoryRoom.Update(room);
                    return RedirectToAction(nameof(Index));
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

        public IActionResult Delete(int roomId)
        {
            var deleteVM = new DeleteViewModel
            {
                ConfirmUrl = Url.Action(nameof(DeleteConfirm), new { roomId = roomId }),
                FallbackUrl = Url.Action(nameof(Index))
            };

            return View("_ConfirmDelete", deleteVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int roomId)
        {
            var room = await _repositoryRoom.Get(roomId);

            if (room != null)
            {
                await _repositoryRoom.Remove(room);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
