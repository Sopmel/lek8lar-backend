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

// JWT-inställningar
var jwtKey = builder.Configuration["Jwt:Key"] ?? "superhemlignyckelsomärmycketsäkerochlång123456"; // Minst 32 tecken!
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

// CORS för Vercel frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins("https://lek-lar-app.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); // om du använder cookies eller auth header
});


// Controllers och services
builder.Services.AddControllers();
builder.Services.AddScoped<CountGameService>();
builder.Services.AddScoped<ShapeGameService>();

// Portinställning (Render)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// Swagger även i produktion om miljövariabeln är satt
if (app.Environment.IsDevelopment() || builder.Configuration["EnableSwaggerInProd"] == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();               // Routing först
app.UseCors("AllowFrontend");   // CORS direkt efter routing

// Ta bort HTTPS-redirect för Render (Render använder http://)
//// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
