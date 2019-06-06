using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.File
{
  public class FileCopy : IRunnerStep
  {
    // TODO: [DOCS] (FileCopy) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public FileCopy()
    {
      Name = "File.Copy";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (FileCopy) Complete this

      return true;
    }
  }
}
