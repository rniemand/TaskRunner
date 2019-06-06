using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Zip
{
  public class ZipCreate : IRunnerStep
  {
    // TODO: [DOCS] (ZipCreate) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public ZipCreate()
    {
      Name = "Zip.Create";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (ZipCreate) Complete this step

      return true;
    }
  }
}
