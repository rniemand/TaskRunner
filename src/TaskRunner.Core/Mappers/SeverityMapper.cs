﻿using TaskRunner.Core.Enums;
using TaskRunner.Core.Extensions;

namespace TaskRunner.Core.Mappers
{
  public class SeverityMapper
  {
    public static RunnerSeverity MapSeverity(string input, RunnerSeverity fallback = RunnerSeverity.Debug)
    {
      // TODO: [TESTS] (SeverityMapper) Add tests

      if (string.IsNullOrWhiteSpace(input))
        return fallback;

      switch (input.TrimAndLower())
      {
        case "v":
        case "ver":
        case "verbose":
          return RunnerSeverity.Verbose;

        case "d":
        case "dbg":
        case "debug":
          return RunnerSeverity.Debug;

        case "i":
        case "info":
        case "informational":
          return RunnerSeverity.Info;

        case "w":
        case "warn":
        case "warning":
          return RunnerSeverity.Warn;

        case "e":
        case "err":
        case "error":
          return RunnerSeverity.Error;

        default:
          return fallback;
      }
    }
  }
}
