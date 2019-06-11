using System;
using System.Collections.Generic;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Providers;
using TaskRunner.Shared.Steps;

namespace TaskRunner.Providers.Core
{
  // TODO: [DOCS] (DateProvider) Document this
  // TODO: [DOCS] (DateProvider) Document input: "Format" (choose and document sensible default value)
  // https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings

  public class UtcDateProvider : BaseProvider
  {
    public UtcDateProvider(IAppLogger logger)
      : base(logger, "UtcDate")
    {
      RegisterInput("Format", false, "yyyy-mm-dd hh:mm:ss tt");
      RegisterInput("ProvideAs", false, "Date");
    }

    public override void Run(StepContext context, string provideAs, Dictionary<string, string> inputs)
    {
      var format = GetInput(inputs, "Format");
      context.Provide(provideAs, DateTime.UtcNow.ToString(format));
    }
  }
}
