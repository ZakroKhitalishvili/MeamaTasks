using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Meama_Tasks.Extensions;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerServices();

builder.Services.AddDbContext<TaskDbContext>(efOptions => efOptions.UseSqlite(builder.Configuration.GetConnectionString("TaskDbContext"), b => b.MigrationsAssembly("Infrastructure")));

builder.Services.AddAppIdentity();
builder.Services.AddAppServices();
builder.Services.AddAppAuth(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetService<RoleManager<AppRole>>();

    await DataSeeder.Seed(userManager!, roleManager!);
}

app.Run();
