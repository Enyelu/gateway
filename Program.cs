using gateway.api.Extensions;
using gateway.api.Middlewares;
using gateway.api.Persistence.Database;
using gateway.api.Shared;
using gateway.api.Utilities.Token.Implementation;
using gateway.api.Utilities.Token.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext and Identity
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.ConfigureIdentity();

builder.Services.AddMediatR(m => m.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

//Configure logger
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .WriteTo.Seq(context.Configuration["Serilog:WriteTo:0:Args:serverUrl"]));

Log.Information("Application is starting - App Gateway");

// Add YARP services
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureExceptionHandler();
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Use YARP middleware
app.MapReverseProxy();

app.Run();