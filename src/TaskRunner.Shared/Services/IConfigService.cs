namespace TaskRunner.Shared.Services
{
  public interface IConfigService
  {
    string ConfigFilePath { get; }
    void Reconfigure();
  }
}
