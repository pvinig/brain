using BRN.identidade.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile(path: $"appsettings.{builder.Environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddApiConfiguration();

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddSwagger();

// HTTP req pipeline. 

var app = builder.Build();

app.UseSwager();

app.UseApiConfiguration(builder.Environment);

app.Run();