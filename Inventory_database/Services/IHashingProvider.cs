namespace Inventory_database.Services
{
    public interface IHashingProvider
    {

        /// <summary>
        /// Хэширует строку и возвращает строчную запись хэша
        /// </summary>
        /// <param name="message">строка для которой надо найти хэш</param>
        /// <returns>хэш в строчном формате</returns>
        string Hash(string message);
    }
}
