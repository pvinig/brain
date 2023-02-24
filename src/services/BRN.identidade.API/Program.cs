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
using BRN.identidade.API.extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<ApplicationDbContext>(optionsAction: options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString(name: "DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT

var appSettingsSection = builder.Configuration.GetSection(key: "AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

var appSetings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSetings.Secret);


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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = appSetings.Audience,
        ValidIssuer = appSetings.Issuer
    };
});



// Swagger

builder.Services.AddSwaggerGen(u =>
{
    u.SwaggerDoc(name: "v1", new OpenApiInfo
    {
        Title = "pinky e cerebro: brain",
        Description = "servidorzinho REST distibuido ecomerce",
        Contact = new OpenApiContact() { Name = "Vinicius Pretto", Email = "pvinig@gmail.com"}
    });
});


// HTTP req pipeline. 

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
