using Microsoft.EntityFrameworkCore;
using project_z_backend.Data;
using project_z_backend.Extensions;
using project_z_backend.Handlers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();
string connectionString = builder.Configuration["ConnectionStrings:Sql"]!;

// SERVICES:
builder.Services.AddSwaggerDocumentation(); // Config Swagger
builder.Services.AddDbContext<ProjectZContext>(
    options => options.UseSqlServer(connectionString) // config db connection
);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // global exception

var app = builder.Build();
// MIDDLEWARES:
app.UseSerilogRequestLogging();
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.UseExceptionHandler();

app.MapGet("greeting", () => "Hello you guy!");
app.Run();



