using Inventory_database.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_database.Data
{
    public class InventoryContext : DbContext
    {
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<StorageItem> Items { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public InventoryContext(DbContextOptions<InventoryContext> options) : base(options)
        {

        }
    }
}
