using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Core
{
  public class WinShell : ITaskBuilderStep
  {
    // TODO: [DOCS] (WinShell) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public WinShell()
    {
      Name = "Core.WinShell";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (WinShell) Complete step

      return true;
    }
  }
}
