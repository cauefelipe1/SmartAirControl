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

            var param = new[]
            {
                //Users
                new {
                    UserId = 1,
                    UserName = "admin",
                    Password = "123456",
                    UserType = 1,
                    InsertTS = DateTime.UtcNow
                },
                new {
                    UserId = 2,
                    UserName = "user",
                    Password = "123456",
                    UserType = 2,
                    InsertTS = DateTime.UtcNow
                }
            };

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

            var param = new[]
            {   
                //Devices
                new {
                    DeviceId = 1,
                    SerialNumber = "H75QfssMqqwHQkNCYpbn96S4LbubWx",
                    DeviceSecret = "jQL5ddEQGu8RuwVYWX5c8vm4W",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 2,
                    SerialNumber = "BVTr7eu5JTLtthZSkqFLz3pRvXhEKc",
                    DeviceSecret = "8X4jrnkgURdbvmCCtx9z6nyTS",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 3,
                    SerialNumber = "M7wGP39qnkBKzhWyRF8BXgW5zvrLfE7z",
                    DeviceSecret = "fTsyqumzQMLgSNxZ4UUbh87e46L",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 4,
                    SerialNumber = "xESYN2xKHshAreFdG9CGbFkvqbtA7R4a",
                    DeviceSecret = "N2xKHshAreFdG9CGbF",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 5,
                    SerialNumber = "nen6Be83CzJu2DWjPYUvnRjqY4tpevvk",
                    DeviceSecret = "nen6Be83CzJu2DWjPvvk",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 6,
                    SerialNumber = "bnDuLtgs2cnxda5JT6SHG6khpPfJsPg4",
                    DeviceSecret = "bnDuLts2caSpPfJs",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 7,
                    SerialNumber = "6qw2HkM2em32eZxENSH3c7TBzGnQGeqA",
                    DeviceSecret = "m32eZxENSH3c7TBzGn",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 8,
                    SerialNumber = "GQvwWHeaBsQSdu5BmxvhhQfv3wGgFU45",
                    DeviceSecret = "wWHsQSdu5BmxvhhQfv3wGgFU",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 9,
                    SerialNumber = "VF73AcZpCcQ5Pw64zt5mSXaXpMyRdf6M",
                    DeviceSecret = "srewrefhftg45",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 10,
                    SerialNumber = "QSFRg674UEb3rBNmNmujSBPtDjc85FkZ",
                    DeviceSecret = "rBNmNmujSBPtDjc",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                }
            };

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
