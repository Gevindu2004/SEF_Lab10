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

// Check database connection and ensure tables exist if reachable
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetService<CampusEatsDbContext>();
        if (db != null)
        {
            if (db.Database.CanConnect())
            {
                logger.LogInformation("Connected to PostgreSQL! Ensuring database and tables exist...");
                db.Database.EnsureCreated();
                logger.LogInformation("PostgreSQL database tables ready!");
            }
            else
            {
                logger.LogWarning("PostgreSQL database is currently unreachable or password authentication failed. Using in-memory fallback for local API responses.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning("Database connection notice: {Message}. Running API with resilient fallback.", ex.Message);
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
