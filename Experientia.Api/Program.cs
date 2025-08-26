using Experientia.Api.Data;
using Experientia.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

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

var jwt = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(t => (t.FullName ?? t.Name).Replace('+', '.'));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {

        builder.WithOrigins("http://localhost:5173", "http://localhost:4200")
               .AllowAnyMethod()
               .AllowCredentials()
               .AllowAnyHeader();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
