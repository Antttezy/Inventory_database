using Inventory_database.Models;
using System.Collections.Generic;

namespace Inventory_database.ViewModels
{
    public class RoomsViewModel
    {
        public IEnumerable<Room> Rooms { get; set; }

        public int Page { get; set; }
    }
}
