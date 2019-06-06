using System.IO;
using TaskRunner.Core.Abstractions.Interfaces;

namespace TaskRunner.Core.Abstractions
{
  public class DirectoryAbstraction : IDirectory
  {
    public bool Exists(string path)
      => Directory.Exists(path);

    public DirectoryInfo CreateDirectory(string path)
      => Directory.CreateDirectory(path);
  }
}
