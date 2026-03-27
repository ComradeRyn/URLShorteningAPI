using System.Reflection;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Sqids;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
    
    setupAction.AddSecurityDefinition("BankingApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Input a valid token to access this API"
    });

    setupAction.AddSecurityRequirement(document => new OpenApiSecurityRequirement()
    {
        [new OpenApiSecuritySchemeReference("BankingApiBearerAuth", document)] = []
    });
});

builder.Services.AddSingleton(new SqidsEncoder<long>(new SqidsOptions
{
    MinLength = 6,
}));

builder.Services.AddScoped<IShortLinksRepository, ShortLinksRepository>();
builder.Services.AddScoped<IVisitsRepository, VisitsRepository>();

builder.Services.AddScoped<IShortCodesService, ShortCodesService>();
builder.Services.AddScoped<IShortLinksService, ShortLinksService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddDbContext<UrlShorteningContext>(opt
    => opt.UseSqlServer(builder.Configuration.GetConnectionString("UrlShorteningContext")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtOptions =>
{
    jwtOptions.Audience = builder.Configuration["Authentication:Audience"];
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]!))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();