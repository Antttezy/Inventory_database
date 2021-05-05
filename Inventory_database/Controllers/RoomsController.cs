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
    public class RoomsController : Controller
    {
        IRepository<Room> _repositoryRoom;
        IAuthenticationProvider AuthenticationProvider { get; }

        public RoomsController(IRepository<Room> repositoryRoom, IAuthenticationProvider authenticationProvider)
        {
            _repositoryRoom = repositoryRoom;
            AuthenticationProvider = authenticationProvider;
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

        public async Task<IActionResult> Create()
        {
            if (await Authorize() == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (await Authorize() == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

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
            if (await Authorize() == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

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
            if (await Authorize() == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

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

        public async Task<IActionResult> Delete(int roomId)
        {
            if (await Authorize() == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

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
            if (await Authorize() == null)
                return RedirectToAction("Login", "Auth", new { fallbackUrl = HttpContext.Request.Path });

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

        protected async Task<User> Authorize()
        {
            string token = HttpContext.Request.Cookies["auth_token"];
            User user = await AuthenticationProvider.GetUserByTokenAsync(token);

            return user;
        }
    }
}
