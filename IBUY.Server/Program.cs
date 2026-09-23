using IBUY.Repository.Repositorios;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("ConnSqlServer")
    ?? throw new InvalidOperationException("No existe la conexión con la base de datos.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Repositorio generico: resuelve IRepositorio<Empresa>, IRepositorio<Producto>, etc.
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

// Repositorios especificos: agregan operaciones propias del dominio sobre el repositorio generico.
builder.Services.AddScoped<INecesidadRepositorio, NecesidadRepositorio>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
