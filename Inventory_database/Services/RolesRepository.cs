using Inventory_database.Data;
using Inventory_database.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{

    /// <summary>
    /// Репозиторий ролей пользователей
    /// </summary>
    public class RolesRepository : IRepository<Role>
    {
        AuthenticationContext Context { get; }

        public RolesRepository(AuthenticationContext context)
        {
            Context = context;
        }

        public async Task Add(Role item)
        {
            await Context.Roles.AddAsync(item);
            await Context.SaveChangesAsync();
        }

        public async Task<Role> Get(int id)
        {
            IQueryable<Role> roles = Context.Roles.Include(r => r.Users);
            var item = await roles.FirstOrDefaultAsync(r => r.Id == id);
            return item;
        }

        public IQueryable<Role> GetAll()
        {
            IQueryable<Role> roles = Context.Roles.Include(r => r.Users);
            return roles;
        }

        public async Task Remove(Role item)
        {
            var del = await Context.Roles.FirstAsync(r => r.Id == item.Id);
            Context.Roles.Remove(del);
            await Context.SaveChangesAsync();
        }

        public async Task Update(Role item)
        {
            Context.Update(item);
            await Context.SaveChangesAsync();
        }
    }
}
