using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Extensions;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Providers;
using TaskRunner.Shared.Services;
using TaskRunner.Shared.Steps;
using TaskRunner.Shared.Validators;

namespace TaskRunner.App.Services
{
  public class TaskRunnerService : ITaskRunnerService
  {
    private readonly IAppLogger _logger;

    private readonly List<IStep> _steps;
    private readonly List<BaseValidator> _validators;
    private readonly List<BaseProvider> _providers;
    private readonly ISecretsService _secretsService;


    public TaskRunnerService(
      IAppLogger logger,
      ISecretsService secretsService,
      IEnumerable<IStep> steps,
      IEnumerable<IValidator> stepValidators,
      IEnumerable<IProvider> providers)
    {
      _logger = logger;
      _secretsService = secretsService;

      _steps = steps.ToList();
      _validators = stepValidators.Cast<BaseValidator>().ToList();
      _providers = providers.Cast<BaseProvider>().ToList();

      // Log all loaded steps for troubleshooting
      _logger.Debug("Loaded {count} step(s): {stepList}",
        _steps.Count,
        string.Join(", ", _steps.Select(x => x.Name).ToList()));
    }


    // Public methods
    public void RunTask(TaskConfig task)
    {
      // TODO: [REMOVE] (ITaskRunnerService) Remove this once initial dev testing has been completed

      // TODO: [OPTIMIZATION] (TaskRunnerService) Find a better way to run this - i.e. cache validated tasks on load
      // Ensure that this task meets requirements to run
      if (ValidateTask(task) == false)
      {
        _logger.Error("Task validation failed for {task}, skipping", task.Name);
        return;
      }

      // Generate the initial step context and execute steps one by one
      var stepContext = new StepContext
      {
        TaskName = task.Name,
        StepNameLookup = GenerateStepLookup(task)
      };

      // Execute each enabled task step (in the order they appear)
      foreach (var currentStep in task.Steps)
      {
        var step = ResolveStep(currentStep.Step);
        SyncStepContext(stepContext, currentStep);

        // Run the current step to decide what needs to happen next
        if (!step.Execute(stepContext))
        {
          // TODO: [COMPLETE] (TaskRunnerService) Handle step execution failed (based on configuration)

          // For now - bail out
          _logger.Error("Task {task} Step {stepName} execution failed",
            task.Name, currentStep.Name);

          return;
        }

        LogPublishedData(stepContext);

        // TODO: [COMPLETE] (TaskRunnerService) Complete step execution success
        _logger.Debug("Step {name} execution succeeded", currentStep.Name);
      }

      // Task execution has completed
      // TODO: [COMPLETE] (TaskRunnerService) Handle task execution completed
    }

    public void Run()
    {

    }


    // Task and Step validation methods
    private bool ValidateTask(TaskConfig task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

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

      // Ensure all task validators are registered
      if (!EnsureTaskValidatorsAreRegistered(task))
      {
        task.Enabled = false;
        return false;
      }

      // Ensure that all step validators have their required arguments \ inputs
      // ReSharper disable once InvertIf
      if (!EnsureAllValidatorInputsAreDefined(task))
      {
        task.Enabled = false;
        return false;
      }

      // TODO: [COMPLETE] (TaskRunnerService) Validate task providers

      // TODO: [COMPLETE] (TaskRunnerService) Ensure that any changes are persisted to the original task file

      // It looks like we are good to go with this task
      return true;
    }

    private static void AssignStepIds(TaskConfig task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      // We assign all steps an ID (even disabled) as the ID is used for indexing

      var stepId = 0;
      foreach (var step in task.Steps)
      {
        step.StepId = stepId++;
      }
    }

    private void AssignStepNames(TaskConfig task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

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
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

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
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

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

    private bool ValidateRequiredInputsPresent(TaskConfig task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [DOCS] (TaskRunnerService) Document this feature

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

    private bool EnsureTaskStepRunnersExist(TaskConfig task)
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

    private bool EnsureTaskValidatorsAreRegistered(TaskConfig task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [REVISE] (TaskRunnerService) Revise this

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

    private bool EnsureAllValidatorInputsAreDefined(TaskConfig task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [REVISE] (TaskRunnerService) Clean up this code

      var steps = task.Steps
        .Where(s => s.Enabled && s.Validators.Count > 0)
        .ToList();

      foreach (var step in steps)
      {
        foreach (var validator in step.Validators)
        {
          // TODO: [IDEA] (TaskRunnerService) Try returning the missing input so we can log it

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


    // Step execution methods
    private BaseStep ResolveStep(string stepName)
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

      return (BaseStep)builderStep;
    }

    private void LogPublishedData(StepContext stepContext)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [REFACTOR] (TaskRunnerService) Make this logging configurable

      // Check to see if the step published any data
      if (!stepContext.DataPublished)
        return;

      // Count published data points and work out the longest "key" length
      var publishedData = stepContext.GetLastPublishedData();
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

    private Dictionary<string, string> GenerateStepInputs(StepContext context, StepConfig step)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [LOGGING] (TaskRunnerService) Add logging

      // NOTE: Below is the current list of value placeholders the service is aware of
      //        {!Section.Key}    => retrieves the provided sections key value from your secrets file
      //        {@Name.Key}   => retrieves the published task data value from a previous step

      var arguments = new Dictionary<string, string>();

      foreach (var (key, value) in step.Inputs)
      {
        var argWithSecrets = _secretsService.ReplaceTags(value);
        arguments[key] = context.ReplaceTags(argWithSecrets);
      }

      return arguments;
    }

    private List<ValidatorAndArguments> GenerateStepValidators(StepConfig currentStep)
    {
      var validators = new List<ValidatorAndArguments>();

      if (currentStep.Validators.Count == 0)
        return validators;

      _logger.Debug("Resolving validators for current step");
      var stepValidators = new List<ValidatorAndArguments>();

      // We only want enabled validators
      foreach (var validatorConfig in currentStep.Validators.Where(x => x.Enabled).ToList())
      {
        // Safe to assign here - we confirmed that the requested validator exists when we verified the task
        stepValidators.Add(new ValidatorAndArguments
        {
          Validator = GetStepValidator(validatorConfig.Validator),
          Config = validatorConfig
        });
      }

      return validators;
    }

    private List<ProviderAndInputs> GenerateProviders(StepConfig currentStep)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      var providers = new List<ProviderAndInputs>();

      if (currentStep.Providers.Count == 0)
        return providers;

      if (!currentStep.Providers.Any(p => p.Enabled))
        return providers;


      foreach (var providerConfig in currentStep.Providers.Where(p => p.Enabled).ToList())
      {
        var provider = _providers.FirstOrDefault(p => p.Name == providerConfig.Provider);

        if (provider == null)
          continue;

        providers.Add(new ProviderAndInputs
        {
          Provider = provider,
          Inputs = providerConfig.Inputs
        });
      }

      return providers;
    }

    private void SyncStepContext(StepContext context, StepConfig currentStep)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      context.SetCurrentStep(
        currentStep,
        GenerateStepInputs(context, currentStep),
        GenerateStepValidators(currentStep),
        GenerateProviders(currentStep));
    }

    private BaseValidator GetStepValidator(string validatorName)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      return _validators.FirstOrDefault(x => x.Name == validatorName);
    }

    private static Dictionary<int, string> GenerateStepLookup(TaskConfig task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [REVISE] (TaskRunnerService) Perhaps move this into an extension method

      var stepLookup = new Dictionary<int, string>();

      foreach (var step in task.Steps)
      {
        stepLookup[step.StepId] = step.Name;
      }

      return stepLookup;
    }
  }
}
