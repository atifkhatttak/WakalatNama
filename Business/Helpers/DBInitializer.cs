using Business.Enums;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Identity;
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
        public async Task InitializeCustomerService()
        {

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                var _context = serviceScope.ServiceProvider.GetRequiredService<WKNNAMADBCtx>();

                var _user =  await _userManager.FindByNameAsync("cservice");

                    if (_user == null)
                    {
                        AppUser user = new AppUser
                        {
                            FirstName="Customer",
                            LastName= "Service",
                            Email="customerservice@yopmail.com",
                            UserName="cservice",
                            PhoneNumber="403-23202-2",
                        };
                    var _userCreated =   await _userManager.CreateAsync(user);
                 
                        if (_userCreated.Succeeded)
                        { 
                            var _passwordCreated = await _userManager.AddPasswordAsync(user!, "Admin@123");
                         
                            if (_passwordCreated.Succeeded)
                            {
                               var _userRoleCreated =  await _userManager.AddToRoleAsync(user, "Admin");
                            }
                        }

                  await  _context.SaveChangesAsync();
                }
            }
        }
    }
public interface IDBInitializer
    {
        Task Init();
        Task InitializeCustomerService();
    }


}
