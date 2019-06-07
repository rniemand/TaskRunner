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
        stepContext.RegisterStepValidators(new List<IStepSuccessValidator>
        {
          new PocSuccessValidator
          {
            Arguments = new Dictionary<string, string>
            {
              {"Property", "response.content"},
              {"Contains", "has not changed"}
            }
          }
        });

        if (!runnerStep.Execute(stepContext))
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


    // Task and Step validation methods
    private bool ValidateTask(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

      // Ensure that the task has some steps defined
      if (task.Steps == null || task.Steps.Length == 0)
      {
        _logger.Error("Task '{task}' has no steps defined, disabling", task.Name);
        task.Enabled = false;
        return false;
      }

      // Ensure that there is at least one enabled step
      if (!task.Steps.Any(x => x.Enabled))
      {
        _logger.Error("Task '{task}' has no enabled steps, disabling", task.Name);
        task.Enabled = false;
        return false;
      }

      // Ensure each step has an associated stepId (do not move this method!)
      AssignStepIds(task);


      // Ensure that all task steps have been registered through DI
      if (!EnsureTaskStepRunnersExist(task))
      {
        task.Enabled = false;
        return false;
      }

      // Ensure each step has all required inputs set
      if (ValidateRequiredInputsPresent(task) == false)
      {
        task.Enabled = false;
        return false;
      }

      // Ensure all steps have valid names
      AssignStepNames(task);
      NormalizeStepNames(task);
      EnsureAllStepNamesAreUnique(task);

      // TODO: [COMPLETE] (TaskRunnerService) Ensure that any changes are persisted to the original task file

      // It looks like we are good to go with this task
      return true;
    }

    private static void AssignStepIds(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      // We assign all steps an ID (even disabled) as the ID is used for indexing

      var stepId = 0;
      foreach (var step in task.Steps)
      {
        step.StepId = stepId++;
      }
    }

    private void AssignStepNames(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

      foreach (var step in task.Steps)
      {
        // Step has a name - we can skip it
        if (string.IsNullOrWhiteSpace(step.StepName) == false)
          continue;

        // We are missing a StepName - let's fix that
        step.StepName = $"{step.Step}_{step.StepId}".CleanStepName();

        _logger.Warn("Step {id} on Task '{task}' has no StepName - setting it to {name}",
          step.StepId, task.Name, step.StepName);
      }
    }

    private void NormalizeStepNames(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

      foreach (var step in task.Steps)
      {
        // Generate the IDEAL step name
        var cleanStepName = step.StepName.CleanStepName();

        // If the current step name == IDEAL NAME we are good
        if (cleanStepName == step.StepName)
          continue;

        // Bad step name defined, let's fix that
        _logger.Warn(
          "The assigned step name '{name}' for stepId {id} on task '{task}' does " +
          "not meet the naming convention - setting step name to '{newName}'",
          step.StepName, step.StepId, task.Name, cleanStepName);

        step.StepName = cleanStepName;
      }
    }

    private void EnsureAllStepNamesAreUnique(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

      var duplicateStepNames = task.Steps
        .Select(x => x.StepName).GroupBy(x => x).Where(x => x.Count() > 1)
        .Select(x => x.Key)
        .ToList();

      // No duplicates - we are good
      if (duplicateStepNames.Count == 0)
        return;

      // Got duplicate names - let's fix them
      foreach (var duplicateName in duplicateStepNames)
      {
        var steps = task.Steps.Where(x => x.StepName == duplicateName).ToList();
        var counter = 1;

        foreach (var step in steps)
        {
          // Generate an unique name for each duplicate step
          step.StepName = $"{step.StepName}_{counter++}".CleanStepName();

          _logger.Warn(
            "StepName '{name}' (stepId: {id}) is not unique and is shared with {count} other " +
            "step(s) - renaming this step to '{newName}' to avoid collisions.",
            duplicateName, step.StepId, steps.Count, step.StepName);
        }
      }
    }

    private bool ValidateRequiredInputsPresent(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

      // Get all enabled steps to check
      var enabledSteps = task.Steps.Where(x => x.Enabled).ToList();

      foreach (var currentStep in enabledSteps)
      {
        var step = ResolveStep(currentStep.Step);
        var inputs = task.Steps[currentStep.StepId].Inputs;

        if (step.RequiredInputsSet(inputs, currentStep.StepName, task.Name))
          continue;

        // Step argument validation failed, we won't be able to run the given task
        _logger.Error("Task '{task}' step '{step}' is missing required inputs, disabling",
          task.Name, currentStep.StepName);

        return false;
      }

      // All task steps have passed argument validation
      return true;
    }

    private bool EnsureTaskStepRunnersExist(RunnerTask task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

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


    // Step execution methods
    private TaskStepBase ResolveStep(string stepName)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      // TODO: [EXCEPTIONS] (TaskRunnerService) Throw a more specific exception here
      if (string.IsNullOrWhiteSpace(stepName))
        throw new Exception("Provided 'step' is blank!");

      // TODO: [REFACTOR] (TaskRunnerService) ToLower() all step names when registering - drop the IgnoreCase below
      var builderStep = _steps
        .FirstOrDefault(x => x.Name.Equals(stepName, StringComparison.InvariantCultureIgnoreCase));

      // TODO: [VALIDATION] (TaskRunnerService) Ensure that we have a match
      // TODO: [LOGGING] (TaskRunnerService) Log if we are missing the requested step

      return (TaskStepBase)builderStep;
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

      foreach (var (key, value) in step.Inputs)
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
