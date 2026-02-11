using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentClub.API.Extensions;
using StudentClub.API.Validators.Club;
using StudentClub.API.WebSockets;
using StudentClub.Application;
using StudentClub.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình TokenValidationParameters (QUAN TRỌNG ĐỂ WEBSOCKET CHẠY ĐƯỢC)
// Bạn cần lấy Key, Issuer, Audience từ appsettings.json giống hệt như lúc config JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]); // Đảm bảo key trong appsettings.json khớp
var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ClockSkew = TimeSpan.Zero // Tùy chọn: bỏ độ trễ mặc định 5 phút
};

// Đăng ký Singleton để ChatWebSocketEndpoint có thể gọi ra dùng
builder.Services.AddSingleton(tokenValidationParams);

builder.Services
    .AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateClubRequest>();
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentClub API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token dạng: Bearer {your token}"
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
            new string[] {}
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Cập nhật lại JWT Auth để dùng chung biến tokenValidationParams (Code gọn hơn)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParams; // Dùng lại biến bên trên
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // WebSocket thường cần cái này nếu có cookie, nhưng token query string thì ko bắt buộc
        });
});
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets(); // Phải đặt trước Authentication/Authorization
ChatWebSocketEndpoint.MapChat(app);

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();