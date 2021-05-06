using System.Text;

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
