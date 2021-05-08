using Inventory_database.Models;
using Inventory_database.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Inventory_database.Util
{
    public static class DBInitializer
    {

        /// <summary>
        /// Инициализирует базу данных
        /// </summary>
        /// <param name="users">репозиторий пользователей</param>
        /// <param name="roles">репозиторий ролей</param>
        /// <param name="provider">сервис вычислений хэша</param>
        /// <param name="logger">логгер</param>
        public static void Seed(IRepository<User> users, IRepository<Role> roles, IHashingProvider provider, ILogger logger)
        {
            logger.LogInformation("Database seeder started");

            if (!(users.GetAll().Any() || roles.GetAll().Any()))
            {
                using (var hasher = System.Security.Cryptography.SHA1.Create())
                {
                    User Admin = new User
                    {
                        FirstName = "Администратор",
                        SecondName = "Администратор",
                        ThirdName = "Администратор",
                        Username = "Admin",
                        PasswordHash = provider.Hash("admin")
                    };

                    Role administrator = new Role
                    {
                        Name = "Администратор"
                    };

                    Role user = new Role
                    {
                        Name = "Пользователь"
                    };

                    Admin.Roles.Add(administrator);
                    Admin.Roles.Add(user);
                    administrator.Users.Add(Admin);
                    user.Users.Add(Admin);

                    users.Add(Admin).Wait();
                }

                logger.LogInformation("Database seeded");
            }
            else
            {
                logger.LogInformation("Database is already seeded. Skipping this part");
            }
        }
    }
}
