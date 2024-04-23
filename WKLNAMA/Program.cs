using Business.Helpers;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.SwaggerUI;
using WKLNAMA.AppHub;
using WKLNAMA.Extensions;
using WKLNAMA.HostedServices;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddDataServices(builder);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig(builder);
builder.Services.AddJwtConfig(builder);
builder.Services.AddSignalR();
builder.Services.AddHostedService<ServerNotificationService>();
var app = builder.Build();

Utils._config = new ConfigurationBuilder().SetBasePath(app.Environment.ContentRootPath).AddJsonFile("appSettings.json").Build();


ServiceActivator.Configure(app.Services);
var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());




// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI(x=> {
        x.DocExpansion(DocExpansion.None);
        x.DefaultModelsExpandDepth(-1);
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "WakalatNama API V1");
    });
//}

//app.UseHttpsRedirection();

//app.UseAuthentication();U

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(),"Uploads")),
    RequestPath = "/resources"
});
app.UseAuthorization();
app.MapHub<ChatHub>("chat-hub");

app.UseCors(options =>
{
    options.WithOrigins().AllowAnyMethod().AllowCredentials().AllowAnyHeader().SetIsOriginAllowed((host) => true);
});
app.MapControllers();
//app.MapIdentityApi<AppUser>();

await app.Services.GetService<IDBInitializer>().Init();
await app.Services.GetService<IDBInitializer>().InitializeCustomerService();

app.Run();
