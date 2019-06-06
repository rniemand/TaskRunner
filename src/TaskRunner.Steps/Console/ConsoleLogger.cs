using System;
using System.Collections.Generic;
using TaskRunner.Core.Abstractions.Interfaces;
using TaskRunner.Core.Enums;
using TaskRunner.Core.Mappers;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Console
{
  // TODO: [DOCS] (ConsoleLog) Document this step
  // TODO: [DOCS] (ConsoleLog) Document argument: Severity (Optional - default Info)
  // TODO: [DOCS] (ConsoleLog) Document argument: Message
  // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

  public class ConsoleLogger : IRunnerStep
  {
    public string Name { get; }

    private readonly IConsole _console;

    public ConsoleLogger(IConsole console)
    {
      _console = console;

      Name = "Console.Log";
    }


    // Public methods
    public bool Execute(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      // TODO: [TESTS] (ConsoleLog) Add tests

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
