using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Zip
{
  public class ZipAddFiles : IRunnerStep
  {
    // TODO: [DOCS] (ZipAddFiles) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public ZipAddFiles()
    {
      Name = "Zip.AddFiles";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (ZipAddFiles) Complete step

      return true;
    }
  }
}
