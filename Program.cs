using Microsoft.IdentityModel.Tokens;
using System.Text;
using Lek8LarBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Lek8LarBackend.Services.MathGames.LevelOne;
using Lek8LarBackend.Data.Entities;
using Lek8LarBackend.Services.MemoryGames;


var builder = WebApplication.CreateBuilder(args);

// === Databas (PostgreSQL) ===
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// === JWT-inställningar ===
var jwtKey = builder.Configuration["jwtKey"] ?? "superhemlignyckelsomärmycketsäkerochlång123456";

var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
builder.Services.AddSingleton(key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
});

// === Swagger ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// === CORS för frontend (Vercel) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins("https://lek-lar-app.vercel.app", "http://localhost:5173") 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});


// === Tjänster och controllers ===
builder.Services.AddControllers();
builder.Services.AddScoped<CountGameService>();
builder.Services.AddScoped<ShapeGameService>();
builder.Services.AddScoped<PlusGameService>();
builder.Services.AddScoped<MemoryGameService>();
builder.Services.AddScoped<WhatsMissingService>();
builder.Services.AddScoped<ColorMixGameService>();



// === Portinställning för Render ===
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// === Swagger aktivering ===
if (app.Environment.IsDevelopment() || builder.Configuration["EnableSwaggerInProd"] == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// === Middleware ===
app.UseRouting();
app.UseCors("AllowFrontend");
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();



