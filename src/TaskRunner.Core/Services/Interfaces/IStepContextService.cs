using TaskRunner.Core.Configuration;
using TaskRunner.Core.Steps;

namespace TaskRunner.Core.Services.Interfaces
{
  public interface IStepContextService
  {
    StepContext CreateNewContext(RunnerTask task);
    void SyncStep(StepContext context, RunnerStep step);
  }
}
