using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Services.Interfaces
{
  public interface ISecretsService
  {
    void Reconfigure(TaskBuilderConfig baseConfig);
    string ReplacePlaceholders(string input);
    bool HasSecret(string secretKey);
    string GetSecret(string secretKey);
  }
}
