using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using SmartAirControl.API.Core.Settings;
using System;
using System.Data;

namespace SmartAirControl.API.Database
{
    public class PostgreSqlDatabaseInitializer : IDatabaseInitializer
    {
        private readonly AppSettingsData _appSettingsData;
        private readonly DapperContext _dapperContext;

        public PostgreSqlDatabaseInitializer(DapperContext dapperContext, AppSettingsData appSettingsData)
        {
            _dapperContext = dapperContext;
            _appSettingsData = appSettingsData;
        }

        public void InitializeDatabase(bool forceCreate = false)
        {
            if (forceCreate && DatabaseExists())
                DropDatabase();

            if (!DatabaseExists())
                CreateDatabase();
        }

        private bool DatabaseExists()
        {
            if (_appSettingsData.Database is PostgreConfig dbConfig)
            {
                using (var conn = new NpgsqlConnection(dbConfig.PostgresConnectionString))
                {
                    conn.Open();

                    const string SQL = @"SELECT datname FROM pg_catalog.pg_database WHERE lower(datname) = lower(@DatabaseName)";

                    string dbname = conn.QuerySingleOrDefault<string>(SQL, new { _appSettingsData.Database.DatabaseName });

                    return dbname is not null;
                }
            }
            else
                throw new Exception("The database is not PostgreSQL.");
        }

        private void CreateDatabase()
        {
            if (_appSettingsData.Database is PostgreConfig dbConfig)
            {
                string sql = $"CREATE DATABASE { _appSettingsData.Database.DatabaseName }";

                using (var conn = new NpgsqlConnection(dbConfig.PostgresConnectionString))
                {
                    conn.Open();

                    conn.Execute(sql);
                }

                CreateTables();
            }
            else            
                throw new Exception("The database is not PostgreSQL.");
        }

        private void DropDatabase()
        {
            if (_appSettingsData.Database is PostgreConfig dbConfig)
            {
                string sql = $"DROP DATABASE { _appSettingsData.Database.DatabaseName }";

                using (var conn = new NpgsqlConnection(dbConfig.PostgresConnectionString))
                {
                    conn.Open();

                    conn.Execute(sql);
                }
            }
            else
                throw new Exception("The database is not PostgreSQL.");
        }

        #region Create_Tables
        private void CreateTables()
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                conn.Open();

                CreateUserTable(conn);
                CreateDeviceTable(conn);
                CreateDeviceRegistrationTable(conn);
                CreateDeviceReportTable(conn);
                CreateDeviceAlertTable(conn);
            }
        }

        #region User_Table
        private void CreateUserTable(IDbConnection conn)
        {
            const string SQL = @"
                CREATE TABLE ""user""
                (
                    UserId INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                    Username VARCHAR(200) NOT NULL,
                    Password VARCHAR(200) NOT NULL,
                    UserType INT NOT NULL,
                    InsertTS TIMESTAMP
                )";

            conn.Execute(SQL);

            FillDefaultValuesUserTable(conn);
        }

        private void FillDefaultValuesUserTable(IDbConnection conn)
        {
            const string INSERT_SQL = @"
                INSERT INTO ""user"" (
                    UserName, 
                    Password, 
                    UserType, 
                    InsertTS
                ) 
                VALUES (
                    @UserName, 
                    @Password, 
                    @UserType, 
                    @InsertTS
                )";

            var param = DataExampleProvider.GetUsersDataExample();

            conn.Execute(INSERT_SQL, param);
        }
        #endregion User_Table

        #region Device_Table
        private void CreateDeviceTable(IDbConnection conn)
        {
            const string SQL = @"
                CREATE TABLE Device
                (
                    DeviceId INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                    SerialNumber VARCHAR(200) NOT NULL,
                    DeviceSecret VARCHAR(200) NOT NULL,
                    DeviceModel VARCHAR(200) NOT NULL,
                    InsertTS TIMESTAMP NOT NULL
                )";

            conn.Execute(SQL);

            FillDefaultValuesDeviceTable(conn);
        }

        private void FillDefaultValuesDeviceTable(IDbConnection conn)
        {
            const string INSERT_SQL = @"
                INSERT INTO Device (
                    SerialNumber, 
                    DeviceSecret, 
                    DeviceModel, 
                    InsertTS
                ) 
                VALUES (
                    @SerialNumber, 
                    @DeviceSecret, 
                    @DeviceModel, 
                    @InsertTS
                )";

            var param = DataExampleProvider.GetDevicesDataExample();

            conn.Execute(INSERT_SQL, param);
        }
        #endregion Device_Table

        #region Device_Register_Attempt_Table
        private void CreateDeviceRegistrationTable(IDbConnection conn)
        {
            const string SQL = @"
                CREATE TABLE DeviceRegistration
                (
                    DeviceRegistrationId INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                    DeviceId INT NOT NULL,
                    FirmwareVersion VARCHAR(200) NOT NULL,
                    RegistrationTS TIMESTAMP NOT NULL
                )";

            conn.Execute(SQL);
        }
        #endregion Device_Register_Attempt_Table

        #region Device_Report_Table
        private void CreateDeviceReportTable(IDbConnection conn)
        {
            const string SQL = @"
                CREATE TABLE DeviceReport
                (
                    DeviceReportId INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                    DeviceId INT NOT NULL,
                    DeviceRegistrationId INT NOT NULL,
                    ReportType INT NULL,
                    SensorType INT NULL,
                    SensorValue DECIMAL(10, 2) NULL,
                    HealthStatus VARCHAR(15) NULL,
                    Message VARCHAR(550) NULL,
                    DeviceReadTS TIMESTAMP NULL,
                    InsertTS TIMESTAMP NOT NULL
                )";

            conn.Execute(SQL);
        }
        #endregion Device_Report_Table

        #region Device_Alert_Table
        private void CreateDeviceAlertTable(IDbConnection conn)
        {
            const string SQL = @"
                CREATE TABLE DeviceAlert(
                    DeviceAlertId INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                    DeviceId INT NOT NULL,
                    Type INT NOT NULL,
                    InitialDeviceReportId INT NOT NULL,
                    LatestDeviceReportId INT NOT NULL,
                    InsertTS TIMESTAMP NOT NULL,
                    ResolveTS TIMESTAMP NULL,
                    ViewStatus INT NOT NULL,
                    ResolveStatus INT NOT NULL,
                    AlertMessage VARCHAR(500) NOT NULL
                )";

            conn.Execute(SQL);
        }
        #endregion Device_Alert_Table

        #endregion Create_Tables
    }
}
