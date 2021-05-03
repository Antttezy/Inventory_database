using Inventory_database.Models;
using Inventory_database.Services;
using Inventory_database.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var users = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
                    var roles = scope.ServiceProvider.GetRequiredService<IRepository<Role>>();
                    var hasher = scope.ServiceProvider.GetRequiredService<IHashingProvider>();

                    DBInitializer.Seed(users, roles, hasher);
                }
                catch (Exception)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Cannot seed a database");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
