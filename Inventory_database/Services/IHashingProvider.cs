namespace Inventory_database.Services
{
    public interface IHashingProvider
    {
        string Hash(string message);
    }
}
