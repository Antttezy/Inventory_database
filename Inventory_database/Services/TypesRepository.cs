﻿using Inventory_database.Data;
using Inventory_database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    public class TypesRepository : IRepository<ItemType>, IDisposable
    {
        InventoryContext Context { get; }
        IServiceScope Scope { get; }

        public TypesRepository(IServiceProvider provider)
        {
            Scope = provider.CreateScope();
            Context = Scope.ServiceProvider.GetRequiredService<InventoryContext>();
        }

        public async Task Add(ItemType item)
        {
            await Context.ItemTypes.AddAsync(item);
            await Context.SaveChangesAsync();
        }

        public async Task<ItemType> Get(int id)
        {
            var item = await Context.ItemTypes.FindAsync(id);
            return item;
        }

        public IQueryable<ItemType> GetAll()
        {
            IQueryable<ItemType> types = Context.ItemTypes;
            return types;
        }

        public async Task Remove(ItemType item)
        {
            var del = await Context.ItemTypes.FindAsync(item.Id);
            Context.ItemTypes.Remove(del);
            await Context.SaveChangesAsync();
        }

        public async Task Update(ItemType item)
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
