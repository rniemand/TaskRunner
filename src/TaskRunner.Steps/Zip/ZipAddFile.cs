using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Zip
{
  public class ZipAddFile : IRunnerStep
  {
    // TODO: [DOCS] (ZipAddFile) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public ZipAddFile()
    {
      Name = "Zip.AddFile";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (ZipAddFile) Complete step

      return true;
    }
  }
}
