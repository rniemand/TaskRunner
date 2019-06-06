using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TaskRunner.Core.Abstractions.Interfaces;
using TaskRunner.Core.Builders.Interfaces;
using TaskRunner.Core.Configuration;
using TaskRunner.Core.Extensions;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Services.Interfaces;

namespace TaskRunner.Core.Services
{
  public class SecretsService : ISecretsService
  {
    private readonly IAppLogger _logger;
    private readonly IPathBuilder _pathBuilder;
    private readonly IJsonHelper _jsonHelper;
    private readonly IFile _file;

    private Dictionary<string, Dictionary<string, string>> _secrets;
    private const string SecretsRxp = @"({!([^}]+)})";


    public SecretsService(
      IAppLogger logger,
      IPathBuilder pathBuilder,
      IJsonHelper jsonHelper,
      IFile file)
    {
      _logger = logger;
      _pathBuilder = pathBuilder;
      _jsonHelper = jsonHelper;
      _file = file;

      _secrets = new Dictionary<string, Dictionary<string, string>>();
      _logger.Debug("New instance created");
    }


    // Public methods
    public void Reconfigure(TaskRunnerConfig baseConfig)
    {
      // TODO: [TESTS] (SecretsService) Add tests

      LoadSecretsFile(baseConfig);
    }

    public string ReplaceTags(string input)
    {
      // TODO: [TESTS] (SecretsService) Add tests

      if (!input.MatchesRxPattern(SecretsRxp))
        return input;

      // Replace all discovered secrets in the given input
      var matches = input.GetRxMatches(SecretsRxp);

      foreach (Match match in matches)
      {
        var placeHolder = match.Groups[1].Value;
        var secretValue = GetSecret(match.Groups[2].Value);

        input = input.Replace(placeHolder, secretValue);
      }

      // TODO: [RECURSE] (TaskRunnerConfig) Add recursion
      // TODO: [OPTIMIZATION] (TaskRunnerConfig) Add some form of calculated values caching

      return input;
    }

    public bool HasSecret(string secretKey)
    {
      // TODO: [TESTS] (SecretsService) Add tests

      if (string.IsNullOrWhiteSpace(secretKey))
        return false;

      var parts = secretKey.Split('.');

      // TODO: [EXCEPTION] (SecretsService) Use a better Exception type here
      if (parts.Length <= 0 || parts.Length > 2)
        throw new Exception($"Invalid secret key '{secretKey}'");

      var section = parts[0];
      var key = parts[1];

      // Ensure that the requested secret exists
      return HasSectionKey(section, key);
    }

    public string GetSecret(string secretKey)
    {
      // TODO: [TESTS] (SecretsService) Add tests

      // TODO: [LOGGING] (SecretsService) Add some logging
      if (!HasSecret(secretKey))
        return string.Empty;

      // Safe to split here as "HasSecret()" validates the structure
      var parts = secretKey.Split('.');

      return _secrets[parts[0]][parts[1]];
    }


    // Configuration file related methods
    private void LoadSecretsFile(TaskRunnerConfig config)
    {
      // TODO: [TESTS] (ConfigService) Add tests

      // Generate the path to the secrets file and ensure it exists
      var rawFilePath = config.SecretsFile;
      var secretsFilePath = _pathBuilder.Build(rawFilePath);

      EnsureFileExists(secretsFilePath);

      // Attempt to load the secrets file into memory
      var secretsJson = _file.ReadAllText(secretsFilePath);
      _secrets = _jsonHelper.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(secretsJson);

      // Log some useful debug information for troubleshooting
      LogLoadedSecretsInfo(secretsFilePath);
    }

    private void EnsureFileExists(string secretsFilePath)
    {
      // TODO: [TESTS] (SecretsService) Add tests

      // Ensure we always have a secrets file to work with
      if (_file.Exists(secretsFilePath))
        return;

      CreateSecretsFile(secretsFilePath);

      // TODO: [EXCEPTIONS] (ConfigService) Add better exception type here
      if (!_file.Exists(secretsFilePath))
        throw new Exception($"Unable to load secrets file: {secretsFilePath}");

      _logger.Debug("Found secrets file {path}, attempting to load values", secretsFilePath);
    }

    private void CreateSecretsFile(string filePath)
    {
      // TODO: [TESTS] (ConfigService) Add tests

      _logger.Info("Creating initial secrets file: {path}", filePath);

      var secretsJson = _jsonHelper.SerializeObject(
        GenerateDefaultSecrets(),
        Formatting.Indented);

      _file.WriteAllText(filePath, secretsJson);
    }

    private static Dictionary<string, Dictionary<string, string>> GenerateDefaultSecrets()
    {
      // TODO: [TESTS] (SecretsService) Add tests

      return new Dictionary<string, Dictionary<string, string>>
      {
        ["Hints"] = new Dictionary<string, string>
        {
          ["Hint_01"] = "You can place your secrets in their own <Section>",
          ["Hint_02"] = "A <Section> can have any amount of <Key>s associated with it",
          ["Hint_03"] = "A <Key> is a string representation of the value you want to store",
          ["Hint_04"] = "<Key>s are cast to the correct type by their consumers",
          ["Hint_05"] = "Use this 'Hints' Section as a template for your own settings",
          ["Hint_06"] = "You need to follow the '<Section>.<Key> = <Value>' pattern",
          ["Hint_07"] = "You can remove the 'Hints' Section whenever you want",
          ["Hint_08"] = "Have fun!",
          ["Hint_09"] = "Contribution is always welcome: https://github.com/rniemand/TaskBuilder"
        },
        ["Core"] = new Dictionary<string, string>
        {
          ["TasksDir"] = "./config/tasks/"
        }
      };
    }

    private void LogLoadedSecretsInfo(string filePath)
    {
      // TODO: [TESTS] (SecretsService) Add tests

      var sb = new StringBuilder()
        .Append($"Loaded secrets file: {filePath}")
        .Append(Environment.NewLine);

      foreach (var key in _secrets.Keys)
      {
        sb.Append($"    Loaded '{key}' section with {_secrets[key].Count} entries");
        sb.Append(Environment.NewLine);
      }

      _logger.Debug(sb.ToString().Trim());
    }


    // Resolving secrets methods
    private bool HasSection(string section)
    {
      // TODO: [TESTS] (SecretsService) Add tests

      if (string.IsNullOrWhiteSpace(section))
        return false;

      return _secrets.ContainsKey(section);
    }

    private bool HasSectionKey(string section, string key)
    {
      // TODO: [TESTS] (SecretsService) Add tests

      if (!HasSection(section))
        return false;

      if (string.IsNullOrWhiteSpace(key))
        return false;

      return _secrets[section].ContainsKey(key);
    }
  }
}
