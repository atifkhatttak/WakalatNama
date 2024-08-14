using Business.Chat_Hub;
using Business.Helpers;
using Business.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net;
using WKLNAMA.Extensions;
using WKLNAMA.Helpers;
using WKLNAMA.HostedServices;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddDataServices(builder);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig(builder);
builder.Services.AddJwtConfig(builder);

//CorsHelper.ConfigureService(builder.Services);

builder.Services.AddSignalR();
builder.Services.AddHostedService<ServerNotificationService>();
builder.Services.AddHostedService<EmailInstantNotificationService>();
builder.Services.AddHostedService<SMSInstantNotification>();

var app = builder.Build();

Utils._config = new ConfigurationBuilder().SetBasePath(app.Environment.ContentRootPath).AddJsonFile("appSettings.json").Build();


ServiceActivator.Configure(app.Services);
var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());




// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//app.UseCors(options =>
//{
//    options.WithOrigins().AllowAnyMethod().AllowCredentials().AllowAnyHeader().SetIsOriginAllowed((host) => true);
//});
//app.UseCors("CORSPolicy");

//app.UseMiddleware<CustomCorsMiddleware>(allowedOrigins);
app.Use(async (context, next) =>
{
    var allowedOrigins = new[] { "https://localhost:44325", "https://wakalatnama.vercel.app", "*" };
    var origin = context.Request.Headers["Origin"].ToString();

    if (allowedOrigins.Contains(origin) || allowedOrigins.Contains("*"))
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With, Authorization,x-signalr-user-agent");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");

        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            return;
        }
    }

    await next.Invoke();

});
//app.UseCors("CORSPolicy");
app.UseSwagger();
app.UseSwaggerUI(x => {
    x.DocExpansion(DocExpansion.None);
    x.DefaultModelsExpandDepth(-1);
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "WakalatNama API V1");
});
//}

//app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    // Place a breakpoint here
    var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    if (authorizationHeader == null)
    {
        string accessToken = context.Request.Query["access_token"].FirstOrDefault();
        if (!string.IsNullOrEmpty(accessToken))
        {
            context.Request.Headers.Add("Authorization", $"Bearer {accessToken}");
        }
        // Check the Authorization token
        Console.WriteLine($"Authorization Header: {authorizationHeader}");
    }

    await next.Invoke();
});

app.UseAuthentication();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/resources"
});
app.UseAuthorization();
app.MapHub<ChatHub>("/chat-hub");


app.MapControllers();
//app.MapIdentityApi<AppUser>();

await app.Services.GetService<IDBInitializer>().Init();
await app.Services.GetService<IDBInitializer>().InitializeCustomerService();

app.Run();
