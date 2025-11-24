# Async/Await Full Demo — Complete Code Example

A comprehensive demonstration of proper async/await usage in ASP.NET Core, featuring both simulated database operations and real external HTTP API calls.

[![.NET](https://img.shields.io/badge/.NET-8.0+-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0+-512BD4)](https://docs.microsoft.com/en-us/aspnet/core/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

## 🧠 Interview Question This Answers

**"You're writing a controller action that needs to fetch data from a database and also call an external HTTP service. Why is it critical to use async and await for these I/O operations?"**

### ⭐ Short Answer

Because I/O operations take time, and **async/await prevents the server thread from being blocked**. This keeps ASP.NET Core fast, scalable, and responsive, allowing it to process many requests at once.

---

## 📁 Project Structure

```
AsyncAwaitDemo/
├── Models/
│   ├── Product.cs
│   └── TodoItem.cs
├── Services/
│   ├── IProductService.cs
│   └── ProductService.cs
├── Controllers/
│   └── AsyncDemoController.cs
└── Program.cs
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or higher
- Visual Studio 2022, VS Code, or Rider (optional)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/async-await-demo.git
   cd async-await-demo
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Test the endpoint**
   ```bash
   curl https://localhost:5001/api/AsyncDemo/combined/1
   ```

---

## 📝 Implementation

### 🧩 1. Service Interface — `IProductService.cs`

Defines the contract for asynchronous data operations.

```csharp
public interface IProductService
{
    Task<Product> GetProductFromDatabaseAsync(int id);
    Task<TodoItem> GetTodoFromApiAsync(int id);
}
```

---

### 🛠️ 2. Service Implementation — `ProductService.cs`

Simulates a database call and makes a real external HTTP API call.

```csharp
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
        await Task.Delay(500); // mimic DB latency

        return new Product
        {
            Id = id,
            Name = $"Product {id}",
            Price = 49.99m
        };
    }

    // External HTTP call
    public async Task<TodoItem> GetTodoFromApiAsync(int id)
    {
        return await _http.GetFromJsonAsync<TodoItem>(
            $"https://jsonplaceholder.typicode.com/todos/{id}"
        );
    }
}
```

**Key Points:**
- ✅ Uses `HttpClient` for HTTP calls (best practice)
- ✅ `Task.Delay()` simulates database latency
- ✅ Both methods return `Task<T>` for async operations

---

### 📦 3. Models

#### `Product.cs`
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

#### `TodoItem.cs`
```csharp
public class TodoItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public bool Completed { get; set; }
}
```

---

### 🌐 4. Controller — `AsyncDemoController.cs`

**This is the impressive part** — performs parallel async operations!

```csharp
[ApiController]
[Route("api/[controller]")]
public class AsyncDemoController : ControllerBase
{
    private readonly IProductService _service;

    public AsyncDemoController(IProductService service)
    {
        _service = service;
    }

    [HttpGet("combined/{id}")]
    public async Task<IActionResult> GetCombinedData(int id)
    {
        // Start both async tasks simultaneously
        var dbTask = _service.GetProductFromDatabaseAsync(id);
        var apiTask = _service.GetTodoFromApiAsync(id);

        // Wait for both to finish
        await Task.WhenAll(dbTask, apiTask);

        return Ok(new
        {
            Product = dbTask.Result,
            Todo = apiTask.Result,
            Message = "This is an example of async/await that keeps the server responsive."
        });
    }
}
```

**Why This Approach is Better:**
- ⚡ Runs both operations in **parallel** instead of sequential
- 📈 Reduces total response time significantly
- 🎯 Uses `Task.WhenAll()` to wait for multiple async operations

---

### ⚙️ 5. Program.cs — Register Services + HttpClient

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
```

**Important:**
- Uses `AddHttpClient<T>()` for proper HttpClient management
- Prevents socket exhaustion
- Enables dependency injection

---

## 🧪 Testing

### Test Endpoint

**URL:** `GET https://localhost:5001/api/AsyncDemo/combined/1`

**Expected Response:**

```json
{
  "product": {
    "id": 1,
    "name": "Product 1",
    "price": 49.99
  },
  "todo": {
    "userId": 1,
    "id": 1,
    "title": "delectus aut autem",
    "completed": false
  },
  "message": "This is an example of async/await that keeps the server responsive."
}
```

### Using cURL

```bash
curl -X GET "https://localhost:5001/api/AsyncDemo/combined/1" -H "accept: application/json"
```

### Using PowerShell

```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/AsyncDemo/combined/1" -Method Get
```

### Using Postman

1. Set method to **GET**
2. Enter URL: `https://localhost:5001/api/AsyncDemo/combined/1`
3. Click **Send**

---

## 🎓 Explanation for Interviews

### The Problem with Blocking Code

When your code performs database queries and HTTP calls, both operations involve **waiting**.

**If you block the thread using `.Result` or `.Wait()`:**

❌ The server thread gets stuck  
❌ ASP.NET Core has fewer threads to handle requests  
❌ The entire API slows down under load  
❌ Poor scalability

### The Solution with Async/Await

**With `async`/`await`:**

✅ The thread is **freed** during the wait  
✅ The framework can handle **more incoming requests**  
✅ Your application **scales better**  
✅ You can run tasks in **parallel** using `Task.WhenAll()`

---

## 📊 Performance Comparison

### Sequential (BAD) ❌
```csharp
// Total time: ~1000ms
var product = await GetProductFromDatabaseAsync(1);  // 500ms
var todo = await GetTodoFromApiAsync(1);             // 500ms
```

### Parallel (GOOD) ✅
```csharp
// Total time: ~500ms (both run simultaneously!)
var dbTask = GetProductFromDatabaseAsync(1);
var apiTask = GetTodoFromApiAsync(1);
await Task.WhenAll(dbTask, apiTask);
```

**Result:** ~50% faster response time! 🚀

---

## 💡 Key Takeaways

1. **Always use `async`/`await` for I/O operations** (database, HTTP, file system)
2. **Never use `.Result` or `.Wait()`** — they block threads
3. **Use `Task.WhenAll()`** for parallel operations
4. **Use `HttpClient` with dependency injection** (registered via `AddHttpClient`)
5. **Return `Task<T>` or `Task<IActionResult>`** from async methods

---

## 🔗 Additional Resources

- [Async/Await Best Practices](https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [ASP.NET Core Performance Best Practices](https://docs.microsoft.com/en-us/aspnet/core/performance/performance-best-practices)
- [HttpClient Guidelines](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📧 Contact

**Your Name** - [@yourtwitter](https://twitter.com/yourtwitter) - your.email@example.com

**Project Link:** [https://github.com/yourusername/async-await-demo](https://github.com/yourusername/async-await-demo)

---

## ⭐ Show Your Support

If this project helped you understand async/await better, give it a ⭐️!

---

**Built with ❤️ for developers learning ASP.NET Core async patterns**
