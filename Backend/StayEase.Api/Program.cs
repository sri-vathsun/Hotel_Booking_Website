using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using System.Text;
using StayEase.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StayEase.Api.Services.Interfaces;
using StayEase.Api.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register authentication service
builder.Services.AddScoped<IAuthService, AuthService>();
// Customer Hotel services
builder.Services.AddScoped<ICustomerHotelRepository, CustomerHotelRepository>();
builder.Services.AddScoped<ICustomerHotelService, CustomerHotelService>();
builder.Services.AddScoped<
    ICustomerBookingRepository,
    CustomerBookingRepository>();

builder.Services.AddScoped<
    ICustomerBookingService,
    CustomerBookingService>();
// Customer Room services
builder.Services.AddScoped<ICustomerRoomRepository, CustomerRoomRepository>();
builder.Services.AddScoped<ICustomerRoomService, CustomerRoomService>();
builder.Services.AddScoped<
    IAdminDashboardService,
    AdminDashboardService>();

// Read JWT configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSettings["Key"]
    ?? throw new InvalidOperationException("JWT Key is missing.");

var key = Encoding.UTF8.GetBytes(jwtKey);

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero
    };
});

// Enable authorization
builder.Services.AddAuthorization();
// Configure SQL Server database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
