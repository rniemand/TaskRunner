using TaskRunner.Core.Tasks;
using TaskRunner.Core.Tasks.Interfaces;

namespace TaskRunner.Steps.Zip
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
