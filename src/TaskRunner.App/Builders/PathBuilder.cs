using System.IO;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Extensions;

namespace TaskRunner.App.Builders
{
  public class PathBuilder : IPathBuilder
  {
    private readonly string _rootDir;

    public PathBuilder(IAppDomain appDomain)
    {
      // TODO: [TESTS] (PathBuilder) Add tests

      _rootDir = appDomain.BaseDirectory.StripTrailingCharacter();
    }

    public string Build(string path)
    {
      // TODO: [TESTS] (PathBuilder) Add tests
      // TODO: [DOCS] (PathBuilder) Document placeholders
      // TODO: [OPTIMIZE] (PathBuilder) When on Windows replace "/" with "\"

      if (string.IsNullOrWhiteSpace(path))
        return string.Empty;

      return path
        .Replace("{root}", _rootDir)
        .Replace("./", $"{_rootDir}\\");
    }

    public string GetDirectoryName(string path)
    {
      return Path.GetDirectoryName(path);
    }
  }
}
