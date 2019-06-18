using System;
using System.Runtime.InteropServices;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Enums;
using TaskRunner.Shared.Logging;

namespace TaskRunner.App.Abstractions
{
  public class RunnerEnvironment : IEnvironment
  {
    public HostEnvironment HostEnvironment { get; private set; }

    private readonly IAppLogger _logger;

    public RunnerEnvironment(IAppLogger logger)
    {
      // TODO: [TESTS] (RunnerEnvironment) Add tests

      _logger = logger;

      SetHostEnvironment();
    }

    // Internal methods
    private void SetHostEnvironment()
    {
      // TODO: [TESTS] (RunnerEnvironment) Add tests

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        HostEnvironment = HostEnvironment.Windows;
      }
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        HostEnvironment = HostEnvironment.Linux;
      }
      else
      {
        // TODO: [REVISE] (RunnerEnvironment) Make use of better exception type here
        throw new Exception("Unsupported OS ... for now....");
      }

      // Log the detected host environment
      _logger.Debug("Setting HostEnvironment to: {env}", HostEnvironment.ToString("G"));
    }
  }
}
