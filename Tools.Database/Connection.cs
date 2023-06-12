using System.Data;
using System.Data.Common;

namespace Tools.Database
{
    public class Connection
    {
        private readonly DbProviderFactory _providerFactory;
        private readonly string _connectionString;

        public Connection(DbProviderFactory providerFactory, string connectionString)
        {
            _connectionString = connectionString;
            _providerFactory = providerFactory;
        }

        public int ExecuteNonQuery(Command command)
        {
            using(DbConnection sqlConnection = CreateConnection(_providerFactory, _connectionString))
            {
                using (DbCommand sqlCommand = CreateCommand(sqlConnection, command))
                {     
                    sqlConnection.Open();
                    return sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public object? ExecuteScalar(Command command)
        {
            using (DbConnection sqlConnection = CreateConnection(_providerFactory, _connectionString))
            {
                using (DbCommand sqlCommand = CreateCommand(sqlConnection, command))
                {
                    sqlConnection.Open();
                    object? result = sqlCommand.ExecuteScalar();
                    return result is DBNull ? null : result;
                }
            }
        }

        public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
        {
            ArgumentNullException.ThrowIfNull(selector, nameof(selector));

            using (DbConnection sqlConnection = CreateConnection(_providerFactory, _connectionString))
            {
                using (DbCommand sqlCommand = CreateCommand(sqlConnection, command))
                {
                    sqlConnection.Open();
                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while(reader.Read()) 
                        {
                            yield return selector(reader);
                        }
                    }
                }
            }
        }

        private static DbConnection CreateConnection(DbProviderFactory providerFactory, string connectionString)
        {
            DbConnection? connection = providerFactory.CreateConnection();

            if(connection is null)
            {
                throw new InvalidOperationException("Erreur à la création de l'instance de type DbConnection...");
            }

            connection.ConnectionString = connectionString;
            return connection;
        }

        private static DbCommand CreateCommand(DbConnection sqlConnection, Command command)
        {
            DbCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = command.Query;

            if (command.IsStoredProcedure)
                sqlCommand.CommandType = CommandType.StoredProcedure;

            foreach (KeyValuePair<string, object> kvp in command.Parameters)
            {
                DbParameter sqlParameter = sqlCommand.CreateParameter();
                sqlParameter.ParameterName = kvp.Key;
                sqlParameter.Value = kvp.Value;
                sqlCommand.Parameters.Add(sqlParameter);
            }

            return sqlCommand;
        }
    }
}
