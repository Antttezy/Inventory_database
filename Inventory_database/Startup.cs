using Inventory_database.Data;
using Inventory_database.Models;
using Inventory_database.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Inventory_database
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<InventoryContext>(opts =>
                opts.UseSqlServer(Configuration.GetConnectionString("InvContext"))
            );

            services.AddDbContext<AuthenticationContext>(opts =>
                opts.UseSqlServer(Configuration.GetConnectionString("AuthContext"))
            );

            services.AddTransient<StringToByteArrayConverter>();
            services.AddTransient<IHashingProvider, SHA1HashingProvider>();

            services.AddScoped<IRepository<StorageItem>, ItemsRepository>();
            services.AddScoped<IRepository<ItemType>, TypesRepository>();
            services.AddScoped<IRepository<Room>, RoomsRepository>();
            services.AddScoped<IRepository<User>, UsersRepository>();
            services.AddScoped<IRepository<Role>, RolesRepository>();

            services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.Map(
                    "/",
                    context =>
                    {
                        context.Response.Redirect("Home/Items/Index", true);
                        return Task.CompletedTask;
                    }
                    );

                endpoints.MapControllerRoute(
                    name: "inv",
                    pattern: "Home/{controller}/{action=Index}"
                    );
            });
        }
    }
}
