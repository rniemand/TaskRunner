using TaskRunner.Core.Configuration;
using TaskRunner.Core.Tasks;

namespace TaskRunner.Core.Services.Interfaces
{
  public interface IStepContextService
  {
    StepContext CreateNewContext(RunnerTask task);
    void SyncCurrentStep(StepContext context, RunnerStep step);
  }
}
