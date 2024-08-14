using Business.Services;
using Data.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Business.BusinessLogic
{
    public class BaseSPRepository
    {
        private WKNNAMADBCtx _context = null;

        public BaseSPRepository(WKNNAMADBCtx context)
        {
            _context = context;
        }
        /// <summary>
        /// Get Record using sp name only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName"></param>
        /// <returns></returns>
        public async Task<IEnumerable<object>> ExecuteStoredProcedureAsync<T>(string spName) where T : class
        {
            try
            {
                return await _context.Database.SqlQuery<object>($"Exec {spName}").ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get database data using sp name and parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string spName, params SqlParameter[] parameters) where T: class
        {
            try
            {
                string query = $"EXEC {spName} {GetSqlParameterNames(parameters)}";
                return await _context.Database.SqlQueryRaw<T>(query, parameters).ToListAsync();               
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        private string GetSqlParameterNames(SqlParameter[] parameters)
        {
            return string.Join(", ", parameters.Select(p => $"{p.ParameterName}"));
        }
        /// <summary>
        /// model properties should be in the same order as per stored procedure parameters order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SqlParameter[] CreateSqlParametersFromModel(object model)
        {
            var properties = model.GetType().GetProperties();
            var parameters = new List<SqlParameter>();

            foreach (var property in properties)
            {
                parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(model) ?? DBNull.Value));
            }

            return parameters.ToArray();
        }
    }
}
