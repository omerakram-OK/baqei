//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using MongoDB.Driver;
//using StackExchange.Redis;
//using Baqei.Domain.Interfaces;
//using Baqei.Infrastructure.Data;
//using Baqei.Infrastructure.Repositories;
//using Baqei.Infrastructure.Mongo;
//using Baqei.Infrastructure.Redis;
//using Baqei.Infrastructure.RabbitMq;

//namespace Baqei.Infrastructure;

//public static class DependencyInjection
//{
//    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
//    {
//        // Postgres / EF Core
//        var pgConn = configuration.GetConnectionString("Postgres");
//        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(pgConn));

//        // Repositories
//        services.AddScoped<IUserRepository, UserRepository>();

//        // MongoDB
//        var mongoConn = configuration.GetConnectionString("Mongo");
//        var mongoClient = new MongoClient(mongoConn);
//        var mongoDbName = MongoUrl.Create(mongoConn).DatabaseName ?? "baqei";
//        var mongoDb = mongoClient.GetDatabase(mongoDbName);
//        services.AddSingleton<IMongoClient>(mongoClient);
//        services.AddSingleton(mongoDb);
//        services.AddScoped<UserMongoRepository>();

//        // Redis
//        var redisConn = configuration.GetConnectionString("Redis");
//        var multiplexer = ConnectionMultiplexer.Connect(redisConn);
//        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
//        services.AddScoped<RedisCacheService>();

//        // RabbitMQ
//        var rmqHost = configuration.GetValue<string>("RabbitMq:Host") ?? "localhost";
//        var rmqPort = configuration.GetValue<int?>("RabbitMq:Port") ?? 5672;
//        var rmqUser = configuration.GetValue<string>("RabbitMq:Username") ?? "guest";
//        var rmqPass = configuration.GetValue<string>("RabbitMq:Password") ?? "guest";
//        var rabbit = new RabbitMqService(rmqHost, rmqPort, rmqUser, rmqPass);
//        services.AddSingleton(rabbit);

//        return services;
//    }
//}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Baqei.Domain.Interfaces;
using Baqei.Infrastructure.Data;
using Baqei.Infrastructure.Repositories;
using Baqei.Infrastructure.Mongo;
using Baqei.Infrastructure.Redis;
using Baqei.Infrastructure.RabbitMq;

namespace Baqei.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Postgres / EF Core
        var pgConn = configuration.GetConnectionString("Postgres");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(pgConn));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPlotRepository, PlotRepository>();

        // MongoDB
        var mongoConn = configuration.GetConnectionString("Mongo");
        var mongoClient = new MongoClient(mongoConn);
        var mongoDbName = MongoUrl.Create(mongoConn).DatabaseName ?? "baqei";
        var mongoDb = mongoClient.GetDatabase(mongoDbName);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton(mongoDb);
        services.AddScoped<UserMongoRepository>();

        // Redis
        var redisConn = configuration.GetConnectionString("Redis");
        var multiplexer = ConnectionMultiplexer.Connect(redisConn);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddScoped<RedisCacheService>();

        // RabbitMQ
        var rmqHost = configuration.GetValue<string>("RabbitMq:Host") ?? "localhost";
        var rmqPort = configuration.GetValue<int?>("RabbitMq:Port") ?? 5672;
        var rmqUser = configuration.GetValue<string>("RabbitMq:Username") ?? "guest";
        var rmqPass = configuration.GetValue<string>("RabbitMq:Password") ?? "guest";
        var rabbit = new RabbitMqService(rmqHost, rmqPort, rmqUser, rmqPass);
        services.AddSingleton(rabbit);

        // JWT Authentication
        var jwtSection = configuration.GetSection("Jwt");
        var secret = jwtSection.GetValue<string>("Secret") ?? "super_secret_key_please_change";
        var issuer = jwtSection.GetValue<string>("Issuer") ?? "BaqeiIssuer";
        var audience = jwtSection.GetValue<string>("Audience") ?? "BaqeiAudience";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),

                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // 🔥 JWT Debugging Logs
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    Console.WriteLine("🔴 ERROR: " + ctx.Exception.GetType().Name);
                    Console.WriteLine("🔴 MESSAGE: " + ctx.Exception.Message);

                    if (ctx.Exception.InnerException != null)
                        Console.WriteLine("🔴 INNER: " + ctx.Exception.InnerException.Message);

                    return Task.CompletedTask;
                }
            };

        });


        return services;
    }
}

