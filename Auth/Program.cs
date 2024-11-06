using Auth.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os servi�os necess�rios
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<JwtTokenGenerator>();

// Configura��o de autentica��o com JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false,
        ValidIssuer = builder.Configuration["AuthApi"],
        ValidAudience = builder.Configuration["AuthApiClient"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("97161037-1098-4519-B50C-C40048FCE383"))
    };
});

// Configura��o CORS para permitir qualquer origem, m�todo e cabe�alho
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configura��o do Swagger para aceitar autentica��o JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API de Agenda de compromissos", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira 'Bearer' seguido de um espa�o e o token JWT no campo abaixo.",
    };

    options.AddSecurityDefinition("Bearer", securitySchema);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { "Bearer" } }
    });
});


var app = builder.Build();

// Configura��o do pipeline de requisi��o
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
