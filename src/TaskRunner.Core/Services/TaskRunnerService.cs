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
    private readonly ISecretsService _secretsService;


    public TaskRunnerService(
      IAppLogger logger,
      IEnumerable<IRunnerStep> steps,
      ISecretsService secretsService)
    {
      _logger = logger;
      _secretsService = secretsService;
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

      // Generate the initial step context and execute steps one by one
      var stepContext = new StepContext
      {
        TaskName = task.Name
      };

      foreach (var currentStep in task.Steps)
      {
        SyncStep(stepContext, currentStep);
        var runnerStep = ResolveStep(currentStep.Step);

        // TODO: [CHECK] (TaskRunnerService) Check for and handle no step found


        // TODO: [CURRENT] (TaskRunnerService) Discover and load all task validators dynamically
        var validators = new List<IStepSuccessValidator>
        {
          new PocSuccessValidator
          {
            Arguments = new Dictionary<string, string>
            {
              {"Property", "response.content"},
              {"Contains", "has not changed"}
            }
          }
        };

        if (!runnerStep.Execute(stepContext, validators))
        {
          // TODO: [COMPLETE] (TaskRunnerService) Handle step execution failed (based on configuration)

          // For now - bail out
          _logger.Error("Task {task} Step {stepName} execution failed",
            task.Name, currentStep.StepName);

          return;
        }

        LogPublishedData(stepContext);

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


    // Step execution related methods
    private IRunnerStep ResolveStep(string stepName)
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

    private void LogPublishedData(StepContext stepContext)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [REFACTOR] (TaskRunnerService) Make this logging configurable

      // Check to see if the step published any data
      if (!stepContext.DataPublished)
        return;

      // Count published data points and work out the longest "key" length
      var publishedData = stepContext.GetCurrentStepsPublishedData();
      var longestKey = publishedData.Select(x => x.Key).Max(x => x.Length);
      var count = publishedData.Count;

      // Generate published data message
      var sb = new StringBuilder()
        .Append($"Step '{stepContext.StepName}' published {count} ")
        .Append(count == 1 ? "item " : "items ")
        .Append("during its execution: ")
        .Append(Environment.NewLine);

      // Generate a line per published item (use "longestKey" to align logged data for easier reading)
      foreach (var (key, value) in publishedData)
      {
        sb.Append($"    {stepContext.StepName}.{key.PadRight(longestKey, ' ')} : ");
        sb.Append(value);
        sb.Append(Environment.NewLine);
      }

      // Finally log published data message
      _logger.Debug(sb.ToString().Trim());
    }

    private Dictionary<string, string> GenerateStepArguments(StepContext context, RunnerStep step)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [LOGGING] (TaskRunnerService) Add logging

      // NOTE: Below is the current list of value placeholders the service is aware of
      //        {!Section.Key}    => retrieves the provided sections key value from your secrets file
      //        {@StepName.Key}   => retrieves the published task data value from a previous step

      var arguments = new Dictionary<string, string>();

      foreach (var (key, value) in step.Arguments)
      {
        var argWithSecrets = _secretsService.ReplaceTags(value);
        arguments[key] = context.ReplaceTags(argWithSecrets);
      }

      return arguments;
    }

    private void SyncStep(StepContext context, RunnerStep step)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      // Ensure that the StepId and StepName are correct
      context.StepId = step.StepId;
      context.StepName = step.StepName;

      // Generate and update the current steps arguments
      // NOTE: this call also resets the "DataPublished" flag
      context.SetArguments(GenerateStepArguments(context, step));
    }
  }
}
