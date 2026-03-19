using FluentResults;
using Microsoft.Extensions.Configuration;

namespace MockLoader.Helpers
{
    internal static class SettingsHelper
    {
        /// <summary>
        /// To extract connection string from the settings
        /// </summary>
        public static Result<string> GetConnectionString()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = configuration.GetConnectionString("ConnectionString");

                if (string.IsNullOrEmpty(connectionString))
                {
                    return Result.Fail("Connection string not found");
                }

                return Result.Ok(connectionString);
            }
            catch (Exception e)
            {
                return Result.Fail($"Failed to extract connection string. Message: {e.Message}");
            }
        }
    }
}
