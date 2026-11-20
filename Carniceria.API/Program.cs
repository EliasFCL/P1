//Repositorio consultado para adaptar el MVC al RESTful: https://github.com/AlanRacic/MovieManagementAPI#1

using AccesoDatos.Contexto.AccesoDatos.Contexto;
using AccesoDatos.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        //Desactiva la validación automática en inglés
        options.SuppressModelStateInvalidFilter = true;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Registrar repositorio de clientes (sin esto salta error 505)
string connectionString = builder.Configuration.GetConnectionString("CarniceriaDb");
builder.Services.AddScoped<AccesoDatos.Repositorios.ClienteRepositorio>(provider => new AccesoDatos.Repositorios.ClienteRepositorio(new AccesoDatos.Contexto.ClienteDbContext()));
builder.Services.AddScoped<AccesoDatos.Repositorios.EmpleadoRepositorio>(provider => new AccesoDatos.Repositorios.EmpleadoRepositorio(new AccesoDatos.Contexto.EmpleadoDbContext()));
builder.Services.AddScoped<AccesoDatos.Repositorios.ProveedorRepositorio>(provider => new AccesoDatos.Repositorios.ProveedorRepositorio(new AccesoDatos.Contexto.ProveedorDbContext()));
builder.Services.AddScoped<AccesoDatos.Repositorios.ProductoRepositorio>(provider => new AccesoDatos.Repositorios.ProductoRepositorio(new AccesoDatos.Contexto.ProductoDbContext()));
builder.Services.AddDbContext<FacturaDbContexto>();

builder.Services.AddScoped<FacturaRepositorio>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseAuthorization();

app.MapControllers();

app.Run();
