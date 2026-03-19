using Decrypt.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Decrypt.Api.Extensions
{
    public static class DoMigrateExtension
    {
        public static WebApplication DoMigrate(this WebApplication app, string[] args)
        {
            //To suppress automatic migration use  --disable_migrate param
            if (Array.Find(args, element => element == "--disable_migrate") != null)
                return app;


            using var serviceScope = (app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope());

            var context = serviceScope.ServiceProvider.GetService<DecryptContext>();

            if (context == null)
                throw new InvalidOperationException("Can't resolve DbContext dependency");

            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new InvalidOperationException($"Error during migration, thr code is {ex.HResult}, the message is {ex.Message}");
            }

            return app;
        }
    }
}
