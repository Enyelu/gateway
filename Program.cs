using gateway.api.Extensions;
using gateway.api.Persistence.Database;
using gateway.api.Shared;
using gateway.api.Utilities.Token.Implementation;
using gateway.api.Utilities.Token.Interface;
using Microsoft.EntityFrameworkCore;
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
builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.AddMediatR(m => m.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Use YARP middleware
app.MapReverseProxy();

app.Run();