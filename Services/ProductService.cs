using AsyncAwaitDemo.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class ProductService : IProductService
{
    private readonly HttpClient _http;

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    // Simulated database call (async)
    public async Task<Product> GetProductFromDatabaseAsync(int id)
    {
        // Simulate DB delay
        await Task.Delay(500);

        return new Product
        {
            Id = id,
            Name = $"Product {id}",
            Price = 49.99m
        };
    }

    public async Task<TodoItem> GetTodoFromApiAsync(int id)
    {
        return await _http.GetFromJsonAsync<TodoItem>(
            $"https://jsonplaceholder.typicode.com/todos/{id}"
        );
    }

}
