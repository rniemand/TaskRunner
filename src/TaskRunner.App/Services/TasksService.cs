using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Extensions;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Services;
using TaskRunner.Shared.Steps;
using TaskRunner.Shared.Validators;

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
    private readonly ISchedulerService _scheduler;
    private readonly IDateTime _dateTime;
    private readonly List<IStep> _steps;
    private readonly List<BaseValidator> _validators;
    private readonly List<TaskConfig> _tasks;


    public TasksService(
      IAppLogger logger,
      ISecretsService secretsService,
      IPathBuilder pathBuilder,
      IDirectory directory,
      IFile file,
      IJsonHelper jsonHelper,
      IEnumerable<IStep> steps,
      IEnumerable<IValidator> stepValidators,
      ISchedulerService scheduler,
      IDateTime dateTime)
    {
      _logger = logger;
      _secretsService = secretsService;
      _pathBuilder = pathBuilder;
      _directory = directory;
      _file = file;
      _jsonHelper = jsonHelper;
      _scheduler = scheduler;
      _dateTime = dateTime;
      _steps = steps.ToList();
      _validators = stepValidators.Cast<BaseValidator>().ToList();
      _tasks = new List<TaskConfig>();
    }



    // Public methods
    public void Reconfigure(TaskRunnerConfig baseConfig)
    {
      // TODO: [TESTS] (TasksService) Add tests

      LoadTasks(baseConfig);
    }

    public List<TaskConfig> GetRunnableTasks()
    {
      // TODO: [TESTS] (TasksService) Add tests

      // TODO: [COMPLETE] (TasksService) Complete this

      var currentTime = _dateTime.Now;

      return _tasks.Where(x => x.Enabled && x.NextRunTime <= currentTime).ToList();
    }

    public void TaskRanSuccessfully(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      // TODO: [COMPLETE] (TasksService) Complete me

      task.LastRunTime = _dateTime.Now;
      task.RunCount += 1;
      _scheduler.ScheduleNextRun(task);

      SaveTaskState(task);
    }



    // Configuration related methods
    private void LoadTasks(TaskRunnerConfig baseConfig)
    {
      // TODO: [TESTS] (ConfigService) Add tests
      // TODO: [REVISE] (ConfigService) Move into a TasksService
      // TODO: [FUTURE] (TasksService) Run integrity check to remove orphaned tasks

      // Generate the task folder path and ensure that it exists
      _tasks.Clear();
      var rawTasksDir = _secretsService.ReplaceTags(baseConfig.TasksFolder);
      var tasksDir = _pathBuilder.Build(rawTasksDir);

      EnsureTaskDirExists(tasksDir);

      // Search for user defined tasks
      var directoryInfo = _directory.GetDirectoryInfo(tasksDir);
      var tasks = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);

      // Check to see if we need to seed some sample tasks for the user
      if (tasks.Count == 0)
      {
        SeedSampleTasks();
        tasks = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
      }

      // Let's load and parse all discovered user defined tasks
      _logger.Info("Found {count} tasks to load", tasks.Count);

      foreach (var task in tasks)
        LoadTask(task.FullName);

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

    private void LoadTask(string taskFilePath)
    {
      // TODO: [TESTS] (TasksService) Add tests

      _logger.Verbose("Attempting to load task file: {file}", taskFilePath);


      try
      {
        // Try to load and deserialize the given task
        var stateFile = GetStateFilePath(taskFilePath);

        var json = _file.ReadAllText(stateFile);
        var task = _jsonHelper.DeserializeObject<TaskConfig>(json);
        task.TaskFilePath = stateFile;

        // To ensure we run at start-up we set the NextRunTime to null
        if (task.RunAtStartup)
          task.NextRunTime = null;

        // Run some strict validation on the task, we only want to let the good ones through :P
        if (ValidateTask(task) == false)
          return;

        // Tasks passed validation, schedule next run time and persist the new state
        _scheduler.ScheduleNextRun(task);
        SaveTaskState(task);

        _logger.Info("Loaded task '{name}' ({file}) which is scheduled to run next at '{time}'",
          task.Name,
          task.TaskFilePath,
          task.NextRunTime);

        _tasks.Add(task);
      }
      catch (Exception ex)
      {
        // TODO: [HANDLE] (TasksService) Handle this better!
        _logger.Error(ex.Message);
      }
    }

    private bool ValidateTask(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [DOCS] (TasksService) Document this feature

      // We can only run enabled tasks
      if (TaskEnabled(task) == false)
        return false;

      // Ensure that the task has a valid schedule defined
      if (HasValidScheduleDefined(task) == false)
        return false;

      // A task needs to have some steps defined to execute
      if (TaskHasSteps(task) == false)
        return false;

      // There has got to be at least 1 enabled step
      if (HasEnabledSteps(task) == false)
        return false;

      // Auto-assign step IDs for indexing purposes
      AssignStepIds(task);

      // Ensure that all unique step-runners are registered
      if (EnabledStepRunnersRegistered(task) == false)
        return false;

      // Make sure all REQUIRED step inputs are present
      if (RequiredStepInputsPresent(task) == false)
        return false;

      // Make sure each step confirms to the naming convention
      AssignMissingStepName(task);
      NormalizeStepNames(task);
      EnsureAllStepNamesAreUnique(task);

      // Make sure all requested step validators are registered
      if (EnabledValidatorsRegistered(task) == false)
        return false;

      // Ensure that all step validators have their required arguments \ inputs
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (!RequiredValidatorInputsPresent(task))
        return false;

      // TODO: [COMPLETE] (TasksService) Ensure that any changes are persisted to the original task file

      return true;
    }

    private string GetStateFilePath(string taskFilePath)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [DOC] (TasksService) Document this

      // TRF = Task Runner File (original, I know)

      var stateFilePath = taskFilePath.Replace(Path.GetExtension(taskFilePath), ".trf");

      // We will always reload the state file as the user could have made changes to it
      if (_file.Exists(stateFilePath))
      {
        _file.Delete(stateFilePath);
      }

      _logger.Info("Creating missing state file: {path}", stateFilePath);
      _file.Copy(taskFilePath, stateFilePath);

      return stateFilePath;
    }

    private void SaveTaskState(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      var taskJson = _jsonHelper.SerializeObject(task, Formatting.Indented);

      _logger.Verbose(
        "Persisting task '{task}' state to file: {file}", 
        task.Name, task.TaskFilePath);

      _file.Delete(task.TaskFilePath);
      _file.WriteAllText(task.TaskFilePath, taskJson);
    }



    // Object resolving methods
    private BaseValidator GetStepValidator(string validatorName)
    {
      // TODO: [TESTS] (TasksService) Add tests

      return _validators.FirstOrDefault(x => x.Name == validatorName);
    }

    private BaseStep ResolveStep(string stepName)
    {
      // TODO: [TESTS] (TasksService) Add tests

      // TODO: [EXCEPTIONS] (TasksService) Throw a more specific exception here
      if (string.IsNullOrWhiteSpace(stepName))
        throw new Exception("Provided 'step' is blank!");

      // TODO: [REFACTOR] (TasksService) ToLower() all step names when registering - drop the IgnoreCase below
      var builderStep = _steps
        .FirstOrDefault(x => x.Name.Equals(stepName, StringComparison.InvariantCultureIgnoreCase));

      // TODO: [VALIDATION] (TasksService) Ensure that we have a match
      // TODO: [LOGGING] (TasksService) Log if we are missing the requested step

      return (BaseStep)builderStep;
    }



    // Task validation methods
    private bool TaskEnabled(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      if (task.Enabled)
        return true;

      _logger.Info("Skipping task '{name}' ({file}) - it's disabled",
        task.Name, task.TaskFilePath);

      return false;
    }

    private bool TaskHasSteps(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      if (task.Steps.Length > 0)
        return true;

      _logger.Error("Task '{name}' ({file}) has no steps defined, skipping",
        task.Name, task.TaskFilePath);

      return false;
    }

    private bool HasEnabledSteps(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [LOGGING] (TasksService) Revise logging

      if (task.Steps.Any(x => x.Enabled))
        return true;

      _logger.Error("Task '{task}' has no enabled steps, disabling", task.Name);

      return false;
    }

    private static void AssignStepIds(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      // We assign all steps an ID (even disabled) as the ID is used for indexing

      var stepId = 0;

      foreach (var step in task.Steps)
      {
        step.StepId = stepId++;
      }
    }

    private bool EnabledStepRunnersRegistered(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      // Get a list of all unique step-runners used for this task
      var uniqueStepRunners = task.Steps
        .Where(x => x.Enabled)
        .Select(x => x.Step)
        .Distinct()
        .ToList();

      // Ensure that the requested step-runners are in the DI container
      foreach (var stepRunnerName in uniqueStepRunners)
      {
        if (_steps.Any(x => x.Name == stepRunnerName))
          continue;

        // Unable to find the requested step-runner
        _logger.Error("Unable to find requested step-runner '{name}' for task '{task}', disabling task",
          stepRunnerName, task.Name);

        return false;
      }

      // It looks like all requested step-runners exist
      return true;
    }

    private bool RequiredStepInputsPresent(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [DOCS] (TasksService) Document this feature

      // Get all enabled steps to check
      var enabledSteps = task.Steps.Where(x => x.Enabled).ToList();

      foreach (var currentStep in enabledSteps)
      {
        var step = ResolveStep(currentStep.Step);
        var inputs = task.Steps[currentStep.StepId].Inputs;

        if (step.RequiredInputsSet(inputs, currentStep.Name, task.Name))
          continue;

        // Step argument validation failed, we won't be able to run the given task
        _logger.Error("Task '{task}' step '{step}' is missing required inputs, disabling",
          task.Name, currentStep.Name);

        return false;
      }

      // All task steps have passed argument validation
      return true;
    }

    private void AssignMissingStepName(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [DOCS] (TasksService) Document this feature

      foreach (var step in task.Steps)
      {
        // Step has a name - we can skip it
        if (string.IsNullOrWhiteSpace(step.Name) == false)
          continue;

        // We are missing a Name - let's fix that
        step.Name = $"{step.Step}_{step.StepId}".CleanStepName();

        _logger.Warn("Step {id} on Task '{task}' has no Name - setting it to {name}",
          step.StepId, task.Name, step.Name);
      }
    }

    private void NormalizeStepNames(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [DOCS] (TasksService) Document this feature

      foreach (var step in task.Steps)
      {
        // Generate the IDEAL step name
        var cleanStepName = step.Name.CleanStepName();

        // If the current step name == IDEAL NAME we are good
        if (cleanStepName == step.Name)
          continue;

        // Bad step name defined, let's fix that
        _logger.Warn(
          "The assigned step name '{name}' for stepId {id} on task '{task}' does " +
          "not meet the naming convention - setting step name to '{newName}'",
          step.Name, step.StepId, task.Name, cleanStepName);

        step.Name = cleanStepName;
      }
    }

    private void EnsureAllStepNamesAreUnique(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [DOCS] (TasksService) Document this feature

      var duplicateStepNames = task.Steps
        .Select(x => x.Name).GroupBy(x => x).Where(x => x.Count() > 1)
        .Select(x => x.Key)
        .ToList();

      // No duplicates - we are good
      if (duplicateStepNames.Count == 0)
        return;

      // Got duplicate names - let's fix them
      foreach (var duplicateName in duplicateStepNames)
      {
        var steps = task.Steps.Where(x => x.Name == duplicateName).ToList();
        var counter = 1;

        foreach (var step in steps)
        {
          // Generate an unique name for each duplicate step
          step.Name = $"{step.Name}_{counter++}".CleanStepName();

          _logger.Warn(
            "Name '{name}' (stepId: {id}) is not unique and is shared with {count} other " +
            "step(s) - renaming this step to '{newName}' to avoid collisions.",
            duplicateName, step.StepId, steps.Count, step.Name);
        }
      }
    }

    private bool EnabledValidatorsRegistered(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [REVISE] (TasksService) Revise this

      var uniqueValidators = task.Steps
        .Where(step => step.Enabled && step.Validators.Count > 0)
        .SelectMany(v => v.Validators.Select(x => x.Validator).ToList())
        .Distinct()
        .ToList();

      foreach (var validator in uniqueValidators)
      {
        if (_validators.Any(x => x.Name == validator))
          continue;

        // Validator not found
        _logger.Error("Cannot find requested validator '{validator}' for task '{task}', disabling",
          validator, task.Name);

        return false;
      }

      // Looks like all requested validators have been found
      return true;
    }

    private bool RequiredValidatorInputsPresent(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests
      // TODO: [REVISE] (TasksService) Clean up this code

      var steps = task.Steps
        .Where(s => s.Enabled && s.Validators.Count > 0)
        .ToList();

      foreach (var step in steps)
      {
        foreach (var validator in step.Validators)
        {
          // TODO: [IDEA] (TasksService) Try returning the missing input so we can log it

          if (GetStepValidator(validator.Validator).HasRequiredInputs(validator))
            continue;

          // Missing required inputs
          _logger.Error("Step '{step}' in Task '{task}' is missing a required input for the Validator '{validator}'",
            step.Name, task.Name, validator.Validator);

          return false;
        }
      }

      // Looks like all required validator inputs are present :)
      return true;
    }

    private bool HasValidScheduleDefined(TaskConfig task)
    {
      // TODO: [TESTS] (TasksService) Add tests

      if (string.IsNullOrWhiteSpace(task.FrequencyArgs) == false)
        return true;

      _logger.Error("Task '{task}' ({file}) is missing a value for FrequencyArgs!",
        task.Name,
        task.TaskFilePath);

      return false;
    }
  }
}
