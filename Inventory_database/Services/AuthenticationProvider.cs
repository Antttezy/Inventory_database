using Inventory_database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        IServiceProvider Services { get; }
        public IHashingProvider Hasher { get; }
        Dictionary<string, int> Tokens { get; }
        Dictionary<string, DateTime> Expiration { get; }

        object dictLock = new object();

        public AuthenticationProvider(IServiceProvider provider, IHashingProvider hasher)
        {
            Services = provider;
            Hasher = hasher;
            Tokens = new Dictionary<string, int>();
            Expiration = new Dictionary<string, DateTime>();
        }

        protected string GenToken(int id, string password)
        {
            string data = "{" + $"\"Id\": \"{id}\", \"pass\"=\"{password}\", \"time\"=\"{DateTime.UtcNow}\"" + "}";
            string token = Hasher.Hash(data);
            return token;
        }

        public async Task<string> GenerateTokenAsync(string login, string password)
        {
            return await LoginAsync(login, password, TimeSpan.FromDays(30));
        }

        public async Task<User> GetUserByTokenAsync(string token)
        {
            if (token != null && Tokens.ContainsKey(token))
            {
                if (Expiration[token] < DateTime.UtcNow)
                {
                    await LogoutAsync(token);
                    return null;
                }

                using var scope = Services.CreateScope();
                var source = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

                var user = await source.Get(Tokens[token]);
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> IsLoginPasswordCorrect(string login, string password)
        {
            using var scope = Services.CreateScope();
            var source = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

            string hash = Hasher.Hash(password);
            var user = await source.GetAll()
                .FirstOrDefaultAsync(u =>
                u.Username == login &&
                u.PasswordHash == Hasher.Hash(password));

            return user != null;
        }

        public async Task<string> LoginAsync(string login, string password, TimeSpan StoreTime)
        {
            using var scope = Services.CreateScope();
            var source = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

            string hash = Hasher.Hash(password);
            var user = await source.GetAll()
                .FirstOrDefaultAsync(u =>
                u.Username == login &&
                u.PasswordHash == Hasher.Hash(password));

            if (user != null)
            {
                var token = GenToken(user.Id, password);

                lock (dictLock)
                {
                    Tokens.Add(token, user.Id);
                    Expiration.Add(token, DateTime.UtcNow + StoreTime);
                }

                return token;
            }
            else
            {
                return null;
            }
        }

        public Task LogoutAsync(string token)
        {
            if (token != null && Tokens.ContainsKey(token))
            {
                lock (dictLock)
                {
                    Tokens.Remove(token);
                    Expiration.Remove(token);
                }
            }

            return Task.CompletedTask;
        }

        public async Task LogoutFromAllSessionsAsync(string token)
        {
            if (token != null && Tokens.ContainsKey(token))
            {
                int id = Tokens[token];

                foreach (var pair in Tokens)
                {
                    if (pair.Value == id)
                    {
                        await LogoutAsync(pair.Key);
                    }
                }
            }
        }
    }
}
