using IpoApp.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IpoApp.Repository;
using IpoApp.Core.Services;
using IpoApp.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using IpoApp.API;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();


// add configuration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
// Add services to the container.
builder.Services.AddControllers();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentContext, CurrentContext>();

// Register DatabaseContext with the DI container
builder.Services.AddSingleton(new IpoApp.Data.DatabaseContext(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton(new IpoApp.Repository.DatabaseContext(builder.Configuration.GetConnectionString("DefaultConnection")));

// Run migrations
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var migrationRunner = new MigrationRunner(connectionString);
migrationRunner.RunMigrations();

// In Startup.cs
builder.Services.AddScoped<IIpoMasterRepository, IpoMasterRepository>();
builder.Services.AddScoped<IIpoService, IpoService>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<ITenantOrgRepository, TenantOrgRepository>();
builder.Services.AddScoped<ITenantOrgService, TenantOrgService>();

// In Startup.cs or similar
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>(); // Your implementation

builder.Services.AddScoped<ITransactionMasterRepository, TransactionMasterRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Add to ConfigureServices
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<SwaggerSettings>(builder.Configuration.GetSection("SwaggerSettings"));
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
         ValidAudience = builder.Configuration["JwtSettings:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
     };
     // Read OrgShortCode from header and add to context
     options.Events = new JwtBearerEvents
     {
         OnTokenValidated = context =>
         {
             if (context.HttpContext.Request.Headers.TryGetValue("OrgShortCode", out var orgShortCode))
             {
                 context.HttpContext.Items["OrgShortCode"] = orgShortCode.ToString();
             }
             return Task.CompletedTask;
         }
     };
 }
 );

// Program.cs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {

        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT containing user identity and organization context"
    });

    // Add OrgShortCode header parameter globally
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


var app = builder.Build();
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IPO API V1");
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();