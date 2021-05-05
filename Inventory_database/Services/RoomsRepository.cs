using Inventory_database.Data;
using Inventory_database.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    public class RoomsRepository : IRepository<Room>
    {
        InventoryContext Context { get; }

        public RoomsRepository(InventoryContext context)
        {
            Context = context;
        }

        public async Task Add(Room item)
        {
            await Context.Rooms.AddAsync(item);
            await Context.SaveChangesAsync();
        }

        public async Task<Room> Get(int id)
        {
            IQueryable<Room> rooms = Context.Rooms.Include(r => r.Items);
            var item = await rooms.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            return item;
        }

        public IQueryable<Room> GetAll()
        {
            IQueryable<Room> types = Context.Rooms.Include(r => r.Items);
            return types.AsNoTracking();
        }

        public async Task Remove(Room item)
        {
            var del = await Context.Rooms.FirstAsync(r => r.Id == item.Id);
            Context.Rooms.Remove(del);
            await Context.SaveChangesAsync();
        }

        public async Task Update(Room item)
        {
            Context.Update(item);
            await Context.SaveChangesAsync();
        }
    }
}
