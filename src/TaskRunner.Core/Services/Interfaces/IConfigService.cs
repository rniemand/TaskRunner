namespace TaskRunner.Core.Services.Interfaces
{
  public interface IConfigService
  {
    string ConfigFilePath { get; }
    void Reconfigure();
  }
}
