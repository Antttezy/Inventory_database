using System.Linq;

namespace Inventory_database.Services
{
    public class SHA1HashingProvider : IHashingProvider
    {
        public SHA1HashingProvider(StringToByteArrayConverter converter)
        {
            Converter = converter;
        }

        public StringToByteArrayConverter Converter { get; }

        public string Hash(string message)
        {
            using (var hasher = System.Security.Cryptography.SHA1.Create())
            {
                return string.Join("", hasher.ComputeHash(Converter.Convert(message)).Select(b => b.ToString("X2")));
            }
        }
    }
}
