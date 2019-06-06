namespace TaskRunner.Shared.Tasks.Interfaces
{
  public interface ITaskBuilderStep
  {
    string Name { get; }

    bool Execute(StepContext context);
  }
}
