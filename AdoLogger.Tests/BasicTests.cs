using System;
using AdoLogger;
using Dapper;
using Microsoft.Data.Sqlite;
using Serilog;
using Xunit;

namespace AdoLogger.Tests
{
    public class BasicTests
    {
        public BasicTests()
        {
            Log.Logger = new LoggerConfiguration()
                //.WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        [Fact]
        public void Parameter()
        {
            SqliteConnection conn = new SqliteConnection("Data Source= :memory:; Cache = Shared");
            var loggingConn = new LoggingDbConnection(conn);

            loggingConn.Open();
            loggingConn.Execute("CREATE TABLE TestTable (Id int null)");

            var obj = new[] { new { Id = 1 }, new { Id = 2 } };
            loggingConn.Execute("INSERT INTO TestTable VALUES (@Id)", obj);
            loggingConn.Close();
        }

        [Fact]
        public void Exception()
        {
            SqliteConnection conn = new SqliteConnection("Data Source= :memory:; Cache = Shared");
            var loggingConn = new LoggingDbConnection(conn);
            try
            {
                loggingConn.Execute("Invalid SQL");
            }
            catch(Exception)
            {

            }
        }
    }
}
