using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    public interface IHashingProvider
    {
        string Hash(string message);
    }
}
