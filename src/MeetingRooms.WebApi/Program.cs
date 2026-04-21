using MeetingRooms.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

builder.Services.AddDataAccess(builder.Configuration);

var app = builder.Build();

app.Run();
