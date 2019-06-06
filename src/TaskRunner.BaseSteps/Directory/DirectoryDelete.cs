using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Directory
{
  public class DirectoryDelete : ITaskBuilderStep
  {
    // TODO: [DOCS] (DirectoryDelete) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public DirectoryDelete()
    {
      Name = "Directory.Delete";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (DirectoryDelete) Complete step

      return true;
    }
  }
}
