using Microsoft.IdentityModel.Tokens;
using System.Text;
using Lek8LarBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Lek8LarBackend.Services.MathGames.LevelOne;

var builder = WebApplication.CreateBuilder(args);

// === Databas (PostgreSQL) ===
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// === JWT-inst�llningar ===
var jwtKey = builder.Configuration["jwtKey"] ?? "superhemlignyckelsom�rmyckets�kerochl�ng123456";

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

// === CORS f�r frontend (Vercel) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins("https://lek-lar-app.vercel.app", "http://localhost:5173") 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});


// === Tj�nster och controllers ===
builder.Services.AddControllers();
builder.Services.AddScoped<CountGameService>();
builder.Services.AddScoped<ShapeGameService>();

// === Portinst�llning f�r Render ===
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



