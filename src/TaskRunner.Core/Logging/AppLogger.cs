using Serilog;
using Serilog.Core;
using Serilog.Events;
using TaskRunner.Shared.Interfaces.Logging;

namespace TaskRunner.Core.Logging
{
  public class AppLogger : IAppLogger
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


    // TODO: [CURRENT] (AppLogger) Add Verbose() to logger - this can be used for noisy important DEBUG messages


    // Debug
    public void Debug(string template)
    {
      _logger.Debug(template);
    }

    public void Debug<T>(string template, T p1)
    {
      _logger.Debug(template, p1);
    }

    public void Debug<T1, T2>(string template, T1 p1, T2 p2)
    {
      _logger.Debug(template, p1, p2);
    }

    public void Debug<T1, T2, T3>(string template, T1 p1, T2 p2, T3 p3)
    {
      _logger.Debug(template, p1, p2, p3);
    }


    // Info
    public void Info(string template)
    {
      _logger.Information(template);
    }

    public void Info<T>(string template, T p1)
    {
      _logger.Information(template, p1);
    }

    public void Info<T1, T2>(string template, T1 p1, T2 p2)
    {
      _logger.Information(template, p1, p2);
    }


    // Warn
    public void Warn<T1, T2>(string template, T1 p1, T2 p2)
    {
      _logger.Warning(template, p1, p2);
    }

    public void Warn<T1, T2, T3>(string template, T1 p1, T2 p2, T3 p3)
    {
      _logger.Warning(template, p1, p2, p3);
    }

    public void Warn<T1, T2, T3, T4>(string template, T1 p1, T2 p2, T3 p3, T4 p4)
    {
      _logger.Warning(template, p1, p2, p3, p4);
    }


    // Error
    public void Error<T1>(string template, T1 p1)
    {
      _logger.Error(template, p1);
    }

    public void Error<T1, T2>(string template, T1 p1, T2 p2)
    {
      _logger.Error(template, p1, p2);
    }

    public void Error<T1, T2, T3>(string template, T1 p1, T2 p2, T3 p3)
    {
      _logger.Error(template, p1, p2, p3);
    }
  }
}
