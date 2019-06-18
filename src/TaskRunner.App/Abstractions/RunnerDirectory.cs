using System.IO;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.App.Abstractions
{
  public class RunnerDirectory : IDirectory
  {
    public bool Exists(string path)
      => Directory.Exists(path);

    public DirectoryInfo CreateDirectory(string path)
      => Directory.CreateDirectory(path);
  }
}
