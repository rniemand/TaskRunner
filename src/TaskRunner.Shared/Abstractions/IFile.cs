namespace TaskRunner.Shared.Abstractions
{
  public interface IFile
  {
    bool Exists(string path);
    void WriteAllText(string path, string contents);
    string ReadAllText(string path);
    void Copy(string sourceFileName, string destFileName);
  }
}
