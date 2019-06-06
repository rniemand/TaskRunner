using System.IO;
using TaskBuilder.Common.Abstractions.Interfaces;

namespace TaskBuilder.Common.Abstractions
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
