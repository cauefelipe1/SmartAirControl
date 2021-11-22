using Microsoft.Data.SqlClient;
using Npgsql;
using SmartAirControl.API.Core.Settings;
using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace SmartAirControl.API.Database
{
    public class DapperContext
    {
        private readonly AppSettingsData _appSettings;

        public DapperContext(AppSettingsData appSettings) => _appSettings = appSettings;

        public IDbConnection CreateConnection()
        {
            switch (_appSettings.Database.DatabaseType)
            {
                case DatabaseType.SqLite:
                    return new SqliteConnection(_appSettings.Database.ConnectionString);
                case DatabaseType.SqlServer:
                    return new SqlConnection(_appSettings.Database.ConnectionString);
                case DatabaseType.PostgreSQL:
                    return new NpgsqlConnection(_appSettings.Database.ConnectionString);
                default:
                    throw new Exception("A valid database type must be provided.");
            }
        }
    }
}
