using System.IO;
using TaskRunner.Shared.Abstractions.Interfaces;

namespace TaskRunner.Shared.Abstractions
{
  public class FileAbstraction : IFile
  {
    public bool Exists(string path)
      => File.Exists(path);

    public void WriteAllText(string path, string contents)
      => File.WriteAllText(path, contents);

    public string ReadAllText(string path)
      => File.ReadAllText(path);
  }
}
