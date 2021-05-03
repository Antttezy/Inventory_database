using Inventory_database.Models;
using Inventory_database.Services;
using System.Linq;

namespace Inventory_database.Util
{
    public static class DBInitializer
    {

        public static void Seed(IRepository<User> users, IRepository<Role> roles, IHashingProvider provider)
        {
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
            }
        }
    }
}
