using ConstructionManagement.BLL.Services;
using ConstructionManagement.DAL.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<SiteService>();
builder.Services.AddScoped<ContractorService>();
builder.Services.AddScoped<DailyWorkSubmissionService>();
builder.Services.AddScoped<SiteUserAccessService>();
builder.Services.AddScoped<WorkApprovalService>();
builder.Services.AddScoped<MaterialDemandService>();
builder.Services.AddScoped<MaterialDeliveryService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ConstructionDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuthService>();

var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("JWT key must be at least 32 bytes.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();
var hasher = new PasswordHasher<object>();

string generatedHash = hasher.HashPassword(
    new object(),
    "Builder@123"
);

Console.WriteLine("Generated Hash: " + generatedHash);

Console.WriteLine("Generated Hash: " + generatedHash);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();