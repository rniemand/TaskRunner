using TaskRunner.Core.Configuration;

namespace TaskRunner.Core.Services.Interfaces
{
  public interface ISecretsService
  {
    void Reconfigure(TaskBuilderConfig baseConfig);
    string ReplacePlaceholders(string input);
    bool HasSecret(string secretKey);
    string GetSecret(string secretKey);
  }
}
