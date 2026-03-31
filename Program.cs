using System.Text;
using FriendStuff.Data;
using FriendStuff.Domain.Entities;
using FriendStuff.Features.Activities.Services;
using FriendStuff.Features.Auth.DTOs;
using FriendStuff.Features.Auth.Services;
using FriendStuff.Features.Expenses.Services;
using FriendStuff.Shared.Results;
using FriendStuff.Shared.Results.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext service
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found");
builder.Services.AddDbContext<FriendStuffDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Add CORS origin Angular App

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://192.168.1.244:4200")
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .AllowAnyMethod();
        });
});
// Add JWT bearer token and settings
builder.Services.Configure<TokenSettings>(builder.Configuration.GetRequiredSection("TokenSettings"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var tokenSettings = builder.Configuration
        .GetRequiredSection("TokenSettings")
        .Get<TokenSettings>() ?? throw new InvalidOperationException("Token settings not found");

    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = tokenSettings.Issuer,
        ValidAudience = tokenSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey))
    };
});

// My services
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var validationErrors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                );

            var message = string.Join(" ", validationErrors
                .SelectMany(kvp => kvp.Value));

            var error = new Error
            {
                Title = "Validation failed",
                Message = message,
                Type = ErrorType.Validation,
            };

            return new UnprocessableEntityObjectResult(error);
        };
    });

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseCors("AngularApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
