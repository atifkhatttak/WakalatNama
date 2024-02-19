using Data.Context;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public class DBInitializer : IDBInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DBInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public   async Task Init()
        {

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<WKNNAMADBCtx>())
                {
                    await context.Database.MigrateAsync();
                }
            }
        }
    }
public interface IDBInitializer
    {
        Task Init();
    }


}
