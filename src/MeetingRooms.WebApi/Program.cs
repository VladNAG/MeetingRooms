using MeetingRooms.Application;
using MeetingRooms.DataAccess;
using MeetingRooms.Infrastructure;
using MeetingRooms.Infrastructure.Logging;
using MeetingRooms.Infrastructure.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseConfiguredSerilog();

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

builder.Services.AddApplication();
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddInfrastructure();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
