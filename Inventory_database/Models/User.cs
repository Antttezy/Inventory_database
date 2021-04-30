using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_database.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string SecondName { get; set; }

        [Required]
        public string ThirdName { get; set; }

        public string PasswordHash { get; set; }

        public ICollection<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
        }
    }
}
