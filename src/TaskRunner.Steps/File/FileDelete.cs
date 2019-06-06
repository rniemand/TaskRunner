using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.File
{
  public class FileDelete : IRunnerStep
  {
    // TODO: [DOCS] (FileDelete) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public FileDelete()
    {
      Name = "File.Delete";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (FileDelete) Complete this step

      return true;
    }
  }
}
