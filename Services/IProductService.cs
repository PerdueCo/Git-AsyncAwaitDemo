using AsyncAwaitDemo.Models;
using System.Threading.Tasks;

public interface IProductService
{
    Task<Product> GetProductFromDatabaseAsync(int id);
    Task<TodoItem> GetTodoFromApiAsync(int id);
}
