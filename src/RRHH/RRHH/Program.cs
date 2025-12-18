using Microsoft.EntityFrameworkCore;
using RRHH.Application.Interfaces.Repositories;
using RRHH.Application.Interfaces.Services;
using RRHH.Infrastructure.Data;
using RRHH.Infrastructure.Repositories;
using RRHH.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<RRHHDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RRHHBD")));

// Registro Inyección de dependencia
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var MyCorsPolicy = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsPolicy, policy =>
    {
        policy.WithOrigins(
                "http://localhost:8100"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(MyCorsPolicy);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
