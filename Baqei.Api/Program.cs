using Baqei.Application.Services;
using Baqei.Infrastructure;
using Baqei.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add infrastructure & services
builder.Services.AddInfrastructure(builder.Configuration); // includes JWT
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlotService>();

builder.Services.AddControllers();

// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Baqei API", Version = "v1" });

    //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    Name = "Authorization",
    //    Type = SecuritySchemeType.ApiKey,
    //    Scheme = "Bearer",
    //    BearerFormat = "JWT",
    //    In = ParameterLocation.Header,
    //    Description = "Enter 'Bearer' followed by your JWT token"
    //});

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,       // FIXED 🔥
        Scheme = "bearer",                    // FIXED 🔥
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});

// DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Baqei API V1");
        c.RoutePrefix = string.Empty;
    });
}
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("Authorization", out var auth))
    {
        Console.WriteLine($"➡️ RAW AUTH HEADER: '{auth}'");
    }
    await next();
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// RabbitMQ consumer
var lifetime = app.Lifetime;
lifetime.ApplicationStarted.Register(() =>
{
    var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
    Task.Run(async () =>
    {
        using var scope = scopeFactory.CreateScope();
        var rabbitService = scope.ServiceProvider.GetRequiredService<Baqei.Infrastructure.RabbitMq.RabbitMqService>();
        rabbitService.StartConsumer(async (message) =>
        {
            Console.WriteLine($"[RabbitMQ] Received message: {message}");
            await Task.CompletedTask;
        }, CancellationToken.None);
    });
});

app.Run();
