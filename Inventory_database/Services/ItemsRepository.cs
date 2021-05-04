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
    public class ItemsRepository : IRepository<StorageItem>, IDisposable
    {
        InventoryContext Context { get; }
        IServiceScope Scope { get; }

        public ItemsRepository(IServiceProvider provider)
        {
            Scope = provider.CreateScope();
            Context = Scope.ServiceProvider.GetRequiredService<InventoryContext>();
        }

        public async Task Add(StorageItem item)
        {
            await Context.Items.AddAsync(item);
            await Context.SaveChangesAsync();
        }

        public async Task<StorageItem> Get(int id)
        {
            var item = await Context.Items.FindAsync(id);
            return item;
        }

        public IQueryable<StorageItem> GetAll()
        {
            IQueryable<StorageItem> items = Context.Items.Include(item => item.Type).Include(item => item.Room);
            return items;
        }

        public async Task Remove(StorageItem item)
        {
            var del = await Context.Items.FindAsync(item.Id);
            Context.Items.Remove(del);
            await Context.SaveChangesAsync();
        }

        public async Task Update(StorageItem item)
        {
            Context.Update(item);
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Scope.Dispose();
        }
    }
}
