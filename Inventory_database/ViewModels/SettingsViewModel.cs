using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.ViewModels
{
    public class SettingsViewModel
    {
        public ChangeNameViewModel ChangeName { get; set; }

        public ChangePasswordViewModel ChangePassword { get; set; }
    }
}
