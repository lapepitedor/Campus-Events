using Microsoft.Extensions.Options;
using Npgsql;

namespace Campus_Events.Persistence
{
    public class DbConnectionFactory
    {
        private ILogger<DbConnectionFactory> logger;
        private DatabaseSettings settings;

        public DbConnectionFactory(ILogger<DbConnectionFactory> logger, IOptions<DatabaseSettings> settings)
        {
            this.logger = logger;
            this.settings = settings.Value;
        }

        public NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(settings.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
