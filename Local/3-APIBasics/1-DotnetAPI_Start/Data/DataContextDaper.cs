using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace HelloWorld.Data
{
    class DataContextDapper
    {
        private readonly IDbConnection dbConnection;
        private string? _connectionString;
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

        public IEnumerable<T> LoadData<T>(string query)
        {
            return dbConnection.Query<T>(query);
        }

        public T LoadSingle<T>(string query)
        {
            return dbConnection.QuerySingle<T>(query);
        }

        public int ExecuteSqlWithRowCount(string query)
        {
            return dbConnection.Execute(query);
        }

        public bool ExecuteSql(string query)
        {
            return dbConnection.Execute(query) > 0;
        }
    }
}