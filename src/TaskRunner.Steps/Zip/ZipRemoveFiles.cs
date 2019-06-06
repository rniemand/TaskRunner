using TaskRunner.Shared.Tasks;
using TaskRunner.Shared.Tasks.Interfaces;

namespace TaskRunner.Steps.Zip
{
  public class ZipRemoveFiles : ITaskBuilderStep
  {
    // TODO: [DOCS] (ZipRemoveFiles) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public ZipRemoveFiles()
    {
      Name = "Zip.RemoveFiles";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (ZipRemoveFiles) Complete step

      return true;
    }
  }
}
