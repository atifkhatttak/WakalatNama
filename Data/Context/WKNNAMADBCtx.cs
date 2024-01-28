using Data.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context
{
    public class WKNNAMADBCtx : IdentityDbContext<AppUser,AppRole,int>
    {
        
        public WKNNAMADBCtx(DbContextOptions<WKNNAMADBCtx> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            this.SeedRoles(builder);
        }
        

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<AppRole>().HasData(

                new AppRole() { Id = 1, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new AppRole() { Id = 2, Name = "Zonal Manager", ConcurrencyStamp = "2", NormalizedName = "Zonal Manager" },
                new AppRole() { Id = 3, Name = "Citizen", ConcurrencyStamp = "3", NormalizedName = "Citizen" },
                new AppRole() { Id = 4, Name = "Laywer", ConcurrencyStamp = "4", NormalizedName = "Laywer" },
                new AppRole() { Id = 5, Name = "Employee ", ConcurrencyStamp = "5", NormalizedName = "Employee " }

                );
        }

        
    }
}

