using Serilog;
using Serilog.Core;
using Serilog.Events;
using TaskRunner.Core.Logging.Interfaces;

namespace TaskRunner.Core.Logging
{
  public partial class AppLogger : IAppLogger
  {
    private readonly Logger _logger;

    public AppLogger()
    {
      var configuration = new LoggerConfiguration();

      configuration.WriteTo.Console(
        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u4}] ({Caller}) {Message:lj}{NewLine}{Exception}"
      );

      configuration.MinimumLevel.Verbose();
      configuration.Enrich.WithCaller();

      configuration.WriteTo.File(
        path: "./logs/TaskBuilder_.log",
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 1024 * 1024 * 20,
        retainedFileCountLimit: 5,
        restrictedToMinimumLevel: LogEventLevel.Verbose,
        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u4}] ({Caller}) {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day
      );

      _logger = configuration.CreateLogger();
    }


    // Debug
    public void Debug(string message)
    {
      _logger.Debug(message);
    }

    public void Debug<T>(string message, T p1)
    {
      _logger.Debug(message, p1);
    }

    public void Debug<T1, T2>(string message, T1 p1, T2 p2)
    {
      _logger.Debug(message, p1, p2);
    }


    // Info
    public void Info(string message)
    {
      _logger.Information(message);
    }

    public void Info<T>(string message, T p1)
    {
      _logger.Information(message, p1);
    }

    public void Info<T1, T2>(string message, T1 p1, T2 p2)
    {
      _logger.Information(message, p1, p2);
    }


    // Warn
    public void Warn<T1, T2>(string message, T1 p1, T2 p2)
    {
      _logger.Warning(message, p1, p2);
    }

    public void Warn<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3)
    {
      _logger.Warning(message, p1, p2, p3);
    }

    public void Warn<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4)
    {
      _logger.Warning(message, p1, p2, p3, p4);
    }

    // Error
    public void Error<T1>(string message, T1 p1)
    {
      _logger.Error(message, p1);
    }

    public void Error<T1, T2>(string message, T1 p1, T2 p2)
    {
      _logger.Error(message, p1, p2);
    }
  }
}
