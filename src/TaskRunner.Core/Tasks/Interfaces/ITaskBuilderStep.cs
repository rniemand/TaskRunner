namespace TaskRunner.Core.Tasks.Interfaces
{
  public interface ITaskBuilderStep
  {
    string Name { get; }

    bool Execute(StepContext context);
  }
}
