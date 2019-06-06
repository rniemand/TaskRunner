using System;
using TaskRunner.Core.Abstractions.Interfaces;
using TaskRunner.Core.Builders.Interfaces;
using TaskRunner.Core.Configuration;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Services.Interfaces;

namespace TaskRunner.Core.Services
{
  public class TasksService : ITasksService
  {
    private readonly IAppLogger _logger;
    private readonly ISecretsService _secretsService;
    private readonly IPathBuilder _pathBuilder;
    private readonly IDirectory _directory;


    public TasksService(
      IAppLogger logger,
      ISecretsService secretsService,
      IPathBuilder pathBuilder,
      IDirectory directory)
    {
      _logger = logger;
      _secretsService = secretsService;
      _pathBuilder = pathBuilder;
      _directory = directory;
    }


    // Public methods
    public void Reconfigure(TaskRunnerConfig baseConfig)
    {
      // TODO: [TESTS] (TasksService) Add tests

      LoadTaskFiles(baseConfig);
    }


    // Configuration file related methods
    private void LoadTaskFiles(TaskRunnerConfig baseConfig)
    {
      // TODO: [TESTS] (ConfigService) Add tests
      // TODO: [REVISE] (ConfigService) Move into a TasksService

      // Generate the task folder path and ensure that it exists
      var rawTasksDir = _secretsService.ReplacePlaceholders(baseConfig.TasksFolder);
      var tasksDir = _pathBuilder.Build(rawTasksDir);

      EnsureTaskDirExists(tasksDir);

      // Load defined tasks into memory

      _logger.Info("Time to load seeded tasks");

      // TODO: [CONFIG] (TasksService) Ensure that StepId is populated on load
    }

    private void EnsureTaskDirExists(string tasksDir)
    {
      // TODO: [REVISE] (ConfigService) Use a better exception here
      // TODO: [LOGGING] (ConfigService) Add logging

      // We need a valid path to work with
      if (string.IsNullOrWhiteSpace(tasksDir))
        throw new Exception("Unable to determine the tasks directory");

      // If the directory exists, we are done
      if (_directory.Exists(tasksDir))
        return;

      // Time to create the missing directory and seed some sample tasks
      _logger.Info("Creating tasks directory: {path}", tasksDir);
      _directory.CreateDirectory(tasksDir);

      // TODO: [EXCEPTION] (TasksService) Use a better exception type here
      if (!_directory.Exists(tasksDir))
        throw new Exception($"Unable to create configured tasks folder: {tasksDir}");

      SeedSampleTasks();
    }

    private void SeedSampleTasks()
    {
      // TODO: [TESTS] (ConfigService) Add tests
      // TODO: [NEXT] (ConfigService) Define and seed a sample task file

      _logger.Info("Time to seed some sample tasks");
    }
  }
}
