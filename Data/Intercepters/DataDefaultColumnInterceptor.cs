using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Data.Intercepters
{
    public class DataDefaultColumnInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public   DataDefaultColumnInterceptor( IHttpContextAccessor httpContextAccessor )
        {
            _httpContextAccessor = httpContextAccessor; 
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

              BeforeSaveTriggers(eventData!.Context!);

            return result;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

           BeforeSaveTriggers(eventData!.Context!);
            return result;
        }

        private void BeforeSaveTriggers(DbContext context)
        {
            bool isAuthenticated = _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            long currentUserId = isAuthenticated ? Convert.ToInt64(  _httpContextAccessor?.HttpContext?.User?.FindFirstValue("UserId")):-1; //-1 means the entries is done by unauthenticated user

            var _currentUtcTime = DateTime.UtcNow;
            var entries =   context?.ChangeTracker
            .Entries()
            .Where(e => (e.Entity is BaseModel || e.Entity is AppUser) && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified
                    || e.State == EntityState.Deleted));

            foreach (var entityEntry in entries!)
            {
                if (entityEntry.Entity is BaseModel)
                {
                    ((BaseModel)entityEntry.Entity).UpdateDate = _currentUtcTime;

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((BaseModel)entityEntry.Entity).CreatedDate = _currentUtcTime;
                        ((BaseModel)entityEntry.Entity).CreatedBy = currentUserId;

                    }
                    if (entityEntry.State == EntityState.Modified)
                    {
                        ((BaseModel)entityEntry.Entity).UpdatedBy = currentUserId;
                    }
                    if (entityEntry.State == EntityState.Deleted)
                    {
                        ((BaseModel)entityEntry.Entity).IsDeleted = true;
                        entityEntry.State = EntityState.Modified;
                    }
                }
                else if(entityEntry.Entity is AppUser)
                {
                    ((AppUser)entityEntry.Entity).UpdateDate = _currentUtcTime;

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((AppUser)entityEntry.Entity).CreatedDate = _currentUtcTime;
                        ((AppUser)entityEntry.Entity).CreatedBy = currentUserId;

                    }
                    if (entityEntry.State == EntityState.Modified)
                    {
                        ((AppUser)entityEntry.Entity).UpdatedBy = currentUserId;
                    }
                    if (entityEntry.State == EntityState.Deleted)
                    {
                        ((AppUser)entityEntry.Entity).IsDeleted = true;
                        entityEntry.State = EntityState.Modified;
                    }
                }
               
            }
        }

    
    }
}
