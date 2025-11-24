var builder = WebApplication.CreateBuilder(args);

// Add HttpClient for external API calls
builder.Services.AddHttpClient<IProductService, ProductService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
