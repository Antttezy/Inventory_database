using System.Collections.Generic;

namespace Inventory_database.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string ThirdName { get; set; }

        public string PasswordHash { get; set; }

        public List<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
        }
    }
}
