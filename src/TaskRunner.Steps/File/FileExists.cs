using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskRunner.Steps.File
{
  public class FileExists : ITaskBuilderStep
  {
    // TODO: [DOCS] (FileExists) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public FileExists()
    {
      Name = "File.Exists";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (FileExists) Complete this step

      return true;
    }
  }
}
