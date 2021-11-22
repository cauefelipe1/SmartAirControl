using Dapper;
using SmartAirControl.API.Core.Settings;
using System;
using System.Data;
using System.IO;

namespace SmartAirControl.API.Database
{
    public class SqLiteDatabaseInitializer : IDatabaseInitializer
    {
        private readonly AppSettingsData _appSettingsData;
        private readonly DapperContext _dapperContext;

        public SqLiteDatabaseInitializer(DapperContext dapperContext, AppSettingsData appSettingsData)
        {
            _dapperContext = dapperContext;
            _appSettingsData = appSettingsData;
        }

        public void InitializeDatabase(bool forceCreate = false)
        {
            if (string.Equals(_appSettingsData.Database.DatabaseName, ":memory:"))
            {
                CreateDatabase();
            }
            else
            {
                if (forceCreate)
                    File.Delete(_appSettingsData.Database.DatabaseName);

                if (!File.Exists(_appSettingsData.Database.DatabaseName))
                    CreateDatabase();
            }
        }

        private void CreateDatabase()
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

        #region Create_Tables

        #region User_Table
        private void CreateUserTable(IDbConnection conn)
        {
            const string SQL = @"
                CREATE TABLE User
                (
                    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username VARCHAR(200) NOT NULL,
                    Password VARCHAR(200) NOT NULL,
                    UserType INTEGER NOT NULL,
                    InsertTS DATETIME
                )";

            conn.Execute(SQL);

            FillDefaultValuesUserTable(conn);
        }

        private void FillDefaultValuesUserTable(IDbConnection conn)
        {
            const string INSERT_SQL = @"
                INSERT INTO User (
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
                    DeviceId INTEGER PRIMARY KEY AUTOINCREMENT,
                    SerialNumber VARCHAR(200) NOT NULL,
                    DeviceSecret VARCHAR(200) NOT NULL,
                    DeviceModel VARCHAR(200) NOT NULL,
                    InsertTS DATETIME NOT NULL
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
                    DeviceRegistrationId INTEGER PRIMARY KEY AUTOINCREMENT,
                    DeviceId INTEGER NOT NULL,
                    FirmwareVersion VARCHAR(200) NOT NULL,
                    RegistrationTS DATETIME NOT NULL
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
                    DeviceReportId INTEGER PRIMARY KEY AUTOINCREMENT,
                    DeviceId INTEGER NOT NULL,
                    DeviceRegistrationId INTEGER NOT NULL,
                    ReportType INTEGER NULL,
                    SensorType INTEGER NULL,
                    SensorValue DECIMAL(10, 2) NULL,
                    HealthStatus VARCHAR(15) NULL,
                    Message VARCHAR(550) NULL,
                    DeviceReadTS DATETIME NULL,
                    InsertTS DATETIME NOT NULL
                )";

            conn.Execute(SQL);
        }
        #endregion Device_Report_Table

        #region Device_Alert_Table
        private void CreateDeviceAlertTable(IDbConnection conn)
        {
            const string SQL = @"
                CREATE TABLE DeviceAlert(
                    DeviceAlertId INTEGER PRIMARY KEY AUTOINCREMENT,
                    DeviceId INTEGER NOT NULL,
                    Type INTEGER NOT NULL,
                    InitialDeviceReportId INTEGER NOT NULL,
                    LatestDeviceReportId INTEGER NOT NULL,
                    InsertTS DATETIME NOT NULL,
                    ResolveTS DATETIME NULL,
                    ViewStatus INTEGER NOT NULL,
                    ResolveStatus INTEGER NOT NULL,
                    AlertMessage VARCHAR(500) NOT NULL
                )";

            conn.Execute(SQL);
        }
        #endregion Device_Alert_Table

        #endregion Create_Tables
    }
}
