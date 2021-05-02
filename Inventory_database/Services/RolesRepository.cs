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
    public class RolesRepository : IRepository<Role>
    {
        public RolesRepository(IServiceProvider provider)
        {
            Provider = provider;
        }

        public IServiceProvider Provider { get; }

        public async Task Add(Role item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                await Context.Roles.AddAsync(item);
                await Context.SaveChangesAsync();
            }
        }

        public async Task<bool> Any()
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                return await Context.Roles.AnyAsync();
            }
        }

        public async Task<bool> Any(Func<Role, bool> predicate)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                return await Context.Roles.AnyAsync(i => predicate(i));
            }
        }

        public async Task<Role> Get(int id)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                IQueryable<Role> roles = Context.Roles.Include(r => r.Users);
                var item = await roles.AsNoTracking().FirstAsync(r => r.Id == id);
                return item;
            }
        }

        public async Task<List<Role>> GetAll()
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                IQueryable<Role> roles = Context.Roles.Include(r => r.Users);
                return await roles.AsNoTracking().ToListAsync();
            }
        }

        public async Task Remove(Role item)
        {
            using (var scope = Provider.CreateScope())
            {
                var Context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                var del = await Context.Roles.FirstAsync(r => r.Id == item.Id);
                Context.Roles.Remove(del);
                await Context.SaveChangesAsync();
            }
        }
    }
}
