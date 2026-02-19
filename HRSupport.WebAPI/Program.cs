using HRSupport.Application.Interfaces;
using HRSupport.Infrastructure.Context;
using HRSupport.Application.Features.Employees.Commans;
using HRSupport.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Veritabanı Context Ayarı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// Repository'lerin Dependency Injection (DI) Kayıtları
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IInternRepository, InternRepository>();

// MediatR Kaydı
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateEmployeeCommand).Assembly));

// AutoMapper Kaydı (.Assembly kısmı silindi, sadece Type verildi)
builder.Services.AddAutoMapper(new[] { typeof(HRSupport.Application.Mappings.MappingProfile) }); 
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseRouting();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();