using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Directory
{
  public class DirectoryExists : IRunnerStep
  {
    // TODO: [DOCS] (DirectoryExists) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public DirectoryExists()
    {
      Name = "Directory.Exists";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (DirectoryExists) Complete step

      return true;
    }
  }
}
