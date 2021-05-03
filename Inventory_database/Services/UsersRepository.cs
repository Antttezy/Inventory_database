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
    public class UsersRepository : IRepository<User>
    {
        AuthenticationContext Context { get; }
        IServiceScope Scope { get; }

        public UsersRepository(IServiceProvider provider)
        {
            Scope = provider.CreateScope();
            Context = Scope.ServiceProvider.GetRequiredService<AuthenticationContext>();
        }

        public async Task Add(User item)
        {
            await Context.Users.AddAsync(item);
            await Context.SaveChangesAsync();
        }

        public async Task<User> Get(int id)
        {
            IQueryable<User> users = Context.Users.Include(u => u.Roles);
            var item = await users.AsNoTracking().FirstAsync(u => u.Id == id);
            return item;
        }

        public IQueryable<User> GetAll()
        {
            IQueryable<User> users = Context.Users.Include(u => u.Roles);
            return users.AsNoTracking();
        }

        public async Task Remove(User item)
        {
            var del = await Context.Users.FirstAsync(u => u.Id == item.Id);
            Context.Users.Remove(del);
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Scope.Dispose();
        }
    }
}
