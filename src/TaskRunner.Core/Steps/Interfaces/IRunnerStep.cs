namespace TaskRunner.Core.Steps.Interfaces
{
  public interface IRunnerStep
  {
    string Name { get; }

    bool Execute(StepContext context);
  }
}
