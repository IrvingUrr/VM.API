using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;
using VM.Core.Filters;
using VM.Data.Models;
using VM.Service.CurrencyService;
using VM.Service.HttpService;
using VM.Service.User;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Debug()
    .WriteTo.File(builder.Configuration["SerilogFilePath"]!, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
    .WriteTo.Console());

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAnyOrigin",
                      policy =>
                      {
                          policy.AllowAnyOrigin();
                      });
});

// Add services to the container.

builder.Services.AddControllers(options => 
    options.Filters.Add(new ApiExceptionFilterAttribute())
    ).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");
builder.Services.AddDbContext<VM_DbContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<HttpClient>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAnyOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
