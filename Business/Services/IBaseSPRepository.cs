using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IBaseSPRepository<T> where T : class
    {
        //Task<IEnumerable<T>> GetAll(string spName);
        //Task<IEnumerable<T>> GetAll(string spName,params SqlParameter[] sqlParameters);
        Task<IEnumerable<T>> GetAllAsync(string spName);
        //Task<IEnumerable<T>> GetAllAsync(string spName,params SqlParameter[] sqlParameters);
        //Task<T> GetById(object id);
        //void Insert(T obj);
        //void Update(T obj);
        //Task Delete(object id);
        //void Save();
        //Task SaveAsync(); 
    }
}
