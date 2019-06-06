using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.File
{
  public class FileLogger : IRunnerStep
  {
    // TODO: [DOCS] (ConsoleLog) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public FileLogger()
    {
      Name = "File.Log";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (FileLogger) Complete me

      return true;
    }
  }
}
