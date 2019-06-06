using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Zip
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
