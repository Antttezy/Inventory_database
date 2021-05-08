using System.Text;

namespace Inventory_database.Services
{
    public class StringToByteArrayConverter
    {
        /// <summary>
        /// Конвертирует строку в набор байт в кодировке utf-8
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] Convert(string message)
        {
            return Encoding.UTF8.GetBytes(message ?? "");
        }
    }
}
