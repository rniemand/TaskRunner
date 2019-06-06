namespace TaskRunner.Shared.Abstractions.Interfaces
{
  public interface IFile
  {
    bool Exists(string path);
    void WriteAllText(string path, string contents);
    string ReadAllText(string path);
  }
}
