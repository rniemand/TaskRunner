using System.IO;
using TaskRunner.Shared.Interfaces.Abstractions;

namespace TaskRunner.Core.Abstractions
{
  public class RunnerDirectory : IDirectory
  {
    public bool Exists(string path)
      => Directory.Exists(path);

    public DirectoryInfo CreateDirectory(string path)
      => Directory.CreateDirectory(path);
  }
}
