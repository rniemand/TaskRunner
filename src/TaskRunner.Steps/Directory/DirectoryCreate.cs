using TaskRunner.Core.Tasks;
using TaskRunner.Core.Tasks.Interfaces;

namespace TaskRunner.Steps.Directory
{
  public class DirectoryCreate : ITaskBuilderStep
  {
    // TODO: [DOCS] (DirectoryCreate) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public DirectoryCreate()
    {
      Name = "Directory.Create";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (DirectoryCreate) Complete step

      return true;
    }
  }
}
