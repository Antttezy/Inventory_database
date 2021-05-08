using Inventory_database.Models;
using System;
using System.Threading.Tasks;

namespace Inventory_database.Services
{

    /// <summary>
    /// Сервис для аутентификации пользователей
    /// </summary>
    public interface IAuthenticationProvider
    {

        /// <summary>
        /// Проверка правильности логина и пароля без генерации токена доступа
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>true, если логин и пароль правильные</returns>
        Task<bool> IsLoginPasswordCorrect(string login, string password);

        /// <summary>
        /// Получение модели пользователя по токену
        /// </summary>
        /// <param name="token">токен доступа</param>
        /// <returns>пользователя, для которого был сгенерирован токен или null в случае, если токен недействителен</returns>
        Task<User> GetUserByTokenAsync(string token);

        /// <summary>
        /// Генерация токена доступа с указанным временем жизни
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        /// <param name="StoreTime">время жизни токена</param>
        /// <returns>Токен доступа с указанным временем жизни</returns>
        Task<string> LoginAsync(string login, string password, TimeSpan StoreTime);

        /// <summary>
        /// Генерация токена доступа с большим временем жизни для постоянного взаимодействия с api
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        /// <returns>Токен доступа с большим временем жизни</returns>
        Task<string> GenerateTokenAsync(string login, string password);

        /// <summary>
        /// Удаляет токен из системы аутентификации
        /// </summary>
        /// <param name="token">токен</param>
        /// <returns></returns>
        Task LogoutAsync(string token);

        /// <summary>
        /// Удаляет этот токен и все другие, принадлежащие тому же пользователю
        /// </summary>
        /// <param name="token">токен доступа</param>
        /// <returns></returns>
        Task LogoutFromAllSessionsAsync(string token);

    }
}
