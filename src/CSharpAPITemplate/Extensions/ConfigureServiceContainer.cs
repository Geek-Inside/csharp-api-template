using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Mappings;
using CSharpAPITemplate.BusinessLayer.Services.Comments;
using CSharpAPITemplate.BusinessLayer.Services.Posts;
using CSharpAPITemplate.BusinessLayer.Services.Users;
using CSharpAPITemplate.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using SendGrid.Extensions.DependencyInjection;

namespace CSharpAPITemplate.Extensions
{
    public static class ConfigureServiceContainer
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUserService, UserService>();
        }
        
        public static void AddGridSend(this IServiceCollection services, IConfiguration configuration)
        {
            var apiKey = configuration.GetSection("Setup:SendGrid:ApiKey").Value;
            services.AddSendGrid(options =>
            {
                options.ApiKey = apiKey;
            });
        }

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(builder =>
            {
                builder.UseNpgsql(configuration.GetConnectionString("CORE_CONNECTION_STRING"), optionsBuilder =>
                {
                    optionsBuilder.CommandTimeout(180);
                    optionsBuilder.MigrationsAssembly("CSharpAPITemplate");
                });
                builder.EnableSensitiveDataLogging(false);
                builder.EnableDetailedErrors();
            });
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            var profiles = typeof(PostProfile).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x));
            services.AddAutoMapper(profiles.ToArray());
        }

        public static void AddSwagger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Example", Version = "v1" });
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
