using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using EDaemonWebServer.Utils.Enums;

namespace EDaemonWebServer.Repositories
{
    /// <summary>
    /// Base repository with shared utilities for other repositories.
    /// </summary>
    /// <remarks>
    /// Provides the connection string read from the "DATABASE_PATH" environment variable and
    /// helpers to convert types returned from the database (for example, integers
    /// that represent enums or boolean flags stored as 0/1).
    /// The comments below serve as a reference when reusing these methods across
    /// different components of the application.
    /// </remarks>
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of <see cref="BaseRepository"/>.
        /// </summary>
        /// <param name="configuration">Configuration object (not used at the moment,
        /// kept for future compatibility).</param>
        /// <remarks>
        /// The connection string is built from the "DATABASE_PATH" environment variable
        /// in the format accepted by the SQLite provider: "Data Source={path}".
        /// If the variable is not set, the connection string will contain
        /// "Data Source=" (empty) — repositories consuming this service
        /// should validate database availability before executing commands.
        /// </remarks>
        public BaseRepository(IConfiguration configuration)
        {
            // Prefer the environment variable for production overrides and
            // fall back to the configuration value from appsettings (e.g. appsettings.Development.json).
            var dbPath = Environment.GetEnvironmentVariable("DATABASE_PATH");

            if (string.IsNullOrWhiteSpace(dbPath))
            {
                // Use the indexer to read configuration to allow simple mocks in tests
                // (e.g. Mock<IConfiguration>().SetupGet(c => c["DatabasePath"]).Returns(...)).
                dbPath = configuration?["DatabasePath"];
            }

            if (string.IsNullOrWhiteSpace(dbPath))
                throw new InvalidOperationException("Invalid database path.");

            _connectionString = $"Data Source={dbPath}";
        }

        /// <summary>
        /// Creates and returns a new <see cref="SqliteConnection"/> instance.
        /// </summary>
        /// <returns>A <see cref="SqliteConnection"/> initialized with the connection string.</returns>
        /// <remarks>
        /// The caller is responsible for opening and disposing the connection (use a <c>using</c>
        /// block or call <c>Dispose()</c>). This method only encapsulates connection creation
        /// to keep the connection string centralized.
        /// </remarks>
        protected SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        /// <summary>
        /// Converts a raw value (typically coming from the database) to the <see cref="AttributeType"/> enum.
        /// </summary>
        /// <param name="raw">Raw value to be converted. Accepts numeric values or null.</param>
        /// <returns>
        /// The corresponding <see cref="AttributeType"/> value when the integer is valid;
        /// otherwise returns <see cref="AttributeType.None"/>.
        /// </returns>
        /// <remarks>
        /// - If <paramref name="raw"/> is <c>null</c>, returns <c>AttributeType.None</c>.
        /// - If the conversion fails or the integer does not map to an enum member,
        ///   returns <c>AttributeType.None</c>.
        /// - Typical usage: when integer columns in the database represent attributes by index.
        /// </remarks>
        protected AttributeType IntToBaseAttribute(object? raw)
        {
            int intVal;

            if (raw is null)
                return AttributeType.None;

            try
            {
                intVal = Convert.ToInt32(raw);
            }
            catch
            {
                intVal = (int)AttributeType.None;
            }

            if (Enum.IsDefined(typeof(AttributeType), intVal))
                return (AttributeType)intVal;

            return AttributeType.None;
        }

        /// <summary>
        /// Converts a raw value (typically an integer) to a boolean.
        /// </summary>
        /// <param name="raw">Raw value to be converted. Numeric values different from zero are considered <c>true</c>.</param>
        /// <returns><c>true</c> if the converted integer is different from zero; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// - If <paramref name="raw"/> is <c>null</c> or conversion fails, returns <c>false</c>.
        /// - Typical usage: columns that store flags as 0/1 in the SQLite database.
        /// </remarks>
        protected bool IntToBool(object? raw)
        {
            if (raw is null)
                return false;

            try
            {
                var intVal = Convert.ToInt32(raw);
                return intVal != 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
