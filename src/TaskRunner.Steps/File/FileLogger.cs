using TaskRunner.Core.Tasks;
using TaskRunner.Core.Tasks.Interfaces;

namespace TaskRunner.Steps.File
{
  public class FileLogger : ITaskBuilderStep
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
