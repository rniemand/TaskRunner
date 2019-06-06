using System.IO;
using TaskBuilder.Common.Abstractions.Interfaces;

namespace TaskBuilder.Common.Abstractions
{
  public class DirectoryAbstraction : IDirectory
  {
    public bool Exists(string path)
      => Directory.Exists(path);

    public DirectoryInfo CreateDirectory(string path)
      => Directory.CreateDirectory(path);
  }
}
