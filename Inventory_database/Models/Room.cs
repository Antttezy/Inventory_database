using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_database.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public ICollection<StorageItem> Items { get; set; }

        public Room()
        {
            Items = new List<StorageItem>();
        }
    }
}
