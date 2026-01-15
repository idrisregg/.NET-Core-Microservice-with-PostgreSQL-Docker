using FluentValidation.AspNetCore;
using Wajeb.API.Data;
using Wajeb.API.Models;
using Wajeb.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();

// register DbContext using DefaultConnection
builder.Services.AddDbContext<WajebDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// configure JWT authentication & authorization (if key present)
var jwtKey = builder.Configuration["AppSettings:Jwt"] ?? builder.Configuration["AppSettings:Key"];

if (!string.IsNullOrEmpty(jwtKey))
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();
}

var app = builder.Build();

app.MapGet("/", () => "NoNe..");
app.UseHttpsRedirection();

// enable authentication/authorization middleware if configured
if (!string.IsNullOrEmpty(jwtKey))
{
    app.UseAuthentication();
    app.UseAuthorization();
}
else
{
    app.UseAuthorization();
}

app.MapControllers();

app.Run();