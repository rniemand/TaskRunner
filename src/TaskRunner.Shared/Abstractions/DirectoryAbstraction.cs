using System.IO;
using TaskRunner.Shared.Abstractions.Interfaces;

namespace TaskRunner.Shared.Abstractions
{
  public class DirectoryAbstraction : IDirectory
  {
    public bool Exists(string path)
      => Directory.Exists(path);

    public DirectoryInfo CreateDirectory(string path)
      => Directory.CreateDirectory(path);
  }
}
