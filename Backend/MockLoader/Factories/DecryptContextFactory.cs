using Decrypt.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MockLoader.Factories
{
    /// <summary>
    /// Build Db context with given connection settings 
    /// </summary>
    public static class DecryptContextFactory
    {
        /// <summary>
        /// Create Db Context
        /// </summary>
        /// <param name="connectionString">DB connection string</param>
        public static DecryptContext Create(string connectionString)
        {
            var options = new DbContextOptionsBuilder<DecryptContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new DecryptContext(options);
        }
    }
}
