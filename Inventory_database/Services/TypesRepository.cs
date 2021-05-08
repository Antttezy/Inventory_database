using Inventory_database.Data;
using Inventory_database.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_database.Services
{

    /// <summary>
    /// Репозиторий категорий предметов инвентаря
    /// </summary>
    public class TypesRepository : IRepository<ItemType>
    {
        InventoryContext Context { get; }

        public TypesRepository(InventoryContext context)
        {
            Context = context;
        }

        public async Task Add(ItemType item)
        {
            await Context.ItemTypes.AddAsync(item);
            await Context.SaveChangesAsync();
        }

        public async Task<ItemType> Get(int id)
        {
            var item = await Context.ItemTypes.FindAsync(id);
            return item;
        }

        public IQueryable<ItemType> GetAll()
        {
            IQueryable<ItemType> types = Context.ItemTypes;
            return types;
        }

        public async Task Remove(ItemType item)
        {
            var del = await Context.ItemTypes.FindAsync(item.Id);
            Context.ItemTypes.Remove(del);
            await Context.SaveChangesAsync();
        }

        public async Task Update(ItemType item)
        {
            Context.Update(item);
            await Context.SaveChangesAsync();
        }
    }
}
