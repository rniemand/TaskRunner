using System.IO;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Extensions;
using TaskRunner.Shared.Logging;

namespace TaskRunner.App.Builders
{
  public class LinuxPathBuilder : IPathBuilder
  {
    private readonly IAppLogger _logger;
    private readonly string _rootDir;

    public LinuxPathBuilder(
      IAppLogger logger,
      IAppDomain appDomain)
    {
      // TODO: [TESTS] (LinuxPathBuilder) Add tests

      _logger = logger;
      _rootDir = appDomain.BaseDirectory.AppendIfMissing("/");
    }

    public string Build(string path)
    {
      // TODO: [TESTS] (LinuxPathBuilder) Add tests

      if (string.IsNullOrWhiteSpace(path))
        return path;

      var builtPath = path
        .Replace("{root}", _rootDir)
        .Replace("./", _rootDir)
        .Replace("\\", "/")
        .Replace("//", "/");

      // Log whenever we build a new path for troubleshooting
      if (path != builtPath)
      {
        _logger.Verbose("Changed '{path}' to '{newPath}'", path, builtPath);
      }

      return builtPath;
    }

    public string GetDirectoryName(string path)
    {
      // TODO: [TESTS] (LinuxPathBuilder) Add tests

      return Path.GetDirectoryName(path);
    }
  }
}
