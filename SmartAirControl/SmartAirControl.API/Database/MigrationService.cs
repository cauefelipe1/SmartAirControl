using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartAirControl.API.Core.Settings;
using System;

namespace SmartAirControl.API.Database
{
    public static class MigrationService
    {
        public static IHost InitializeDatabase(this IHost host, bool forceCreate = false)
        {
            using (var scope = host.Services.CreateScope())
            {
                var settings = scope.ServiceProvider.GetRequiredService<AppSettingsData>();

                IDatabaseInitializer dbInitializer = GetDatabaseInitializerClass(settings.Database.DatabaseType, scope);

                dbInitializer.InitializeDatabase(forceCreate);
            }

            return host;
        }

        private static IDatabaseInitializer GetDatabaseInitializerClass(DatabaseType databaseType, IServiceScope serviceScope)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    return serviceScope.ServiceProvider.GetRequiredService<SqlServerDatabaseInitializer>();

                case DatabaseType.SqLite:
                    return serviceScope.ServiceProvider.GetRequiredService<SqLiteDatabaseInitializer>();

                case DatabaseType.PostgreSQL:
                    return serviceScope.ServiceProvider.GetRequiredService<PostgreSqlDatabaseInitializer>();

                default:
                    throw new Exception($"Missing implementation for initializer for database type {databaseType}.");
            }
        }
    }
}
