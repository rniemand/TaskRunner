using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Directory
{
  public class DirectoryExists : ITaskBuilderStep
  {
    // TODO: [DOCS] (DirectoryExists) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public DirectoryExists()
    {
      Name = "Directory.Exists";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (DirectoryExists) Complete step

      return true;
    }
  }
}
