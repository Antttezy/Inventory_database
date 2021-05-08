using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{
    /// <summary>
    /// Репозиторий для хранения данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        Task<T> Get(int id);
        Task Add(T item);
        Task Remove(T item);
        Task Update(T item);
    }
}
