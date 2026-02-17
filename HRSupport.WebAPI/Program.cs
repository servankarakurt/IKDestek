using HRSupport.Infrastructure.Context;
using HRSupport.Infrastructure.Repositories;
using MediatR;
using System.Reflection;
using HRSupport.Application.Features.Employee.Commans;
using HRSupport.Application.Interfaces;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IEmployeeRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HRSupport.Application.AssemblyReference).Assembly));
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRecdirect();
app.useauthorization();
app.MapControllers();

app.Run();