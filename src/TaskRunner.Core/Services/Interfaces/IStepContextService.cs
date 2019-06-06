using TaskRunner.Core.Tasks;

namespace TaskRunner.Core.Services.Interfaces
{
  public interface IStepContextService
  {
    StepContext CreateNewContext(TaskBuilderTask task);
    void SyncCurrentStep(StepContext context, TaskBuilderStep step);
  }
}
