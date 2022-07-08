using CSharpAPITemplate.Data;
using CSharpAPITemplate.Extensions;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddGridSend(builder.Configuration);

builder.Services.AddServices();

builder.Services.AddDbContext(builder.Configuration);

builder.Services.AddAutoMapper();

builder.Services.AddSwagger();

builder.Services.AddVersioning();

builder.Services.AddHealthCheck(builder.Configuration);


// Configure the HTTP request pipeline.
var app = builder.Build();

app.SyncMigrations<ApplicationDbContext>();

if (app.Environment.IsDevelopment())
	app.ConfigureSwagger();

if(app.Environment.IsProduction())
	app.UseHttpsRedirection();

app.ConfigureMiddlewares();

app.UseHealthCheck();

app.MapControllers();

app.UseCors(policyBuilder =>
{
	policyBuilder.AllowAnyOrigin();
	policyBuilder.AllowAnyMethod();
	policyBuilder.AllowAnyHeader();
});

app.Run();