using System.IO;

namespace TaskRunner.Shared.Abstractions
{
  public interface IDirectory
  {
    bool Exists(string path);
    DirectoryInfo CreateDirectory(string path);
  }
}
