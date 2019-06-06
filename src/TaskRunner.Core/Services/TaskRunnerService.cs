using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Core.Configuration;
using TaskRunner.Core.Extensions;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Services.Interfaces;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Core.Services
{
  public class TaskRunnerService : ITaskRunnerService
  {
    private readonly IAppLogger _logger;

    private readonly List<IRunnerStep> _steps;
    // TODO: [REMOVE] (TaskRunnerService) Remove once we have a running service - used at the moment to bootstrap the secrets service
    private readonly IConfigService _configService;
    private readonly ISecretsService _secretsService;
    private readonly IStepContextService _stepContextService;


    public TaskRunnerService(
      IAppLogger logger,
      ISecretsService secretsService,
      IConfigService configService,
      IEnumerable<IRunnerStep> steps,
      IStepContextService stepContextService)
    {
      _logger = logger;
      _secretsService = secretsService;
      _configService = configService;
      _stepContextService = stepContextService;
      _steps = steps.ToList();

      // Log all loaded steps for troubleshooting
      _logger.Debug("Loaded {count} step(s): {stepList}",
        _steps.Count,
        string.Join(", ", _steps.Select(x => x.Name).ToList()));
    }


    // Public methods
    public void RunTask(RunnerTask task)
    {
      // TODO: [REMOVE] (ITaskRunnerService) Remove this once initial dev testing has been completed

      // TODO: [OPTIMIZATION] (TaskRunnerService) Find a better way to run this - i.e. cache validated tasks on load
      // Ensure that this task meets requirements to run
      if (!ValidateTask(task))
      {
        _logger.Error("Task validation failed for {task}, skipping", task.Name);
        return;
      }

      var newContext = _stepContextService.CreateNewContext(task);

      // Create a "global" data-bus for steps to push/pull values from
      var taskGlobalData = new Dictionary<string, Dictionary<string, string>>();

      // Execute task steps one by one
      foreach (var currentStep in task.Steps)
      {
        // TODO: [CURRENT] (TaskRunnerService) Sync generated step context with "currentStep"

        var stepContext = GenerateStepContext(task, currentStep.StepId, taskGlobalData);
        var taskBuilderStep = GetRequestedStep(currentStep.Step);

        // TODO: [CHECK] (TaskRunnerService) Check for and handle no step found


        if (!taskBuilderStep.Execute(stepContext))
        {
          // TODO: [HANDLE] (TaskRunnerService) Handle step execution failed
          // TODO: [COMPLETE] (TaskRunnerService) Check for and handle the continue action

          // For now - bail out
          _logger.Error("Task {task} Step {stepName} execution failed",
            task.Name, currentStep.StepName);

          return;
        }

        // TODO: [REFACTOR] (TaskRunnerService) Make this logging configurable
        // Log out all the new information published by the step
        if (stepContext.PublishedData.ContainsKey(stepContext.StepName) && stepContext.PublishedData[stepContext.StepName].Count > 0)
        {
          var sb = new StringBuilder();
          var count = stepContext.PublishedData[stepContext.StepName].Count;
          var longestKey = stepContext.PublishedData[stepContext.StepName]
            .Select(x => x.Key)
            .Max(x => x.Length);

          sb
            .Append($"Step '{stepContext.StepName}' published {count} ")
            .Append(count == 1 ? "item " : "items ")
            .Append("to the global arguments: ")
            .Append(Environment.NewLine);

          foreach (var (key, value) in stepContext.PublishedData[stepContext.StepName])
          {
            sb.Append($"    {stepContext.StepName}.{key.PadRight(longestKey, ' ')} : ");
            sb.Append(value);
            sb.Append(Environment.NewLine);
          }

          _logger.Debug(sb.ToString().Trim());
        }


        // TODO: [COMPLETE] (TaskRunnerService) Complete step execution success
        _logger.Debug("Step {name} execution succeeded", currentStep.StepName);
      }

    }


    // Task and Step validation
    private static void AssignStepIds(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      if (task?.Steps == null || task.Steps.Length == 0)
        return;

      var stepId = 0;
      foreach (var step in task.Steps)
      {
        step.StepId = stepId++;
      }
    }

    private bool ValidateStepNames(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature
      // TODO: [REFACTOR] (TaskRunnerService) Break up into smaller methods

      // Ensure that all steps have a name associated to them
      var stepCounter = 0;
      foreach (var step in task.Steps)
      {
        stepCounter++;

        // There is a name associated with the step - make sure it meets the requirements
        if (!string.IsNullOrWhiteSpace(step.StepName))
        {
          var cleanStepName = step.StepName.CleanStepName();
          if (cleanStepName == step.StepName)
            continue;

          _logger.Warn(
            "Renaming step {number} for Task {taskName} to {newName} (originally: {originalName})",
            stepCounter, task.Name, cleanStepName, step.StepName);

          step.StepName = cleanStepName;
          continue;
        }


        // Assign a step name and log
        step.StepName = $"{step.Step}_{step.StepId}".CleanStepName();

        _logger.Warn(
          "Step {number} for Task {taskName} has no 'StepName' defined, setting value to {newName}",
          stepCounter, task.Name, step.StepName);
      }

      // Ensure that all step names are unique
      var duplicateStepNames = task.Steps
        .Select(x => x.StepName)
        .GroupBy(x => x)
        .Where(x => x.Count() > 1)
        .Select(x => x.Key)
        .ToList();

      if (duplicateStepNames.Count == 0)
        return true;

      _logger.Warn("Duplicate step name(s) used for Task {task}, duplicate step name(s): {name}",
        task.Name,
        string.Join(", ", duplicateStepNames));

      return false;
    }

    private bool ValidateTask(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      // Ensure that each task has an associated stepId
      AssignStepIds(task);

      // TODO: [VALIDATE] (TaskRunnerService) Add logic to check each step against "_steps" and disable if a step is missing
      // TODO: [VALIDATE] (TaskRunnerService) Check for and handle the "Enabled" state
      // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments (at a step by step level)

      // Ensure that each step has an unique name
      return ValidateStepNames(task);
    }


    // Step context generation
    private StepContext GenerateStepContext(
      RunnerTask task,
      int stepId,
      Dictionary<string, Dictionary<string, string>> taskData)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      // Create a new step context based on the provided data
      var context = new StepContext
      {
        StepId = stepId,
        StepName = task.Steps[stepId].StepName,
        Arguments = new Dictionary<string, string>(),
        PublishedData = taskData
      };

      // Generate step arguments
      foreach (var (key, value) in task.Steps[stepId].Arguments)
      {
        // NOTE: Below is the current list of value placeholders the service is aware of
        //        {!Section.Key}    => retrieves the provided sections key value from your secrets file
        //        {@StepName.Key}   => retrieves the published task data value from a previous step

        context.Arguments[key] = context.ReplaceTags(
          _secretsService.ReplacePlaceholders(value)
        );
      }

      return context;
    }

    private IRunnerStep GetRequestedStep(string stepName)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      // TODO: [EXCEPTIONS] (TaskRunnerService) Throw a more specific exception here
      if (string.IsNullOrWhiteSpace(stepName))
        throw new Exception("Provided 'step' is blank!");

      var builderStep = _steps.FirstOrDefault(x => x.Name.Equals(stepName, StringComparison.InvariantCultureIgnoreCase));

      // TODO: [VALIDATION] (TaskRunnerService) Ensure that we have a match
      // TODO: [LOGGING] (TaskRunnerService) Log if we are missing the requested step

      return builderStep;
    }
  }
}
