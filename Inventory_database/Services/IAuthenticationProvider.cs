using Inventory_database.Models;
using System;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    public interface IAuthenticationProvider
    {
        Task<bool> IsLoginPasswordCorrect(string login, string password);

        Task<User> GetUserByTokenAsync(string token);

        Task<string> LoginAsync(string login, string password, TimeSpan StoreTime);

        Task<string> GenerateTokenAsync(string login, string password);

        Task LogoutAsync(string token);

        Task LogoutFromAllSessionsAsync(string token);

    }
}
