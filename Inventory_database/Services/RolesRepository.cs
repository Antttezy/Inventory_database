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
    public class RolesRepository : IRepository<Role>, IDisposable
    {
        AuthenticationContext Context { get; }
        IServiceScope Scope { get; }

        public RolesRepository(IServiceProvider provider)
        {
            Scope = provider.CreateScope();
            Context = Scope.ServiceProvider.GetRequiredService<AuthenticationContext>();
        }

        public async Task Add(Role item)
        {
            await Context.Roles.AddAsync(item);
            await Context.SaveChangesAsync();
        }

        public async Task<Role> Get(int id)
        {
            IQueryable<Role> roles = Context.Roles.Include(r => r.Users);
            var item = await roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            return item;
        }

        public IQueryable<Role> GetAll()
        {
            IQueryable<Role> roles = Context.Roles.Include(r => r.Users);
            return roles.AsNoTracking();
        }

        public async Task Remove(Role item)
        {
            var del = await Context.Roles.FirstAsync(r => r.Id == item.Id);
            Context.Roles.Remove(del);
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Scope.Dispose();
        }
    }
}
