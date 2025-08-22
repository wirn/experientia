using Experientia.Api.Data;
using Experientia.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(cs, sql => sql.EnableRetryOnFailure()));

builder.Services
    .AddIdentityCore<AppUser>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.Password.RequiredLength = 4;
    })
    .AddRoles<IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Auth lägger vi på i ett senare steg när du vill logga in på riktigt
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();
app.Run();
