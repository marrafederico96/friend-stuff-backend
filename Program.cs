using System.Security.Cryptography;
using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.Account;
using FriendStuffBackend.Features.Account.Token;
using FriendStuffBackend.Features.UserEvent;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Load public key
var rsa = RSA.Create();
const string publicKeyPath = "etc/secrets/public_key";
var publicKey = await File.ReadAllTextAsync(publicKeyPath);
rsa.ImportFromPem(publicKey);
var rsaSecurityKey = new RsaSecurityKey(rsa);

//CORS origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") 
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
});

//Authentication JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = rsaSecurityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database Context
builder.Services.AddDbContext<FriendStuffDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});


// My services
builder.Services.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEventService, EventService>();

var app = builder.Build();

app.UseCors("AllowAngularApp");

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
