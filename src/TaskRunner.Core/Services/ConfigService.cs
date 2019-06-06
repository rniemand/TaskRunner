using System;
using Newtonsoft.Json;
using TaskRunner.Core.Abstractions.Interfaces;
using TaskRunner.Core.Builders.Interfaces;
using TaskRunner.Core.Configuration;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Services.Interfaces;

namespace TaskRunner.Core.Services
{
  public class ConfigService : IConfigService
  {
    private readonly IAppLogger _logger;
    private readonly IFile _file;
    private readonly IDirectory _directory;
    private readonly IJsonHelper _jsonHelper;
    private readonly ISecretsService _secretsService;
    private readonly ITasksService _tasksService;
    private readonly IPathBuilder _pathBuilder;

    public string ConfigFilePath { get; }

    private TaskRunnerConfig _baseConfig;


    public ConfigService(
      IAppLogger logger,
      IPathBuilder pathBuilder,
      IFile file,
      IJsonHelper jsonHelper,
      ISecretsService secretsService,
      ITasksService tasksService,
      IDirectory directory)
    {
      _logger = logger;
      _file = file;
      _jsonHelper = jsonHelper;
      _secretsService = secretsService;
      _tasksService = tasksService;
      _directory = directory;
      _pathBuilder = pathBuilder;

      // TODO: [TESTS] (ConfigService) Add tests
      ConfigFilePath = pathBuilder.Build("{root}\\config\\TaskBuilder.json");
      Reconfigure();
    }


    // Public methods
    public void Reconfigure()
    {
      // TODO: [TESTS] (ConfigService) Add tests
      // TODO: [OPTIMIZATION] (ConfigService) Only reload configuration if the file has changed

      // Reload our base configuration
      LoadConfigFile();

      // Reconfigure all dependent sub-services with the new configuration
      _secretsService.Reconfigure(_baseConfig);
      _tasksService.Reconfigure(_baseConfig);
    }


    // Configuration file related methods
    private void LoadConfigFile()
    {
      // TODO: [TESTS] (ConfigService) Add tests

      // Ensure the configuration file exists and load it
      EnsureConfigFileExists();
      var configJson = _file.ReadAllText(ConfigFilePath);

      _baseConfig = _jsonHelper.DeserializeObject<TaskRunnerConfig>(configJson);
      _logger.Debug("Loaded configuration file: {path}", ConfigFilePath);
    }

    private void EnsureConfigFileExists()
    {
      // TODO: [TESTS] (ConfigService) Add tests

      if (_file.Exists(ConfigFilePath))
        return;

      CreateInitialConfigFile();

      // TODO: [REVISE] (ConfigService) Throw a better exception here
      if (!_file.Exists(ConfigFilePath))
        throw new Exception("Unable to load configuration file");
    }

    private void CreateInitialConfigFile()
    {
      // TODO: [TESTS] (ConfigService) Add tests

      _logger.Info("Creating initial configuration file: {path}", ConfigFilePath);

      // Ensure that the configuration directory exists
      var baseDir = _pathBuilder.GetDirectoryName(ConfigFilePath);

      if (!_directory.Exists(baseDir))
      {
        _logger.Info("Creating base configuration directory: {dir}", baseDir);
        _directory.CreateDirectory(baseDir);
      }

      // Create the initial configuration file
      var configJson = _jsonHelper.SerializeObject(
        GenerateDefaultConfig(),
        Formatting.Indented);

      _file.WriteAllText(ConfigFilePath, configJson);
    }

    private static TaskRunnerConfig GenerateDefaultConfig()
    {
      // TODO: [TESTS] (ConfigService) Add tests
      // TODO: [COMPLETE] (ConfigService) Ensure that we are setting sensible configuration here

      return new TaskRunnerConfig
      {
        // TODO: [REVERT] (ConfigService) Revert when the program is stable enough
        //SecretsFile = "./config/secrets.json",
        SecretsFile = "\\\\10.0.0.50\\p$\\secrets.json",
        TasksFolder = "{!Core.TasksDir}"
      };
    }
  }
}
