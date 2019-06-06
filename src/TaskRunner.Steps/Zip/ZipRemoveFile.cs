using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Zip
{
  public class ZipRemoveFile : IRunnerStep
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
