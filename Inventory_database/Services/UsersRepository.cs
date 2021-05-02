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
        public UsersRepository(IServiceProvider provider)
        {
            Provider = provider;
        }

        public IServiceProvider Provider { get; }

        public async Task Add(User item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                await Context.Users.AddAsync(item);
                await Context.SaveChangesAsync();
            }
        }

        public async Task<bool> Any()
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                return await Context.Users.AnyAsync();
            }
        }

        public async Task<bool> Any(Func<User, bool> predicate)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                return await Context.Users.AnyAsync(i => predicate(i));
            }
        }

        public async Task<User> Get(int id)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                IQueryable<User> users = Context.Users.Include(u => u.Roles);
                var item = await users.AsNoTracking().FirstAsync(u => u.Id == id);
                return item;
            }
        }

        public async Task<List<User>> GetAll()
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                IQueryable<User> users = Context.Users.Include(u => u.Roles);
                return await users.AsNoTracking().ToListAsync();
            }
        }

        public async Task Remove(User item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                var del = await Context.Users.FirstAsync(u => u.Id == item.Id);
                Context.Users.Remove(del);
                await Context.SaveChangesAsync();
            }
        }
    }
}
