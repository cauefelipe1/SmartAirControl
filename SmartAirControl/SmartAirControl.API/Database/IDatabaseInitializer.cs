using SmartAirControl.API.Core.Settings;

namespace SmartAirControl.API.Database
{
    public interface IDatabaseInitializer
    {
        void InitializeDatabase(bool forceCreate = false);
    }
}
