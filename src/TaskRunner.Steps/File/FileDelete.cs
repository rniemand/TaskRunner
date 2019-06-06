using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskRunner.Steps.File
{
  public class FileDelete : ITaskBuilderStep
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
