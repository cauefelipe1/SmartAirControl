using Microsoft.Extensions.Configuration;
using System;

namespace SmartAirControl.API.Core.Settings
{
    public enum DatabaseType : byte
    {
        SqLite = 1,
        SqlServer = 2,
        PostgreSQL = 3
    }

    public sealed class AppSettingsData
    {
        public JwtSettings Jwt { get; }

        public DatabaseConfig Database { get; }

        public AppSettingsData(IConfiguration configuration)
        {
            Database = GetDatabaseConfig(configuration);
            Jwt = new JwtSettings(configuration);
        }

        private DatabaseConfig GetDatabaseConfig(IConfiguration configuration)
        {
            string databaseType = configuration["Database:DatabaseType"];

            var type = (DatabaseType)int.Parse(databaseType);

            switch(type)
            {
                case DatabaseType.SqlServer:
                    return new SqlSeverConfig(configuration);

                case DatabaseType.SqLite:
                    return new SqLiteConfig(configuration);

                case DatabaseType.PostgreSQL:
                    return new PostgreConfig(configuration);

                default:
                    throw new Exception($"Missing implementation for database type {databaseType}.");
            }
        }
    }

    public sealed class JwtSettings
    {
        public string Secret { get; }

        public string Issuer { get; }

        public string Audience { get; }

        public int Expiration { get; }

        internal JwtSettings(IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JwtSettings");

            Secret = jwtSection[nameof(Secret)];
            Issuer = jwtSection[nameof(Issuer)];
            Audience = jwtSection[nameof(Audience)];
            Expiration = int.Parse(jwtSection[nameof(Expiration)]);
        }
    }

    public abstract class DatabaseConfig
    {
        public abstract DatabaseType DatabaseType { get; }

        public string DatabaseName { get; }

        public string ServerAddress { get; }

        public string Username { get; }

        protected string Password { get; }

        protected int Port { get; }

        public string ConnectionString { get; protected set; }

        protected abstract void SetConnectionString();

        internal DatabaseConfig(IConfiguration configuration)
        {
            var databaseSection = configuration.GetSection("Database");

            DatabaseName = databaseSection[nameof(DatabaseName)];
            ServerAddress = databaseSection[nameof(ServerAddress)];
            Username = databaseSection[nameof(Username)];
            Password = databaseSection[nameof(Password)];
            
            if (int.TryParse(databaseSection[nameof(Port)], out int port))
                Port = port;

            SetConnectionString();
        }
    }

    public sealed class SqLiteConfig : DatabaseConfig
    {
        public override DatabaseType DatabaseType => DatabaseType.SqLite;

        internal SqLiteConfig(IConfiguration configuration) : base(configuration) { }

        protected override void SetConnectionString() => ConnectionString = @$"Data Source={DatabaseName}";
    }

    public sealed class SqlSeverConfig : DatabaseConfig
    {   
        public override DatabaseType DatabaseType => DatabaseType.SqlServer;

        public string MasterConnectionString { get; private set; }

        internal SqlSeverConfig(IConfiguration configuration) : base(configuration) { }

        protected override void SetConnectionString()
        {
            int lPort = 1433;

            if (Port > 0)
                lPort = Port;

            ConnectionString = @$"Server={ServerAddress},{lPort};Database={DatabaseName};User ID={Username};Password={Password};TrustServerCertificate=True;Encrypt=True;";
            MasterConnectionString = @$"Server={ServerAddress},{lPort};Database=master;User ID={Username};Password={Password};TrustServerCertificate=True;Encrypt=True;";
        }
    }

    public sealed class PostgreConfig : DatabaseConfig
    {
        public override DatabaseType DatabaseType => DatabaseType.PostgreSQL;

        public string PostgresConnectionString { get; private set; }

        internal PostgreConfig(IConfiguration configuration) : base(configuration) { }

        protected override void SetConnectionString()
        {
            int lPort = 5432;

            if (Port > 0)
                lPort = Port;

            ConnectionString = @$"Server={ServerAddress};Port={lPort};Database={DatabaseName};User ID={Username};Password={Password};";
            PostgresConnectionString = @$"Server={ServerAddress};Port={lPort};User ID={Username};Password={Password};";
        }
    }
}
