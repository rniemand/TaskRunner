using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Shared.Configuration;
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
    private readonly ITasksService _tasksService;


    public TaskRunnerService(
      IAppLogger logger,
      ISecretsService secretsService,
      IEnumerable<IStep> steps,
      IEnumerable<IValidator> stepValidators,
      IEnumerable<IProvider> providers,
      ITasksService tasksService)
    {
      _logger = logger;
      _secretsService = secretsService;
      _tasksService = tasksService;

      _steps = steps.ToList();
      _validators = stepValidators.Cast<BaseValidator>().ToList();
      _providers = providers.Cast<BaseProvider>().ToList();

      // Log all loaded steps for troubleshooting
      _logger.Debug("Loaded {count} step(s): {stepList}",
        _steps.Count,
        string.Join(", ", _steps.Select(x => x.Name).ToList()));
    }


    // Run() and supporting methods
    public void Run()
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      var tasks = _tasksService.GetRunnableTasks();

      if (tasks.Count == 0)
        return;

      foreach (var task in tasks)
      {
        RunTask(task);
      }
    }

    private void RunTask(TaskConfig task)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      var stepContext = CreateStepContext(task);

      // Execute each enabled task step (in the order they appear)
      foreach (var currentStep in task.Steps)
      {
        SyncStepContext(stepContext, currentStep);
        var step = ResolveStep(currentStep.Step);

        // Run the current step to decide what needs to happen next
        if (step.Execute(stepContext) == false)
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

      // Task execution succeeded
      _tasksService.TaskRanSuccessfully(task);
    }



    // StepContext related methods
    private static StepContext CreateStepContext(TaskConfig task)
    {
      return new StepContext(task.Name)
      {
        StepNameLookup = GenerateStepLookup(task)
      };
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

    private List<ValidatorAndInputs> GenerateStepValidators(StepConfig currentStep)
    {
      var validators = new List<ValidatorAndInputs>();

      if (currentStep.Validators.Count == 0)
        return validators;

      _logger.Debug("Resolving validators for current step");
      var stepValidators = new List<ValidatorAndInputs>();

      // We only want enabled validators
      foreach (var validatorConfig in currentStep.Validators.Where(x => x.Enabled).ToList())
      {
        // Safe to assign here - we confirmed that the requested validator exists when we verified the task
        stepValidators.Add(new ValidatorAndInputs
        {
          Validator = ResolveValidator(validatorConfig.Validator),
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



    // Resolver methods
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

    private BaseValidator ResolveValidator(string validatorName)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      return _validators.FirstOrDefault(x => x.Name == validatorName);
    }



    // Misc. methods
    private void LogPublishedData(StepContext stepContext)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests
      // TODO: [REFACTOR] (TaskRunnerService) Make this logging configurable

      // Check to see if the step published any data
      if (stepContext.DataPublished == false)
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
  }
}
