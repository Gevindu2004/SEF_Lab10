using CampusEats.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Load optional local settings for development secrets (git-ignored)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddSingleton<IMenuService, MenuService>(); 

var app = builder.Build();

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

