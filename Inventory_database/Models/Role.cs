using System.Collections.Generic;

namespace Inventory_database.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<User> Users { get; set; }

        public Role()
        {
            Users = new List<User>();
        }
    }
}
