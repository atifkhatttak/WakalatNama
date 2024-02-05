using Business.Services;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x=> {

        x.DefaultModelsExpandDepth(-1);
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "WakalatNama API V1");
    });
}

app.UseHttpsRedirection();
app.MapHub<ChatHub>("chat-hub");
 
//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapIdentityApi<AppUser>();

app.Run();
