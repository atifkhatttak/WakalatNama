using Business.Helpers;
using Business.Services;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using WKLNAMA.AppHub;
using WKLNAMA.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddDataServices(builder);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig(builder);
builder.Services.AddJwtConfig(builder);
builder.Services.AddSignalR();
var app = builder.Build();

Utils._config = new ConfigurationBuilder().SetBasePath(app.Environment.ContentRootPath).AddJsonFile("appSettings.json").Build();


ServiceActivator.Configure(app.Services);
var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());

app.Services.GetService<IDBInitializer>().Init();
 
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(x=> {

        x.DefaultModelsExpandDepth(-1);
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "WakalatNama API V1");
    });
//}

//app.UseHttpsRedirection();
 
//app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("chat-hub");


app.MapControllers();
//app.MapIdentityApi<AppUser>();

app.Run();
