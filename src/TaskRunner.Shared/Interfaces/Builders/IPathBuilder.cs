namespace TaskRunner.Shared.Interfaces.Builders
{
  public interface IPathBuilder
  {
    string Build(string path);
    string GetDirectoryName(string path);
  }
}
