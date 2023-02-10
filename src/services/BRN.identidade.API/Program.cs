using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using BRN.identidade.API.data;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging.Configuration;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<ApplicationDbContext>(optionsAction: options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString(name: "DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(configureOptions: Options =>
{
    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerOptions =>
{
    JwtBearerOptions.RequireHttpsMetadata = true;
    JwtBearerOptions.SaveToken = true;
    JwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(s:"x")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "x",
        ValidIssuer = "x"
    };
});

builder.Services.AddSwaggerGen(u =>
{
    u.SwaggerDoc(name: "v1", new OpenApiInfo
    {
        Title = "pinky e cerebro: brain",
        Description = "servidorzinho REST distibuido ecomerce",
        Contact = new OpenApiContact() { Name = "Vinicius Pretto", Email = "pvinig@gmail.com"}
    });
});
// var appSetingsSection = Configuration.
   // .GetSection(key: "AppSetings");

// Configure the HTTP request pipeline. 


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(u =>
{
    u.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "v1");
});

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();


app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();
