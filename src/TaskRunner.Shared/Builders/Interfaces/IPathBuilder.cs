namespace TaskRunner.Shared.Builders.Interfaces
{
  public interface IPathBuilder
  {
    string Build(string path);
    string GetDirectoryName(string path);
  }
}
