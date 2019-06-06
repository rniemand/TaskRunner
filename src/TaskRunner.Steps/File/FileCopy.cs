using TaskRunner.Shared.Tasks;
using TaskRunner.Shared.Tasks.Interfaces;

namespace TaskRunner.Steps.File
{
  public class FileCopy : ITaskBuilderStep
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
