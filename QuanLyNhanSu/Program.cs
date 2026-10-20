using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Data;
using System.Text.Json.Serialization;
using QuanLyNhanSu.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using Serilog;
using QuanLyNhanSu.Services;
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers()
.AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler =
         ReferenceHandler.IgnoreCycles;
 });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Add repository services
builder.Services.AddScoped<INhanVienRepository, SQLNhanVienRepository>();
builder.Services.AddScoped<IPhongBanRepository,SQLPhongBanRepository>();
builder.Services.AddScoped<IChucVuRepository,SQLChucVuRepository>();
builder.Services.AddScoped<ITaiKhoanRepository,SQLTaiKhoanRepository>();
builder.Services.AddScoped<IChamCongRepository, SQLChamCongRepository>();
builder.Services.AddScoped<IBangLuongRepository,SQLBangLuongRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();
builder.Services.AddScoped<ImageStampService>();
builder.Services.AddScoped<IThongBaoRepository, SQLThongBaoRepository>();
builder.Services.AddScoped<IDonNghiPhepRepository,SQLDonNghiPhepRepository>(); builder.Services.AddHttpContextAccessor();
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

var imagePath = Path.Combine(
    builder.Environment.ContentRootPath,
    "Images");

if (!Directory.Exists(imagePath))
{
    Directory.CreateDirectory(imagePath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        imagePath),
    RequestPath = "/Images"
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
