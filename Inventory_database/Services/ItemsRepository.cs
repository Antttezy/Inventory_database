using Inventory_database.Data;
using Inventory_database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory_database.Services
{
    public class ItemsRepository : IRepository<StorageItem>
    {
        IServiceProvider Provider { get; }

        public ItemsRepository(IServiceProvider provider)
        {
            Provider = provider;
        }

        public async Task Add(StorageItem item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                await Context.Items.AddAsync(item);
                await Context.SaveChangesAsync();
            }
        }

        public async Task<StorageItem> Get(int id)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                IQueryable<StorageItem> items = Context.Items.Include(item => item.Type).Include(item => item.Room);
                var item = await items.AsNoTracking().FirstAsync(i => i.Id == id);
                return item;
            }
        }

        public async Task<List<StorageItem>> GetAll()
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

                IQueryable<StorageItem> items = Context.Items.Include(item => item.Type).Include(item => item.Room);
                return await items.AsNoTracking().ToListAsync();
            }
        }

        public async Task Remove(StorageItem item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<InventoryContext>();
                var del = await Context.Items.FirstAsync(i => i.Id == item.Id);
                Context.Items.Remove(del);
                await Context.SaveChangesAsync();
            }
        }
    }
}
