using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Zip
{
  public class ZipRemoveFile : ITaskBuilderStep
  {
    // TODO: [DOCS] (ZipRemoveFile) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public ZipRemoveFile()
    {
      Name = "Zip.RemoveFile";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (ZipRemoveFile) Complete step

      return true;
    }
  }
}
