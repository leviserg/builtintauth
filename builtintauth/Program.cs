using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var dbLogin = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? String.Empty;
var dbPass = Environment.GetEnvironmentVariable("RABBITMQ_USERPASS") ?? String.Empty;
var connectionString = string.Format(builder.Configuration.GetConnectionString("AppIdentity") ?? string.Empty, dbLogin, dbPass);

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
var dataSource = dataSourceBuilder.Build();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
    //options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
    //options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
.AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<AppIdentityDbContext>(options => {
    options.UseNpgsql(dataSource);
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
     .AddEntityFrameworkStores<AppIdentityDbContext>()
     .AddDefaultTokenProviders()
     .AddApiEndpoints();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


class AppIdentityDbContext: IdentityDbContext
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }
}