using Microsoft.IdentityModel.Tokens;
using System.Text;
using Lek8LarBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Lek8LarBackend.Services.MathGames.LevelOne;

var builder = WebApplication.CreateBuilder(args);

// Databas
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT-inst�llningar
var jwtKey = builder.Configuration["Jwt:Key"] ?? "superhemlignyckelsom�rmyckets�kerochl�ng123456"; // Minst 32 tecken!
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS f�r Vercel frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins("https://lek-lar-app.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Controllers och services
builder.Services.AddControllers();
builder.Services.AddScoped<CountGameService>();
builder.Services.AddScoped<ShapeGameService>();

// Portinst�llning (Render)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// Aktivera Swagger �ven i produktion om satt i milj�variabel
if (app.Environment.IsDevelopment() || builder.Configuration["EnableSwaggerInProd"] == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
