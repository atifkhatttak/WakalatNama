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
using System.Reflection.Emit;
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

            //Transaction Table
            builder.Entity<Payment>().Property(x => x.Amount).HasPrecision(16, 3);
            builder.Entity<Payment>().Property(x => x.RemainingAmount).HasPrecision(16, 3);
            builder.Entity<PaymentTransaction>().Property(x => x.Amount).HasPrecision(16, 3);

            builder.Entity<ExperienceCost>().Property(x=>x.CostMin).HasPrecision(16,3);
            builder.Entity<ExperienceCost>().Property(x=>x.CostMax).HasPrecision(16,3);
            builder.Entity<LawyerFeeStructure>().Property(x=>x.LawyerFee).HasPrecision(16,3);
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
                new AppRole() { Id = 4, Name = "Lawyer", ConcurrencyStamp = "4", NormalizedName = "Lawyer" },
                new AppRole() { Id = 5, Name = "Employee ", ConcurrencyStamp = "5", NormalizedName = "Employee " });

            builder.Entity<RejectionReason>().HasData(
               new RejectionReason() { RejectionId = 1, RejectionNote = "Other",Description="Other" },
               new RejectionReason() { RejectionId = 2, RejectionNote = "Incomplete documents", Description= "Incomplete documents" },
               new RejectionReason() { RejectionId = 3, RejectionNote = "Payment Incomplete", Description = "Payment Incomplete" }
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
        public virtual DbSet<ChatSession> ChatSessions { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Favourite> Favourites{ get; set; } = null!;
        public virtual DbSet<Country> Countries{ get; set; } = null!;
        public virtual DbSet<City> Cities{ get; set; } = null!;
        public virtual DbSet<LawyerFeeStructure> LawyerFeeStructures { get; set; } = null!;
        public virtual DbSet<CaseRejectionReason> CaseRejectionReasons { get; set; } = null!; 
        public virtual DbSet<CaseStatus> CaseStatuses{ get; set; } = null!;
        public virtual DbSet<CategoriesStatus> CategoriesStatuses{ get; set; } = null!;
        public virtual DbSet<LawyerExperties> LawyerExperties{ get; set; } = null!;
        public virtual DbSet<LawyerQualification> LawyerQualifications{ get; set; } = null!;
        public virtual DbSet<RejectionReason> RejectionReasons{ get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
        public virtual DbSet<UserBanner> UserBanners { get; set; } = null!;
        public virtual DbSet<SMSLogs> SMSLogs { get; set; } = null!; 
        public virtual DbSet<EmailLogs> EmailLogs { get; set; } = null!;

    }
}

