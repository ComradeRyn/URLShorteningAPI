using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Sqids;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new SqidsEncoder<long>(new SqidsOptions
{
    MinLength = 5,
}));

builder.Services.AddScoped<IShortLinksRepository, ShortLinksRepository>();
builder.Services.AddScoped<IVisitsRepository, VisitsRepository>();

builder.Services.AddScoped<IShortCodesService, ShortCodesService>();
builder.Services.AddScoped<IShortLinksService, ShortLinksService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.AddDbContext<UrlShorteningContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("UrlShorteningContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();