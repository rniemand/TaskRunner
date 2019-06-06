using TaskBuilder.Common.Configuration;

namespace TaskBuilder.Common.Services.Interfaces
{
  public interface ISecretsService
  {
    void Reconfigure(TaskBuilderConfig baseConfig);
    string ReplacePlaceholders(string input);
    bool HasSecret(string secretKey);
    string GetSecret(string secretKey);
  }
}
