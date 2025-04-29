using StudentRegistration.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Agregar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5035); // HTTP sin HTTPS
    serverOptions.ListenAnyIP(7001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

var app = builder.Build();

// Llamar a DbInitializer para poblar datos iniciales
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

// Activar CORS antes de autorización
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
