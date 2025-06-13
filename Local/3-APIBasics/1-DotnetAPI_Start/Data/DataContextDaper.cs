using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace HelloWorld.Data
{
    class DataContextDapper
    {
        private readonly IDbConnection dbConnection;
        private readonly string? _connectionString;
        public DataContextDapper(IConfiguration config)
        {
            // 1. Create a connection
            // 1 a. Create a connection string
            // string connectionString = "Server=localhost;Database=DotNetCourseDatabase;TrustServerCertificate=true;Trusted_Conection=true";
            // string _connectionString = "Server=(local);Database=DotNetCourseDatabase;TrustServerCertificate=true;Integrated Security=true";
            _connectionString = config.GetConnectionString("DefaultConnection");
            // To learn more about connection strings see the table at https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlconnection.connectionstring?view=sqlclient-dotnet-core-6.0#remarks
            // See also https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/connection-string-syntax

            // 2. Create a database connection
            dbConnection = new SqlConnection(_connectionString);
        }

        public IEnumerable<T> LoadData<T>(string query, object? parameters = null)
        {
            return dbConnection.Query<T>(query, parameters);
        }

        public T LoadSingle<T>(string query, object? parameters = null)
        {
            return dbConnection.QuerySingle<T>(query, parameters);
        }

        public int ExecuteSqlWithRowCount(string query, object? parameters = null)
        {
            return dbConnection.Execute(query, parameters);
        }

        public bool ExecuteSql(string query, object? parameters = null)
        {
            return dbConnection.Execute(query, parameters) > 0;
        }
    }
}