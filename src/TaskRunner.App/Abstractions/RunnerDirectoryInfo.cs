using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.App.Abstractions
{
  public class RunnerDirectoryInfo : IDirectoryInfo
  {
    private readonly DirectoryInfo _directoryInfo;

    public RunnerDirectoryInfo(string path)
    {
      _directoryInfo = new DirectoryInfo(path);
    }

    public List<IFileInfo> GetFiles(string searchPattern, SearchOption searchOption)
    {
      return _directoryInfo.GetFiles(searchPattern, searchOption)
        .Select(file => new RunnerFileInfo(file.FullName))
        .Cast<IFileInfo>()
        .ToList();
    }
  }
}
