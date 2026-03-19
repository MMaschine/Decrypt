using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Decrypt.DataAccess.Infrastructure;

/// <summary>
/// Infrastructure to init DB context in design time
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DecryptContext>
{
    public DecryptContext CreateDbContext(string[] args)
    {
        //TODO: for the real app re-write to get values from appsettings.json file and EnvVars 

        var connectionString = "Server=localhost;Database=Decrypt;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<DecryptContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DecryptContext(optionsBuilder.Options);
    }
}