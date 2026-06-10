using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Store.Application.Interfaces;
using Store.Domain.Interfaces;
using Store.Infrastructure.Authentication;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.Repositories;

namespace Store.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:DefaultConnection"]
            ?? "Data Source=store.db";

        services.AddDbContext<StoreDbContext>(options =>
            options.UseSqlite(connectionString));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        var jwtKey = configuration[$"{JwtOptions.SectionName}:Key"]
            ?? throw new InvalidOperationException("JWT key is missing from configuration.");
        var jwtIssuer = configuration[$"{JwtOptions.SectionName}:Issuer"];
        var jwtAudience = configuration[$"{JwtOptions.SectionName}:Audience"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }
}
