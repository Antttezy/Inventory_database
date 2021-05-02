using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    interface IHashingProvider
    {
        string Hash(string message);
    }
}
