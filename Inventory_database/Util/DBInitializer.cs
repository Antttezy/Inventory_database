using Inventory_database.Models;
using Inventory_database.Services;
using System.Linq;

namespace Inventory_database.Util
{
    public static class DBInitializer
    {

        public static void Seed(IRepository<User> users, IRepository<Role> roles, StringToByteArrayConverter converter)
        {
            if (!users.GetAll().Result.Any() && !roles.GetAll().Result.Any())
            {
                using (var hasher = System.Security.Cryptography.SHA1.Create())
                {
                    User Admin = new User
                    {
                        FirstName = "Администратор",
                        SecondName = "Администратор",
                        ThirdName = "Администратор",
                        Username = "Admin",
                        PasswordHash = string.Join("", hasher.ComputeHash(converter.Convert("admin")).Select(b => b.ToString("X2")))
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
