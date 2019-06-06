using System.IO;

namespace TaskRunner.Shared.Abstractions.Interfaces
{
  public interface IDirectory
  {
    bool Exists(string path);
    DirectoryInfo CreateDirectory(string path);
  }
}
