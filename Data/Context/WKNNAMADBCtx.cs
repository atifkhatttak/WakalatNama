using Data.DomainModels;
using Data.Intercepters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjWakalatnama.DataLayer.Models;
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
        private readonly IHttpContextAccessor httpContextAccessor;

        public WKNNAMADBCtx(DbContextOptions<WKNNAMADBCtx> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            this.SeedRoles(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder
           .AddInterceptors(new DataDefaultColumnInterceptor(httpContextAccessor));

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<AppRole>().HasData(

                new AppRole() { Id = 1, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new AppRole() { Id = 2, Name = "Zonal Manager", ConcurrencyStamp = "2", NormalizedName = "Zonal Manager" },
                new AppRole() { Id = 3, Name = "Citizen", ConcurrencyStamp = "3", NormalizedName = "Citizen" },
                new AppRole() { Id = 4, Name = "Lawyer", ConcurrencyStamp = "4", NormalizedName = "Laywer" },
                new AppRole() { Id = 5, Name = "Employee ", ConcurrencyStamp = "5", NormalizedName = "Employee " }

                );
        }

        public virtual DbSet<CaseCategory> CaseCategories { get; set; } = null!;
        public virtual DbSet<CaseJurisdiction> CaseJurisdictions { get; set; } = null!;
        public virtual DbSet<CasesDetail> CasesDetails { get; set; } = null!;
        public virtual DbSet<ConsultationOption> ConsultationOptions { get; set; } = null!;
        public virtual DbSet<CourtCases> CourtCases { get; set; } = null!;
        public virtual DbSet<ExperienceCost> ExperienceCosts { get; set; } = null!;
        public virtual DbSet<PartyStatus> PartyStatuses { get; set; } = null!;
        public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public virtual DbSet<UserDocument> UserDocuments { get; set; } = null!;
        public virtual DbSet<CasesDocument> CasesDocuments { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Favourite> Favourites{ get; set; } = null!;

    }
}

