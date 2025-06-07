using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SecureWebApp.Data;
using SecureWebApp.Models;
using SecureWebApp.backend.Services;
using System;
using OtpNet;
using System.Threading.RateLimiting;
using SecureWebApp.Services;
using Microsoft.OpenApi.Models;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("SecureAppDB"));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddEndpointsApiExplorer();
// Enable Swagger JWT Bearer Auth
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SecureWebApp API", Version = "v1" });

    // Add JWT Auth support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:3000") // React default port
            .AllowAnyMethod()
            .AllowAnyHeader());
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "secureapi",
            ValidAudience = "secureapi",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("ThisIsMySuperSecureKey32BytesLong1234")) // Exactly 32 bytes
        };
    });
builder.Services.AddScoped<IMfaService, TotpMfaService>();
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("api", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers["X-Client-Id"].ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});
builder.Services.AddScoped<IMfaService, TotpMfaService>();


var app = builder.Build();
app.Use(async (context, next) =>
{
    // Security headers
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("Permissions-Policy",
        "geolocation=(), microphone=(), camera=(), payment=()");

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/register", async (RegisterDto dto, IAuthService service) =>
{
    var result = await service.Register(dto);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

app.MapPost("/login", async (LoginDto dto, IAuthService service) =>
{
    var result = await service.Login(dto);
    return result.Success ? Results.Ok(result) : Results.Unauthorized();
});
app.MapPost("/mfa/setup",
    [Authorize]
async (IMfaService mfaService, HttpContext context) =>
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = context.User.FindFirstValue(ClaimTypes.Email);
        var setupCode = await mfaService.GenerateSetupCode(userId, email);
        return Results.Ok(new { setupCode });
    })
    .WithName("SetupMfa")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Generate MFA setup code",
        Description = "Returns a QR code data URL for MFA setup."
    }).RequireAuthorization(); // Ensures auth is required



app.MapGet("/admin", [Authorize(Policy = "Admin")] () => "Welcome Admin!");
app.MapGet("/user", [Authorize(Policy = "User")] () => "Welcome User!");
app.UseRateLimiter();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");

app.Run();
