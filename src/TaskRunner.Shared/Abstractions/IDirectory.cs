using System.IO;

namespace TaskRunner.Shared.Abstractions
{
  public interface IDirectory
  {
    bool Exists(string path);

    // TODO: [REVISE] (IDirectory) Replace with IDirectoryInfo
    DirectoryInfo CreateDirectory(string path);
    IDirectoryInfo GetDirectoryInfo(string path);
  }
}
