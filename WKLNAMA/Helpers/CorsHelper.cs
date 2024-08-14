namespace WKLNAMA.Helpers
{
    public class CorsHelper
    {
        public static void ConfigureService(IServiceCollection service)
        {
            service.AddCors(option => option.AddPolicy("CORSPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
        }
    }
}
