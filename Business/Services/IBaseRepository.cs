using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IBaseRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<T> GetById(object id);
        void Insert(T obj);
        void Update(T obj);
        Task Delete(object id);
        void Save();
        Task SaveAsync(); 
    }
}
