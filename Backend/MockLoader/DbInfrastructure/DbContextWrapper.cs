using Decrypt.DataAccess;
using Decrypt.DataAccess.DataSources;
using Decrypt.DataAccess.Entities;
using FluentResults;
using MockLoader.Factories;

namespace MockLoader.DbInfrastructure
{
    /// <summary>
    /// Simple wrapper to work with DataAccess layer in Loader utility
    /// </summary>
    internal class DbContextWrapper(string connectionString)
    {
        private readonly DecryptContext _context = DecryptContextFactory.Create(connectionString);

        public DecryptContext Context => _context;

        /// <summary>
        /// Diagnostic of the DB connection
        /// </summary>
        public async Task<Result> TryConnectionAsync()
        {
            try
            {
                var result = await _context.Database.CanConnectAsync();

                return result ? Result.Ok() : Result.Fail("Can't connect");
            }
            catch (Exception e)
            {
                return Result.Fail($"Failed to connect. Message: {e.Message}");
            }
        }

        /// <summary>
        /// Get particular DataSource based on the context 
        /// </summary>
        public GenericDataSource<T> GetDataSource<T>() where T : BaseEntity
        {
            return new GenericDataSource<T>(_context);
        }
    }
}
