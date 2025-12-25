using hrms_api.Data;
using hrms_api.Repository.AuthRepository;
using hrms_api.Repository.EmailRepository;
using hrms_api.Repository.EmployeeRepository;
using hrms_api.Repository.RoleRepository;
using hrms_api.Repository.SystemUserRepository;
using hrms_api.Repository.UserContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DotNetEnv;
using hrms_api.Helper;
using hrms_api.Repository.AttendanceRepository;
using hrms_api.Repository.LeaveRequestRepository;
using hrms_api.Repository.PermissionRepository;
using hrms_api.Services;

var builder = WebApplication.CreateBuilder(args);



Env.Load();
//Get from env file 
var config = builder.Configuration;
config["SmtpSettings:Host"] = Env.GetString("SMTP_HOST", config["SmtpSettings:Host"]);
config["SmtpSettings:Port"] = Env.GetString("SMTP_PORT", config["SmtpSettings:Port"]);
config["SmtpSettings:Username"] = Env.GetString("SMTP_USERNAME", config["SmtpSettings:Username"]);
config["SmtpSettings:Password"] = Env.GetString("SMTP_PASSWORD", config["SmtpSettings:Password"]);
config["SmtpSettings:FromEmail"] = Env.GetString("SMTP_FROM", config["SmtpSettings:FromEmail"]);
config["SmtpSettings:EnableSsl"] = Env.GetString("SMTP_ENABLE_SSL", config["SmtpSettings:EnableSsl"]);

var connectionString = Env.GetString("DB_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing. Ensure it is set in the .env file.");
}

// Override the appSettings.json connection string with the .env value
builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


builder.Services.AddScoped<QrCodeService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<ISystemUserRepository, SystemUserRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IPermissionRepository,PermissionRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaverRequestRepository>();



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // Add Bearer token support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field. Example: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            new string[] { }
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5174",
        policy =>
        {
            policy.WithOrigins("http://localhost:5174") // Your Vue.js frontend
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});
builder.Services.AddAuthentication(options =>
{
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;}).
    AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    }
    );


var app = builder.Build();

app.Urls.Add("http://0.0.0.0:5292");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowLocalhost5174");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
