using System.IO;

namespace TaskRunner.Core.Abstractions.Interfaces
{
  public interface IDirectory
  {
    bool Exists(string path);
    DirectoryInfo CreateDirectory(string path);
  }
}
