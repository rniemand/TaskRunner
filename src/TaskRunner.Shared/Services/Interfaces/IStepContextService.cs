using TaskBuilder.Common.Tasks;

namespace TaskBuilder.Common.Services.Interfaces
{
  public interface IStepContextService
  {
    StepContext CreateNewContext(TaskBuilderTask task);
    void SyncCurrentStep(StepContext context, TaskBuilderStep step);
  }
}
