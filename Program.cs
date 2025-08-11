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
using DotNetEnv;
using FriendStuffBackend.Features.EventMessage;
using FriendStuffBackend.Features.ExpenseEvent;
using FriendStuffBackend.Features.ExpenseEventRefund;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
var rsa = RSA.Create();
var publicKey = Environment.GetEnvironmentVariable("PUBLIC_KEY");

rsa.ImportFromPem(publicKey);
var rsaSecurityKey = new RsaSecurityKey(rsa);

//CORS origin
builder.Services.AddCors(options =>
{
   options.AddPolicy("AllowAngularApp",
        policy =>
       {
           policy.WithOrigins(Environment.GetEnvironmentVariable("URL") ?? throw new InvalidOperationException())
              .AllowAnyHeader()
             .AllowCredentials()
            .AllowAnyMethod();
   });
});

//Authentication JWT
var issuer = Environment.GetEnvironmentVariable(("JWT_ISSUER"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = rsaSecurityKey,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            ValidateLifetime = true,
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

builder.Services.AddSignalR();

// My services
builder.Services.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IExpenseEventRefundService, ExpenseEventRefundService>();
builder.Services.AddScoped<IEventMessageService, EventMessageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

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
app.MapHub<MessageHub>("/messageHub");
app.Run();
