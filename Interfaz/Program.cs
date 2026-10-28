using Proyecto1_EliasCorderoLeon.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<IClienteService, ClienteService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5221/");
});

builder.Services.AddHttpClient<IEmpleadoService, EmpleadoService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5221/");
});

builder.Services.AddHttpClient<IProductoService, ProductoService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5221/");
});


builder.Services.AddHttpClient<IProveedorService, ProveedorService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5221/");
});

builder.Services.AddHttpClient<IFacturaService, FacturaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5221/");
});

// Add services to the container.
builder.Services.AddControllersWithViews();

//Se le dice a c# que use la cultura de Costa Rica para la aplicación
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

//Pero que mantenga los textos, fechas y demás en español de Costa Rica (esto para evitar problemas en el formato del subtotal)
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("es-CR");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Persona/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
