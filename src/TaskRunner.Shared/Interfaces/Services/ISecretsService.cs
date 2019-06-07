using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Interfaces.Services
{
  public interface ISecretsService
  {
    void Reconfigure(TaskRunnerConfig baseConfig);
    string ReplaceTags(string input);
  }
}
