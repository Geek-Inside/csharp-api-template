using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Mappings;
using CSharpAPITemplate.BusinessLayer.Services.Comments;
using CSharpAPITemplate.BusinessLayer.Services.Posts;
using CSharpAPITemplate.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace CSharpAPITemplate.BusinessLayer.Extensions
{
    public static class ConfigureServiceContainer
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
        }
        
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(builder =>
            {
                builder.UseNpgsql(configuration.GetConnectionString("CORE_CONNECTION_STRING"), optionsBuilder =>
                {
                    optionsBuilder.CommandTimeout(180);
                    optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
                builder.EnableSensitiveDataLogging(false);
                builder.EnableDetailedErrors();
            });
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            var profiles = typeof(PostProfile).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x));
            services.AddAutoMapper(profiles.ToArray());
        }

        public static void AddSwagger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(setupAction =>
            {

                setupAction.SwaggerDoc(
                    "OpenAPI",
                    new OpenApiInfo
                    {
                        Title = "Simple API Template",
                        Version = "1",
                        Contact = new OpenApiContact()
                        {
                            Email = "maslennikovvaleriy@gmail.com",
                            Name = "Valeriy Maslennikov",
                        }
                    });

                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = $"Input your Bearer token.",
                });
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        }, new List<string>()
                    },
                });
            });

        }
        
        public static void AddVersioning(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
        }

        public static void AddHealthCheck(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>(name: "Application DB Context", failureStatus: HealthStatus.Degraded)
                .AddNpgSql(configuration.GetConnectionString("CORE_CONNECTION_STRING"));
            
            serviceCollection.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("Health Check Endpoint", "/health");
            }).AddInMemoryStorage();
        }
    }
}
