using Business.BusinessLogic;
using Business.Helpers;
using Business.Services;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using WKLNAMA.AppHub;
using WKLNAMA.Extensions.Override;
using WKLNAMA.Filters;
using WKLNAMA.TokenService;

namespace WKLNAMA.Extensions
{
    public class ServiceActivator
    {
        internal static IServiceProvider _serviceProvider = null;

        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IServiceScope GetScope(IServiceProvider serviceProvider = null)
        {
            var provider = serviceProvider ?? _serviceProvider;
            return provider?
                .CreateScope();
        }
    }
    public static class ExtensionServicesContainer
    { 
        /// <summary>
        /// The method will add data services to DI Container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builder"></param>
        public static void AddDataServices(this IServiceCollection services,WebApplicationBuilder builder)
        {
            services.AddDbContext<WKNNAMADBCtx>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WKLNAMAConnection")));


            services
            .AddIdentityApiEndpoints<AppUser>()
            .AddRoles<AppRole>()
                .AddEntityFrameworkStores<WKNNAMADBCtx>();

            services.AddScoped(typeof(IBaseRepository<>),typeof(BaseRepository<>));
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICaseJurisdictionRepository, CaseJurisdictionRepository>();
            services.AddScoped<IPartyStatusRepository, PartyStatusRepository>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<ICasesRepository,CasesRepository>();
            services.AddScoped<IDocumentService, GoogleDriveDocument>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ISettingsRepository,SettingsRepository>();
            services.AddSingleton<IUserIdProvider, IdBasedUserIdProvider>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddSingleton<ChatHub>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<ITokenService, TokenService.TokenService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddSingleton<IDBInitializer, DBInitializer>();
            builder.Services.AddSingleton< IHttpContextAccessor,HttpContextAccessor>();


        }

        /// <summary>
        /// The method is used for Ading JWT Configuration to DI Container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builder"></param>
        public static void AddJwtConfig(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //Jwt configuration starts here
            var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
            var jwtAudience = builder.Configuration.GetSection("Jwt:Audience").Get<string>(); 
            var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwtIssuer,
                     ValidAudience = jwtAudience,
                     ClockSkew = TimeSpan.Zero,
                     LifetimeValidator = TokenLifetimeValidator.Validate,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                 };
             });
        }

        /// <summary>
        /// This method is used for Swagger Configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builder"></param>
        public static void AddSwaggerConfig(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(option =>
            {
                option.EnableAnnotations();
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "WakalatNama API", Version = "V1" });

                option.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter a valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer"
                    }
                );
                    option.OperationFilter<AuthResponsesOperationFilter>();
                // option.OperationFilter<AuthenticationRequirementsOperationFilter>();

                // option.OperationFilter<SecurityRequirementsOperationFilter>();

                option.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
                    }
                );
              //  option.AddSignalRSwaggerGen();

            });


        }
   
    }
}
