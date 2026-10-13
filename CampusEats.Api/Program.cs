using CampusEats.Api.Data;
using CampusEats.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load optional local settings for development secrets (git-ignored)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL DbContext with connection string from environment or config
var connectionString = builder.Configuration.GetConnectionString("Default");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<CampusEatsDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Register MenuService as Scoped for DbContext dependency injection
builder.Services.AddScoped<IMenuService, MenuService>();

// Configure CORS for deployed frontend and local development (Task 03)
builder.Services.AddCors(o => o.AddPolicy("Frontend", p =>
    p.WithOrigins(
        "https://campuseats-web.vercel.app",
        "https://campuseat-dashboard.vercel.app",
        "http://localhost:5173",
        "http://localhost:3000"
    )
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(origin => true)
));

var app = builder.Build();

// Automatically create database, tables, and apply seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetService<CampusEatsDbContext>();
        if (db != null)
        {
            logger.LogInformation("Checking and automatically creating PostgreSQL database and tables...");
            db.Database.EnsureCreated();
            logger.LogInformation("PostgreSQL database and tables ready!");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Automatic database creation encountered an error. Verify PostgreSQL server connection.");
    }
}

// Configure Swagger (available for testing API in cloud and local dev)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CampusEats API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

// Enable CORS before MapControllers (Task 03)
app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
