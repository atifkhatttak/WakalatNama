using Business.Helpers.Attributes;
using Business.Helpers.ReportQueries;
using Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Business.Helpers.Extension
{
    public static class FormatSqlQuery
    {
        private static string ReportType { set; get; }
        public static string CreateWhereClause(this SQLBasedVm sqlBasedVm)
        {
            var properties = sqlBasedVm?.GetType().GetProperties().Where(x=>x.GetCustomAttribute<JsonIgnoreAttribute>()==null);
           
            string _whereClause = string.Empty;
            string columnName = string.Empty;
            
            foreach (var propertyInfo in properties)
            {
                var sqlCoumn = propertyInfo.GetCustomAttributes(typeof(SQLColumnAttribute), false).FirstOrDefault();

                columnName = sqlCoumn == null ? propertyInfo.Name : ((SQLColumnAttribute)sqlCoumn).Name;

                if (propertyInfo.PropertyType == typeof(string) && !string.IsNullOrEmpty(propertyInfo.GetValue(sqlBasedVm)?.ToString()) && propertyInfo.GetValue(sqlBasedVm)?.ToString() != "string")
                {
                    _whereClause += string.IsNullOrEmpty(_whereClause)? "": " AND ";
                   
                    if (((SQLColumnAttribute)sqlCoumn).IsContain)
                        _whereClause += $" {columnName} like '%{propertyInfo.GetValue(sqlBasedVm)}%'{Environment.NewLine}";
                    else 
                        _whereClause +=  $" {columnName} = '{propertyInfo.GetValue(sqlBasedVm)}'{Environment.NewLine}";
                }
                else if ((propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?)) && propertyInfo.GetValue(sqlBasedVm, null) != null)
                {
                    _whereClause += string.IsNullOrEmpty(_whereClause) ? "" : " AND ";
                    _whereClause += $" {columnName} = {propertyInfo.GetValue(sqlBasedVm)} ";
                }
                else if ((propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?)) && propertyInfo.GetValue(sqlBasedVm) != null)
                {
                    _whereClause += string.IsNullOrEmpty(_whereClause) ? "" : " AND ";

                    if (((SQLColumnAttribute)sqlCoumn).IsDataEqual)
                        _whereClause += $" {columnName} = '{propertyInfo.GetValue(sqlBasedVm)}' {Environment.NewLine} ";
                    else if (((SQLColumnAttribute)sqlCoumn).IsDateLessOrEqual)
                        _whereClause += $" {columnName} =< '{propertyInfo.GetValue(sqlBasedVm)}' {Environment.NewLine} ";
                    else if (((SQLColumnAttribute)sqlCoumn).IsDateGreaterOrEqual)
                        _whereClause += $" {columnName} >= '{propertyInfo.GetValue(sqlBasedVm)}' {Environment.NewLine} ";
                    else
                        _whereClause += $" {columnName} = '{propertyInfo.GetValue(sqlBasedVm)}' {Environment.NewLine} ";

                }

            }
            return _whereClause;
        }

        public static string GenrateReportQuery(this SQLBasedVm sqlBasedVm) 
        {
           var reportQuery = new ReportQueries.ReportQueries().GetType().GetMethod(sqlBasedVm.ReportType).Invoke(sqlBasedVm.ReportType,null);

                return reportQuery?.ToString();
        }

        /// <summary>
        /// This extension method will call query based on Report Type and will generate where clause based on the inputs
        /// </summary>
        /// <param name="sqlBasedVm"></param>
        /// <returns>The method will return output query with where clause</returns>
        public static string GenerateReportQueriesWithWhereClause(this SQLBasedVm sqlBasedVm) 
        {
            var reportQuery = new ReportQueries.ReportQueries().GetType().GetMethod(sqlBasedVm.ReportType).Invoke(sqlBasedVm.ReportType, null);

            var whereClause = CreateWhereClause(sqlBasedVm);

            if (!string.IsNullOrEmpty(whereClause))
                reportQuery += " Where " + whereClause; 

            string finalQuery = $" {reportQuery} ";

            return finalQuery;
        }
    }
}
