using Microsoft.EntityFrameworkCore;
using SkillExtractor.Api.Data;
using SkillExtractor.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register application services
builder.Services.AddScoped<IEmployeesService, EmployeesService>();
builder.Services.AddScoped<ISkillsService, SkillsService>();
builder.Services.AddScoped<ISkillExtractionService, SkillExtractionService>();
builder.Services.AddScoped<IExtractionService, ExtractionService>();

// Register AppDbContext using SQL Server with the "DefaultConnection" connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline. Enable Swagger and the Dev CORS policy only in Development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors");

    // Run EF migrations and seed default data in Development
    DataSeeder.SeedAsync(app.Services, app.Environment).GetAwaiter().GetResult();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

