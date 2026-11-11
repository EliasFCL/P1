//Repositorio consultado para adaptar el MVC al RESTful: https://github.com/AlanRacic/MovieManagementAPI#1

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

//Registrar repositorio de clientes 
string connectionString = builder.Configuration.GetConnectionString("CarniceriaDb");
builder.Services.AddScoped<AccesoDatos.Repositorios.ClienteRepositorio>(provider => new AccesoDatos.Repositorios.ClienteRepositorio(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseAuthorization();

app.MapControllers();

app.Run();
