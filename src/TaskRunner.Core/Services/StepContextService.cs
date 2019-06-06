using System.Collections.Generic;
using TaskRunner.Core.Configuration;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Services.Interfaces;
using TaskRunner.Core.Steps;

namespace TaskRunner.Core.Services
{
  public class StepContextService : IStepContextService
  {
    private readonly IAppLogger _logger;
    private readonly ISecretsService _secretsService;

    public StepContextService(
      IAppLogger logger,
      ISecretsService secretsService)
    {
      _logger = logger;
      _secretsService = secretsService;
    }

    public StepContext CreateNewContext(RunnerTask task)
    {
      // TODO: [TESTS] (StepContextService) Add tests

      // Create a new step context based on the provided data
      var context = new StepContext();

      _logger.Debug("Created initial context for task {task}", task.Name);

      return context;
    }

    public void SyncStep(StepContext context, RunnerStep step)
    {
      // TODO: [TESTS] (StepContextService) Add tests

      // Ensure that the StepId and StepName are correct
      context.StepId = step.StepId;
      context.StepName = step.StepName;

      // Generate and update the current steps arguments
      // NOTE: this call also resets the "DataPublished" flag
      context.SetArguments(GenerateStepArguments(context, step));
    }


    // Internal methods
    private Dictionary<string, string> GenerateStepArguments(StepContext context, RunnerStep step)
    {
      // TODO: [TESTS] (StepContextService) Add tests
      // TODO: [LOGGING] (StepContextService) Add logging

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
  }
}
