using project_z_backend.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure Swagger
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// MIDDLEWARES
app.UseSerilogRequestLogging();
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();

app.MapGet("greeting", () => "Hello you guy!");
app.Run();



