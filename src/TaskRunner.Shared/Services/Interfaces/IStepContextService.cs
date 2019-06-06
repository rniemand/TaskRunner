using TaskRunner.Shared.Tasks;

namespace TaskRunner.Shared.Services.Interfaces
{
  public interface IStepContextService
  {
    StepContext CreateNewContext(TaskBuilderTask task);
    void SyncCurrentStep(StepContext context, TaskBuilderStep step);
  }
}
