using TaskRunner.Core.Tasks;
using TaskRunner.Core.Tasks.Interfaces;

namespace TaskRunner.Steps.Zip
{
  public class ZipCreate : ITaskBuilderStep
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
