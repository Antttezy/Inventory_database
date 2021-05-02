using Inventory_database.Data;
using Inventory_database.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inventory_database.Services
{
    public class TypesRepository : IRepository<ItemType>
    {
        public TypesRepository(IServiceProvider provider)
        {
            Provider = provider;
        }

        public IServiceProvider Provider { get; }

        public async Task Add(ItemType item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                await Context.ItemTypes.AddAsync(item);
                await Context.SaveChangesAsync();
            }
        }

        public async Task<bool> Any()
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                return await Context.ItemTypes.AnyAsync();
            }
        }

        public async Task<bool> Any(Func<ItemType, bool> predicate)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                return await Context.ItemTypes.AnyAsync(i => predicate(i));
            }
        }

        public async Task<ItemType> Get(int id)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                IQueryable<ItemType> types = Context.ItemTypes;
                var item = await types.AsNoTracking().FirstAsync(i => i.Id == id);
                return item;
            }
        }

        public async Task<List<ItemType>> GetAll()
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                IQueryable<ItemType> types = Context.ItemTypes;
                return await types.AsNoTracking().ToListAsync();
            }
        }

        public async Task Remove(ItemType item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                var del = await Context.ItemTypes.FirstAsync(i => i.Id == item.Id);
                Context.ItemTypes.Remove(del);
                await Context.SaveChangesAsync();
            }
        }
    }
}
