using System;
using System.IO;
using System.Linq;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Services;

namespace TaskRunner.App.Services
{
  public class TasksService : ITasksService
  {
    private readonly IAppLogger _logger;
    private readonly ISecretsService _secretsService;
    private readonly IPathBuilder _pathBuilder;
    private readonly IDirectory _directory;
    private readonly IFile _file;
    private readonly IJsonHelper _jsonHelper;


    public TasksService(
      IAppLogger logger,
      ISecretsService secretsService,
      IPathBuilder pathBuilder,
      IDirectory directory,
      IFile file,
      IJsonHelper jsonHelper)
    {
      _logger = logger;
      _secretsService = secretsService;
      _pathBuilder = pathBuilder;
      _directory = directory;
      _file = file;
      _jsonHelper = jsonHelper;
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
      var rawTasksDir = _secretsService.ReplaceTags(baseConfig.TasksFolder);
      var tasksDir = _pathBuilder.Build(rawTasksDir);

      EnsureTaskDirExists(tasksDir);

      // Check to see if we need to seed some sample tasks for the user
      if (HasTaskFiles(tasksDir) == false)
      {
        SeedSampleTasks();
      }

      // Load defined tasks into memory
      LoadTasks(tasksDir);

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

    private bool HasTaskFiles(string tasksDir)
    {
      // TODO: [TESTS] (TasksService) Add tests

      var di = _directory.GetDirectoryInfo(tasksDir);
      var tasks = di.GetFiles("*.json", SearchOption.AllDirectories);
      return tasks.Count > 0;
    }

    private void LoadTasks(string tasksDir)
    {
      // TODO: [TESTS] (TasksService) Add tests

      var di = _directory.GetDirectoryInfo(tasksDir);
      var tasks = di.GetFiles("*.json", SearchOption.AllDirectories);

      _logger.Info("Found {count} tasks to load", tasks.Count);

      foreach (var task in tasks)
      {
        LoadTask(task.FullName);
      }


    }

    private void LoadTask(string taskFilePath)
    {
      // TODO: [TESTS] (TasksService) Add tests

      _logger.Verbose("Attempting to load task file: {file}", taskFilePath);


      try
      {
        var json = _file.ReadAllText(taskFilePath);
        var task = _jsonHelper.DeserializeObject<TaskConfig>(json);
        task.TaskFilePath = taskFilePath;

        // We need to validate as much of the task at this point
        // Move some validation methods out of the TaskRunnerService

        if (ValidateTask(task) == false)
          return;


      }
      catch (Exception ex)
      {
        // TODO: [HANDLE] (TasksService) Handle this better!
        _logger.Error(ex.Message);
      }


    }


    // Task validation methods
    private bool ValidateTask(TaskConfig task)
    {
      if (EnsureTaskIsEnabled(task) == false)
        return false;

      if (EnsureTaskHasSteps(task) == false)
        return false;

      if (EnsureTaskHasAnEnabledStep(task) == false)
        return false;


      return true;
    }

    private bool EnsureTaskIsEnabled(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      if (task.Enabled)
        return true;

      _logger.Info("Skipping task '{name}' ({file}) - it's disabled",
        task.Name, task.TaskFilePath);

      return false;
    }

    private bool EnsureTaskHasSteps(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      if (task.Steps.Length > 0)
        return true;

      _logger.Error("Task '{name}' ({file}) has no steps defined, skipping",
        task.Name, task.TaskFilePath);

      return false;
    }

    private bool EnsureTaskHasAnEnabledStep(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [LOGGING] (TasksService) Revise logging

      if (task.Steps.Any(x => x.Enabled))
        return true;

      _logger.Error("Task '{task}' has no enabled steps, disabling", task.Name);

      return false;
    }
  }
}
