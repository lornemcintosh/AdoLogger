using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AdoLogger.Logging;


namespace AdoLogger
{
    public partial class LoggingDbCommand : DbCommand
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private DbCommand _command;
        private DbConnection _connection;
        private DbTransaction _transaction;
        

        public LoggingDbCommand(DbCommand command, DbConnection connection)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _connection = connection;
        }

        /// <summary>
        /// Gets or sets the text command to run against the data source.
        /// </summary>
        public override string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        /// <summary>
        /// Gets or sets the command timeout.
        /// </summary>
        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        /// <summary>
        /// Gets or sets the command type.
        /// </summary>
        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        /// <summary>
        /// Gets or sets the database connection.
        /// </summary>
        protected override DbConnection DbConnection
        {
            get => _connection;
            set
            {
                _connection = value;
                if (value is LoggingDbConnection profiledConn)
                {
                    _command.Connection = profiledConn.WrappedConnection;
                }
                else
                {
                    _command.Connection = value;
                }
            }
        }

        /// <summary>
        /// Gets the database parameter collection.
        /// </summary>
        protected override DbParameterCollection DbParameterCollection => _command.Parameters;

        /// <summary>
        /// Gets or sets the database transaction.
        /// </summary>
        protected override DbTransaction DbTransaction
        {
            get => _transaction;
            set
            {
                _transaction = value;
                var awesomeTran = value as LoggingDbTransaction;
                _command.Transaction = awesomeTran == null ? value : awesomeTran.WrappedTransaction;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command is design time visible.
        /// </summary>
        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }

        /// <summary>
        /// Gets or sets the updated row source.
        /// </summary>
        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        /// <summary>
        /// Executes a database data reader.
        /// </summary>
        /// <param name="behavior">The command behavior to use.</param>
        /// <returns>The resulting <see cref="DbDataReader"/>.</returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            DbDataReader result = null;
            LogQuery();
            try
            {
                result = _command.ExecuteReader(behavior);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception", e);
                throw;
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// Executes a database data reader asynchronously.
        /// </summary>
        /// <param name="behavior">The command behavior to use.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for this async operation.</param>
        /// <returns>The resulting <see cref="DbDataReader"/>.</returns>
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            DbDataReader result = null;
            LogQuery();
            try
            {
                result = await _command.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception", e);
                throw;
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// Executes a SQL statement against a connection object.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteNonQuery()
        {
            int result;
            LogQuery();
            try
            {
                result = _command.ExecuteNonQuery();
                Logger.Debug("Affected {0} rows", result);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception", e);
                throw;
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection object asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for this async operation.</param>
        /// <returns>The number of rows affected.</returns>
        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            int result;
            LogQuery();
            try
            {
                result = await _command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                Logger.Debug("Affected {0} rows", result);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception", e);
                throw;
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. 
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <returns>The first column of the first row in the result set.</returns>
        public override object ExecuteScalar()
        {
            object result;
            LogQuery();
            try
            {
                result = _command.ExecuteScalar();
                Logger.Debug("Scalar result: {@result}", result);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception", e);
                throw;
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// Asynchronously executes the query, and returns the first column of the first row in the result set returned by the query. 
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for this async operation.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            object result;
            LogQuery();
            try
            {
                result = await _command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                Logger.Debug("Scalar result: {@result}", result);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception", e);
                throw;
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// Attempts to cancels the execution of this command.
        /// </summary>
        public override void Cancel() => _command.Cancel();

        /// <summary>
        /// Creates a prepared (or compiled) version of the command on the data source.
        /// </summary>
        public override void Prepare() => _command.Prepare();

        /// <summary>
        /// Creates a new instance of an <see cref="DbParameter"/> object.
        /// </summary>
        /// <returns>The <see cref="DbParameter"/>.</returns>
        protected override DbParameter CreateDbParameter() => _command.CreateParameter();

        /// <summary>
        /// Releases all resources used by this command.
        /// </summary>
        /// <param name="disposing">false if this is being disposed in a <c>finalizer</c>.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _command != null)
            {
                _command.Dispose();
            }
            _command = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the internal command.
        /// </summary>
        public DbCommand InternalCommand => _command;

        /// <summary>
        /// Logs the _command text and parameters (if any). This should be called before executing it
        /// </summary>
        private void LogQuery()
        {
            var parameters = _command.Parameters.Cast<DbParameter>().ToDictionary( p => p.ParameterName, p => p.Value);
            Logger.Debug("Server: {dataSource}, SQL: {sql}, Parameters: {@params}",
                _command.Connection.DataSource, _command.CommandText, parameters);
        }
    }
}