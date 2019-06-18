using System.IO;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.App.Abstractions
{
  public class RunnerFileInfo : IFileInfo
  {
    public string FullName => _fileInfo.FullName;

    private readonly FileInfo _fileInfo;

    public RunnerFileInfo(string path)
    {
      _fileInfo = new FileInfo(path);
    }
  }
}
