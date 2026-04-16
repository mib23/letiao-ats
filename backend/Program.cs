using System.Text;
using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Infrastructure;
using Letiao.ATS.Api.Middleware;
using Letiao.ATS.Api.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Dapper 全局配置：snake_case → PascalCase 字段映射
DapperConfig.ConfigureSnakeCaseMapping();

var builder = WebApplication.CreateBuilder(args);

// ─── 服务注册 ────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy =
        System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "乐跳 ATS API", Version = "v1" });
    // Swagger 支持 Bearer Token 填写
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "请输入 JWT Token（不含 'Bearer ' 前缀）"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ─── JWT 认证 ─────────────────────────────────────────────────
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key 未配置");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero // 不允许时钟偏差，Token 到期立即失效
        };
    });

builder.Services.AddAuthorization();

// ─── 跨域（前端开发服务器）────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ─── DI 注册：基础设施 & 业务服务 ──────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JobRepository>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<CandidateRepository>();
builder.Services.AddScoped<CandidateService>();
builder.Services.AddScoped<IOssService, LocalOssService>();
builder.Services.AddHttpClient<AiResumeParser>();

// ─── 应用构建 ────────────────────────────────────────────────
var app = builder.Build();

// 全局异常中间件（最外层）
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "乐跳 ATS API v1"));
}

// 静态文件（OSS 本地化存储目录）
app.UseStaticFiles();

app.UseCors("DevCors");
app.UseAuthentication();
app.UseAuthorization();

// 操作日志中间件（需在 Auth 之后，才能读取 JWT Claims）
app.UseMiddleware<OperationLogMiddleware>();

app.MapControllers();

app.Run();
