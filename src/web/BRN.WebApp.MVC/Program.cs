using BRN.WebApp.MVC.Configuration;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddMvcConfig();

builder.Services.AddIdentityConfig();

builder.Services.RegisterServices();

var app = builder.Build();


app.UseWebAppConfig(app.Environment);

app.Run();