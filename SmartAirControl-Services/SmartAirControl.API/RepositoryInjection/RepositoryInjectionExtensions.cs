using Microsoft.Extensions.DependencyInjection;
using SmartAirControl.API.Core.Settings;
using SmartAirControl.API.Features.Device;
using SmartAirControl.API.Features.DeviceAlert;
using SmartAirControl.API.Features.DeviceReport;
using SmartAirControl.API.Features.User;
using System;

namespace SmartAirControl.API.RepositoryInjection
{
    public static class RepositoryInjectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, AppSettingsData settings)
        {
            var dbType = settings.Database.DatabaseType;

            switch (dbType)
            {
                case DatabaseType.SqLite:
                    services.AddSqLiteRepositories();
                    break;

                case DatabaseType.SqlServer:
                    services.AddSqlServerRepositories();
                    break;

                case DatabaseType.PostgreSQL:
                    services.AddPostgreRepositories();
                    break;

                default:
                    throw new Exception($"Missing implementation for repository injection for database type { dbType }.");
            }

            return services;
        }

        private static void AddSqLiteRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserSqLiteRepository>();
            services.AddSingleton<IDeviceRepository, DeviceSqLiteRepository>();
            services.AddSingleton<IDeviceAlertRepository, DeviceAlertSqLiteRepository>();
            services.AddSingleton<IDeviceReportRepository, DeviceReportSqLiteRepository>();
        }

        private static void AddSqlServerRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserSqlServerRepository>();
            services.AddSingleton<IDeviceRepository, DeviceSqlServerRepository>();
            services.AddSingleton<IDeviceAlertRepository, DeviceAlertSqlServerRepository>();
            services.AddSingleton<IDeviceReportRepository, DeviceReportSqlServerRepository>();
        }

        private static void AddPostgreRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserPostgreSqlRepository>();
            services.AddSingleton<IDeviceRepository, DevicePostgreSqlRepository>();
            services.AddSingleton<IDeviceAlertRepository, DeviceAlertPostgreSqlRepository>();
            services.AddSingleton<IDeviceReportRepository, DeviceReportPostgreSqlRepository>();
        }
    }
}
