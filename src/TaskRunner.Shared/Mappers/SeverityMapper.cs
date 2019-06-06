using TaskBuilder.Common.Extensions;
using TaskBuilder.Common.Tasks.Enums;

namespace TaskBuilder.Common.Mappers
{
  public class SeverityMapper
  {
    public static Severity MapSeverity(string input, Severity fallback = Severity.Debug)
    {
      // TODO: [TESTS] (SeverityMapper) Add tests

      if (string.IsNullOrWhiteSpace(input))
        return fallback;

      switch (input.TrimAndLower())
      {
        case "v":
        case "ver":
        case "verbose":
          return Severity.Verbose;

        case "d":
        case "dbg":
        case "debug":
          return Severity.Debug;

        case "i":
        case "info":
        case "informational":
          return Severity.Info;

        case "w":
        case "warn":
        case "warning":
          return Severity.Warn;

        case "e":
        case "err":
        case "error":
          return Severity.Error;

        default:
          return fallback;
      }
    }
  }
}
