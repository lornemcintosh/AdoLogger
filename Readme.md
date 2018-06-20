# AdoLogger

AdoLogger is a simple .NET library that wraps an ADO.NET DbConnection and logs all activity, including SQL statements, and connection open/close events. It uses LibLog so you can use the logging library of your choice (NLog, Log4Net, Serilog or Loupe).


## Getting Started

Just wrap your existing connection (of any type) in an AdoLogger LoggingDbConnection:
```C#
SqliteConnection conn = new SqliteConnection("Data Source= :memory:; Cache = Shared");
var loggingConn = new LoggingDbConnection(conn);
```

## Built With

* [LibLog](https://github.com/damianh/LibLog) - logging abstraction


## License

This project is licensed under the MIT License - see the [LICENSE.TXT](LICENSE.TXT) file for details


## Acknowledgments

* AdoLogger was heavily inspired by [MiniProfiler](https://miniprofiler.com/).
