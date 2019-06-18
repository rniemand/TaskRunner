using System.Diagnostics;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace TaskRunner.App.Logging
{
  public class CallerEnricher : ILogEventEnricher
  {
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
      // TODO: [REVISE] (CallerEnricher) Clean up this code

      var frames = new StackTrace().GetFrames().Skip(4).ToList();
      var caller = "";

      foreach (var frame in frames)
      {
        var method = frame.GetMethod();
        var methodName = method.Name;

        if (method.ReflectedType != null)
        {
          var fullName = method.ReflectedType.FullName;

          // TODO: [REFINE] (CallerEnricher) Refine the skip list for this
          if (fullName.EndsWith(".Logging.AppLogger"))
            continue;

          caller = $"{fullName}.{methodName}";
          break;
        }
      }

      if (!string.IsNullOrWhiteSpace(caller))
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Caller", caller));
    }
  }
}
