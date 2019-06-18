using System.IO;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Extensions;
using TaskRunner.Shared.Logging;

namespace TaskRunner.App.Builders
{
  public class WinPathBuilder : IPathBuilder
  {
    private readonly IAppLogger _logger;
    private readonly string _rootDir;

    public WinPathBuilder(
      IAppLogger logger,
      IAppDomain appDomain)
    {
      _logger = logger;
      // TODO: [TESTS] (WinPathBuilder) Add tests

      _rootDir = appDomain.BaseDirectory.AppendIfMissing("\\");
      _logger.Debug("Base application directory is: {path}", _rootDir);
    }

    public string Build(string path)
    {
      // TODO: [TESTS] (WinPathBuilder) Add tests
      // TODO: [DOCS] (WinPathBuilder) Document placeholders
      // TODO: [OPTIMIZE] (WinPathBuilder) When on Windows replace "/" with "\"

      if (string.IsNullOrWhiteSpace(path))
        return string.Empty;

      var builtPath = path
        .Replace("{root}", _rootDir)
        .Replace("./", $"{_rootDir}\\")
        .Replace("/", "\\");

      // TODO: [REVISE] (WinPathBuilder) Replace with RegEx
      if (builtPath.StartsWith("\\\\"))
      {
        builtPath = "\\\\" + builtPath.Substring(2).Replace("\\\\", "\\");
      }
      else
      {
        builtPath = builtPath.Replace("\\\\", "\\");
      }

      // Log whenever we build a new path for troubleshooting
      if (path != builtPath)
      {
        _logger.Verbose("Changed '{path}' to '{newPath}'", path, builtPath);
      }

      return builtPath;
    }

    public string GetDirectoryName(string path)
    {
      // TODO: [TESTS] (WinPathBuilder) Add tests

      return Path.GetDirectoryName(path);
    }
  }
}
