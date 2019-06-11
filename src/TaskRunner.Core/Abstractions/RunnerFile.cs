using System.IO;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.Core.Abstractions
{
  public class RunnerFile : IFile
  {
    public bool Exists(string path)
      => File.Exists(path);

    public void WriteAllText(string path, string contents)
      => File.WriteAllText(path, contents);

    public string ReadAllText(string path)
      => File.ReadAllText(path);
  }
}
