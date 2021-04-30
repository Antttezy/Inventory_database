using Inventory_database.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_database.Data
{
    public class AuthenticationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public AuthenticationContext(DbContextOptions options) : base(options)
        {

        }
    }
}
