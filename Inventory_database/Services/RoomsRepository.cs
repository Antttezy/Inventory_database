using Inventory_database.Data;
using Inventory_database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    public class RoomsRepository : IRepository<Room>
    {
        public RoomsRepository(IServiceProvider provider)
        {
            Provider = provider;
        }

        public IServiceProvider Provider { get; }

        public async Task Add(Room item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                await Context.Rooms.AddAsync(item);
                await Context.SaveChangesAsync();
            }
        }

        public async Task<Room> Get(int id)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                IQueryable<Room> rooms = Context.Rooms.Include(r => r.Items);
                var item = await rooms.AsNoTracking().FirstAsync(i => i.Id == id);
                return item;
            }
        }

        public async Task<List<Room>> GetAll()
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                IQueryable<Room> types = Context.Rooms.Include(r => r.Items);
                return await types.AsNoTracking().ToListAsync();
            }
        }

        public async Task Remove(Room item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                var del = await Context.Rooms.FirstAsync(i => i.Id == item.Id);
                Context.Rooms.Remove(del);
                await Context.SaveChangesAsync();
            }
        }
    }
}
