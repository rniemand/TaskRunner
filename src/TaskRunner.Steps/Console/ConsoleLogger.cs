using System;
using System.Collections.Generic;
using TaskRunner.Core.Abstractions.Interfaces;
using TaskRunner.Core.Enums;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Mappers;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Console
{
  // TODO: [DOCS] (ConsoleLog) Document this step
  // TODO: [DOCS] (ConsoleLog) Document argument: Severity (Optional - default Info)
  // TODO: [DOCS] (ConsoleLog) Document argument: Message
  // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

  public class ConsoleLogger : TaskStepBase
  {
    private readonly IConsole _console;

    public ConsoleLogger(IAppLogger logger, IConsole console)
      : base(logger, "Console.Log")
    {
      _console = console;

      RegisterInput("Severity", InputValidator.String, false, "Info");
      RegisterInput("Message", InputValidator.String);
    }


    // Public methods
    public override bool Execute(StepContext context)
    {
      // TODO: [TESTS] (ConsoleLog) Add tests

      GetInput("Severity");

      var severity = SeverityMapper.MapSeverity(
        context.GetArgument("Severity"),
        RunnerSeverity.Info
      );

      Log(severity, context.GetArgument("Message"));

      return true;
    }


    // Internal methods
    private void Log(RunnerSeverity severity, string message)
    {
      // TODO: [TESTS] (ConsoleLog) Add tests

      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (severity)
      {
        case RunnerSeverity.Verbose:
          LogVerbose(message);
          return;

        case RunnerSeverity.Debug:
          LogDebug(message);
          return;

        case RunnerSeverity.Info:
          LogInfo(message);
          return;

        case RunnerSeverity.Warn:
          LogWarn(message);
          return;

        default:
          LogError(message);
          break;
      }
    }

    private void LogVerbose(string message)
    {
      // TODO: [TESTS] (ConsoleLog) Add tests

      _console.ForegroundColor = ConsoleColor.Gray;
      _console.BackgroundColor = ConsoleColor.DarkBlue;
      _console.WriteLine($"[VERB] {message} ");
      _console.ResetColor();
    }

    private void LogDebug(string message)
    {
      // TODO: [TESTS] (ConsoleLog) Add tests

      _console.ForegroundColor = ConsoleColor.Gray;
      _console.BackgroundColor = ConsoleColor.DarkBlue;
      _console.WriteLine($"[DBUG] {message} ");
      _console.ResetColor();
    }

    private void LogInfo(string message)
    {
      // TODO: [TESTS] (ConsoleLog) Add tests

      _console.ForegroundColor = ConsoleColor.Blue;
      _console.WriteLine(message);
      _console.ResetColor();
    }

    private void LogWarn(string message)
    {
      // TODO: [TESTS] (ConsoleLog) Add tests

      _console.ForegroundColor = ConsoleColor.Yellow;
      _console.WriteLine(message);
      _console.ResetColor();
    }

    private void LogError(string message)
    {
      // TODO: [TESTS] (ConsoleLog) Add tests

      _console.ForegroundColor = ConsoleColor.Red;
      _console.WriteLine(message);
      _console.ResetColor();
    }
  }
}
