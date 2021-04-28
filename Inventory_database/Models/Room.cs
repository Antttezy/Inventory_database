using System.Collections.Generic;

namespace Inventory_database.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<StorageItem> Items { get; set; }

        public Room()
        {
            Items = new List<StorageItem>();
        }
    }
}