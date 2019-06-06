namespace TaskRunner.Core.Builders.Interfaces
{
  public interface IPathBuilder
  {
    string Build(string path);
    string GetDirectoryName(string path);
  }
}
