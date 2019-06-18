using System;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Logging;

namespace TaskRunner.App.Builders
{
  public class LinuxPathBuilder : IPathBuilder
  {
    private readonly IAppLogger _logger;

    public LinuxPathBuilder(
      IAppLogger logger,
      IAppDomain appDomain)
    {
      _logger = logger;

      var appDomainBaseDirectory = appDomain.BaseDirectory;
    }

    public string Build(string path)
    {
      throw new NotImplementedException();
    }

    public string GetDirectoryName(string path)
    {
      throw new NotImplementedException();
    }
  }
}
