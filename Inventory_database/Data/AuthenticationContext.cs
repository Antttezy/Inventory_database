using Inventory_database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
