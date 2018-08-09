using System;
using System.Data;
using System.Data.Common;
using AdoLogger.Logging;

namespace AdoLogger
{
    public partial class LoggingDbConnection : DbConnection
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private DbConnection _connection;

        public DbConnection WrappedConnection => _connection;

        public LoggingDbConnection(DbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public override string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        /// <summary>
        /// Gets the time to wait while establishing a connection before terminating the attempt and generating an error.
        /// </summary>
        public override int ConnectionTimeout => _connection.ConnectionTimeout;

        /// <summary>
        /// Gets the name of the current database after a connection is opened, 
        /// or the database name specified in the connection string before the connection is opened.
        /// </summary>
        public override string Database => _connection.Database;

        /// <summary>
        /// Gets the name of the database server to which to connect.
        /// </summary>
        public override string DataSource => _connection.DataSource;

        /// <summary>
        /// Gets a string that represents the version of the server to which the object is connected.
        /// </summary>
        public override string ServerVersion => _connection.ServerVersion;

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        public override ConnectionState State => _connection.State;

        /// <summary>
        /// Changes the current database for an open connection.
        /// </summary>
        /// <param name="databaseName">The new database name.</param>
        public override void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);

        /// <summary>
        /// Closes the connection to the database.
        /// This is the preferred method of closing any open connection.
        /// </summary>
        public override void Close()
        {
            Logger.Trace("Closing {0} connection to {1}", _connection.GetType(), DataSource);
            try
            { 
                _connection.Close();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception closing {0} connection to {1}", e, _connection.GetType(), DataSource);
                throw;
            }
}

        /// <summary>
        /// Opens a database connection with the settings specified by the <see cref="ConnectionString"/>.
        /// </summary>
        public override void Open()
        {
            Logger.Trace("Opening {0} connection to {1}", _connection.GetType(), DataSource);
            try
            {
                _connection.Open();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception opening {0} connection to {1}", e, _connection.GetType(), DataSource);
                throw;
            }
        }

        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        /// <returns>An object representing the new transaction.</returns>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new LoggingDbTransaction(_connection.BeginTransaction(isolationLevel), this);
        }

        /// <summary>
        /// Creates and returns a <see cref="DbCommand"/> object associated with the current connection.
        /// </summary>
        /// <returns>A <see cref="LoggingDbCommand"/> wrapping the created <see cref="DbCommand"/>.</returns>
        protected override DbCommand CreateDbCommand() => new LoggingDbCommand(_connection.CreateCommand(), this);

    }
}
