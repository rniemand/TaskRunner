using System.IO;

namespace TaskBuilder.Common.Abstractions.Interfaces
{
  public interface IDirectory
  {
    bool Exists(string path);
    DirectoryInfo CreateDirectory(string path);
  }
}
