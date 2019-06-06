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

      var taskData = new Dictionary<string, Dictionary<string, string>>();

      // Create a new step context based on the provided data
      var context = new StepContext
      {
        Arguments = new Dictionary<string, string>(),
        TaskData = taskData
      };

      _logger.Debug("Created initial context for task {task}", task.Name);

      return context;
    }

    public void SyncCurrentStep(StepContext context, RunnerStep step)
    {
      throw new System.NotImplementedException();
    }
  }
}
