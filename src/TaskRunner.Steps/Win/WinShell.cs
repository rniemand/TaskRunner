using System.Collections.Generic;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Win
{
  public class WinShell : IRunnerStep
  {
    // TODO: [DOCS] (WinShell) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public WinShell()
    {
      Name = "Win.Shell";
    }

    public bool Execute(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      // TODO: [COMPLETE] (WinShell) Complete step

      return true;
    }
  }
}
