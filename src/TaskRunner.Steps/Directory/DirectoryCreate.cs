using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Directory
{
  public class DirectoryCreate : IRunnerStep
  {
    // TODO: [DOCS] (DirectoryCreate) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public DirectoryCreate()
    {
      Name = "Directory.Create";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (DirectoryCreate) Complete step

      return true;
    }
  }
}
