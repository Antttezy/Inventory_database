using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        Task<T> Get(int id);
        Task Add(T item);
        Task Remove(T item);
        Task Update(T item);
    }
}
