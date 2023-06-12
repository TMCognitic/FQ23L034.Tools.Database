using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Database.Extensions
{
    public static class DbConnectionExtensions
    {
        public static int ExecuteNonQuery(this DbConnection dbConnection, string query, bool isStoredProcedure = false, object? parameters = null)
        {
            using (DbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters)) 
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();

                return dbCommand.ExecuteNonQuery();
            }
        }

        public static object? ExecuteScalar(this DbConnection dbConnection, string query, bool isStoredProcedure = false, object? parameters = null)
        {
            using (DbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters))
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();

                object? result = dbCommand.ExecuteScalar();
                return result is DBNull ? null : result;
            }
        }

        public static IEnumerable<TResult> ExecuteReader<TResult>(this DbConnection dbConnection, string query, Func<IDataRecord, TResult> selector, bool isStoredProcedure = false, object? parameters = null)
        {
            ArgumentNullException.ThrowIfNull(selector, nameof(selector));
            using (DbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters))
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();

                using(DbDataReader reader = dbCommand.ExecuteReader())
                {
                    while(reader.Read())
                        yield return selector(reader);
                }
            }
        }

        private static DbCommand CreateCommand(DbConnection dbConnection, string query, bool isStoredProcedure, object? parameters)
        {
            DbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = query;

            if (isStoredProcedure)
                dbCommand.CommandType = CommandType.StoredProcedure;

            if(parameters is not null)
            {
                //Attention Reflection
                Type type = parameters.GetType();

                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    if(propertyInfo.CanRead)
                    {
                        DbParameter sqlParameter = dbCommand.CreateParameter();
                        sqlParameter.ParameterName = propertyInfo.Name;
                        sqlParameter.Value = propertyInfo.GetValue(parameters) ?? DBNull.Value;
                        dbCommand.Parameters.Add(sqlParameter);
                    }                    
                }
            }
            

            return dbCommand;
        }
    }
}
