using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        // Start both tasks simultaneously
        var dbTask = _service.GetProductFromDatabaseAsync(id);
        var apiTask = _service.GetTodoFromApiAsync(id);

        // Await both
        await Task.WhenAll(dbTask, apiTask);

        return Ok(new
        {
            Product = dbTask.Result,
            Todo = apiTask.Result,
            Message = "This is an example of async/await that keeps the server responsive."
        });
    }
}

