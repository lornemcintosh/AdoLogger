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
                .MinimumLevel.Verbose()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        [Fact]
        public void Parameter()
        {
            SqliteConnection conn = new SqliteConnection("Data Source= :memory:; Cache = Shared");
            var loggingConn = new LoggingDbConnection(conn);

            loggingConn.Open();
            loggingConn.Execute("CREATE TABLE TestTable (Id int null, Test nvarchar(10) null)");

            var obj = new[] { new { Id = 1, Test = "test1" }, new { Id = 2, Test = "test2" } };
            loggingConn.Execute("INSERT INTO TestTable VALUES (@Id, @Test)", obj);
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
