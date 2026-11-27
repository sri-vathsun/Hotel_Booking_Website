using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StayEase.Api.Services.Interfaces;
using StayEase.Api.Services.Implementations;
using StayEase.Api.Repositories.Interfaces;
using StayEase.Api.Repositories.Implementations;
using StayEase.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<
    IAdminDashboardService,
    AdminDashboardService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IFacilityRepository, FacilityRepository>();
builder.Services.AddScoped<IAdminManagementService, AdminManagementService>();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:4200",
                "https://localhost:3000",
                "https://localhost:5173",
                "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

app.UseCors("Frontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Ensure Categories table and CategoryId column exist in database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
            BEGIN
                CREATE TABLE [dbo].[Categories] (
                    [CategoryId] INT IDENTITY(1,1) NOT NULL,
                    [CategoryName] NVARCHAR(100) NOT NULL,
                    [Description] NVARCHAR(250) NULL,
                    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([CategoryId] ASC)
                );
                CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories_CategoryName] ON [dbo].[Categories] ([CategoryName] ASC);
            END;

            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Hotels]') AND name = 'CategoryId')
            BEGIN
                ALTER TABLE [dbo].[Hotels] ADD [CategoryId] INT NULL;
                CREATE NONCLUSTERED INDEX [IX_Hotels_CategoryId] ON [dbo].[Hotels] ([CategoryId] ASC);
                ALTER TABLE [dbo].[Hotels] ADD CONSTRAINT [FK_Hotels_Categories_CategoryId] 
                    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([CategoryId]) ON DELETE SET NULL;
            END;
        ");
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Failed to verify or apply Categories table/columns");
    }
}

app.Run();
