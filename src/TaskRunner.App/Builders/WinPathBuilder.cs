using System.IO;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Extensions;

namespace TaskRunner.App.Builders
{
  public class WinPathBuilder : IPathBuilder
  {
    private readonly string _rootDir;

    public WinPathBuilder(IAppDomain appDomain)
    {
      // TODO: [TESTS] (WinPathBuilder) Add tests

      _rootDir = appDomain.BaseDirectory.AppendIfMissing("\\");
    }

    public string Build(string path)
    {
      // TODO: [TESTS] (WinPathBuilder) Add tests
      // TODO: [DOCS] (WinPathBuilder) Document placeholders
      // TODO: [OPTIMIZE] (WinPathBuilder) When on Windows replace "/" with "\"

      if (string.IsNullOrWhiteSpace(path))
        return string.Empty;

      path = path
        .Replace("{root}", _rootDir)
        .Replace("./", $"{_rootDir}\\")
        .Replace("/", "\\");

      // TODO: [REVISE] (WinPathBuilder) Replace with RegEx
      if (path.StartsWith("\\\\"))
      {
        return "\\\\" + path.Substring(2).Replace("\\\\", "\\");
      }

      return path.Replace("\\\\", "\\");
    }

    public string GetDirectoryName(string path)
    {
      return Path.GetDirectoryName(path);
    }
  }
}
