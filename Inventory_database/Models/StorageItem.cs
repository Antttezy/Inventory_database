using System.ComponentModel.DataAnnotations;

namespace Inventory_database.Models
{
    public class StorageItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int? ItemTypeId { get; set; }

        public ItemType Type { get; set; }

        [Required]
        public int? RoomId { get; set; }

        public Room Room { get; set; }
    }
}
