using EDaemonWebServer.Repositories;
using EDaemonWebServer.Repositories.Interfaces;
using EDaemonWebServer.Services;
using EDaemonWebServer.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IHealthCheckRepository, HealthCheckRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();

builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();
builder.Services.AddScoped<ISkillService, SkillService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});


var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
