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
        Dictionary<string, int> Tokens { get; }                     //Сопоставление токена с id пользователя
        Dictionary<string, DateTime> Expiration { get; }            //Сопоставление токена с временем окончания действия

        object dictLock = new object();                             //Объект для блокировки многопоточной записи в словари

        public AuthenticationProvider(IServiceProvider provider, IHashingProvider hasher)
        {
            Services = provider;
            Hasher = hasher;
            Tokens = new Dictionary<string, int>();
            Expiration = new Dictionary<string, DateTime>();
        }

        protected string GenToken(int id, string password) //Генерация токена из пароля и id пользователя
        {
            string data = "{" + $"\"Id\": \"{id}\", \"pass\"=\"{password}\", \"time\"=\"{DateTime.UtcNow}\"" + "}";
            string token = Hasher.Hash(data);
            return token;
        }

        public async Task<string> GenerateTokenAsync(string login, string password)
        {
            return await LoginAsync(login, password, TimeSpan.FromDays(30)); //Генерируем токен с временем жизни 30 дней
        }

        public async Task<User> GetUserByTokenAsync(string token)
        {
            if (token != null && Tokens.ContainsKey(token))             //Если токен не null и есть в списке токенов
            {
                if (Expiration[token] < DateTime.UtcNow)                //Проверка на действительность
                {
                    await LogoutAsync(token);
                    return null;
                }

                using var scope = Services.CreateScope();                                   //Время жизни scope
                var source = scope.ServiceProvider.GetRequiredService<IRepository<User>>(); //Запрос scoped объекта

                var user = await source.Get(Tokens[token]);                                 //Получение из репозитория пользователей нужного
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

            string hash = Hasher.Hash(password);            //Получение хэша пароля
            var user = await source.GetAll()
                .FirstOrDefaultAsync(u =>
                u.Username == login &&
                u.PasswordHash == Hasher.Hash(password));   //Поиск пользователя с нужным логином и паролем

            return user != null;                            //Если вернулось null, значит данные для входа не верны
        }

        public async Task<string> LoginAsync(string login, string password, TimeSpan StoreTime)
        {
            using var scope = Services.CreateScope();
            var source = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

            string hash = Hasher.Hash(password);                    //Хеш пароля
            var user = await source.GetAll()
                .FirstOrDefaultAsync(u =>
                u.Username == login &&
                u.PasswordHash == Hasher.Hash(password));           //Поиск пользователя по логину и паролю

            if (user != null)                                       //Если такой пользователь есть
            {
                var token = GenToken(user.Id, password);            //Генерация токена

                lock (dictLock)                                     //Блокировка доступа и запись в словари
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
            if (token != null && Tokens.ContainsKey(token)) //Проверка на существование токена в системе
            {
                lock (dictLock)                             //Блокировка доступа и удаление
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
                int id = Tokens[token];                 //Получение id пользователя для токена

                foreach (var pair in Tokens)            //Проходимся по всем записям
                {
                    if (pair.Value == id)
                    {
                        await LogoutAsync(pair.Key);    //Выполняем выход для всех токенов, id пользователя которых соответствует id пользователя переданного токена
                    }
                }
            }
        }
    }
}
