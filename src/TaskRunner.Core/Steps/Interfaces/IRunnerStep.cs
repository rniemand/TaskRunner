namespace TaskRunner.Core.Steps.Interfaces
{
  // TODO: [CURRENT] (IRunnerStep) Rename this - I don't like it
  public interface IRunnerStep
  {
    string Name { get; }

    bool Execute(StepContext context);
  }
}
