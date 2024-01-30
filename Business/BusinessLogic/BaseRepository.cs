using Business.Services;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        //The following variable is going to hold the EmployeeDBContext instance
        private WKNNAMADBCtx _context = null;
        //The following Variable is going to hold the DbSet Entity
        private DbSet<T> table = null;

        public BaseRepository(WKNNAMADBCtx _context) 
        {
            this._context = _context;
            table = _context.Set<T>();
        }

        public async Task<T> GetById(object id)
        {
            return await table.FindAsync(id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public  async Task SaveAsync()
        {
          await  _context.SaveChangesAsync();
        }

        public async Task  Delete(object id)
        {
            //First, fetch the record from the table
            T existing = await table.FindAsync(id);

            if(existing != null) 
            {
                
            table.Remove(existing);
            
            }
            //This will mark the Entity State as Deleted
        }

        public void Update(T obj)
        {
            //First attach the object to the table
            table.Attach(obj);
            //Then set the state of the Entity as Modified
            _context.Entry(obj).State = EntityState.Modified;
        }

        public void Insert(T obj)
        {
            //It will mark the Entity state as Added State
            table.Add(obj);
        }


        public async Task<IEnumerable<T>> GetAll()
        {
            return  await table.ToListAsync();
        }
    }
}
