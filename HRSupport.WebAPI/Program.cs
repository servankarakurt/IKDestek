using HRSupport.Application.Features.Employees.Commans;
using HRSupport.Application.Interfaces;
using HRSupport.Infrastructure.Context;
using HRSupport.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

 var builder = WebApplication.CreateBuilder(args);

// 1. Temel Servisler
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();

// 2. Veritabanı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// 3. Repository Kayıtları (Dependency Injection)
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<IInternRepository, InternRepository>();
// 4. MediatR ve AutoMapper
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateEmployeeCommand).Assembly));
builder.Services.AddAutoMapper(typeof(HRSupport.Application.Mappings.MappingProfile).Assembly);

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HRSupport API v1");
 
        c.RoutePrefix = string.Empty; 
    });
}
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
