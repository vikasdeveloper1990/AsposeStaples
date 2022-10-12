using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace PrintRobotConsoleApp.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration configuration;

        public ConfigurationManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetSqlConnectionString()
        {
            return configuration.GetConnectionString("SqlConnectionString");
        }
    }
}
