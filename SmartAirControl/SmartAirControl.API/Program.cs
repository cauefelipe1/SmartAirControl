using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SmartAirControl.API.Database;

namespace SmartAirControl.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool forceCreateDatabase = true;
            if (args.Length > 0)
            {
                string[] forceCreateDatabaseValue = args[0].Split('=');
                
                if(forceCreateDatabaseValue.Length > 1 && int.TryParse(forceCreateDatabaseValue[1].Trim(), out int v))
                    forceCreateDatabase = v == 1;
            }

            CreateHostBuilder(args)
                .Build()
                .InitializeDatabase(forceCreateDatabase)
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
