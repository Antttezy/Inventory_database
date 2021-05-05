using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    public class StringToByteArrayConverter
    {
        public byte[] Convert(string message)
        {
            return Encoding.UTF8.GetBytes(message ?? "");
        }
    }
}
