namespace Inventory_database.Models
{
    public class StorageItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ItemTypeId { get; set; }

        public ItemType Type { get; set; }

        public int? RoomId { get; set; }

        public Room Room { get; set; }
    }
}
