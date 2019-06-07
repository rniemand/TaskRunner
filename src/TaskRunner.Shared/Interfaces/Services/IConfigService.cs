namespace TaskRunner.Shared.Interfaces.Services
{
  public interface IConfigService
  {
    string ConfigFilePath { get; }
    void Reconfigure();
  }
}
