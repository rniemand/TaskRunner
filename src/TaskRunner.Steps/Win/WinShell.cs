using TaskRunner.Core.Tasks;
using TaskRunner.Core.Tasks.Interfaces;

namespace TaskRunner.Steps.Win
{
  public class WinShell : ITaskBuilderStep
  {
    // TODO: [DOCS] (WinShell) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public WinShell()
    {
      Name = "Win.Shell";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (WinShell) Complete step

      return true;
    }
  }
}
