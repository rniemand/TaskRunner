using System.Collections.Generic;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Directory
{
  public class DirectoryDelete : IRunnerStep
  {
    // TODO: [DOCS] (DirectoryDelete) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public DirectoryDelete()
    {
      Name = "Directory.Delete";
    }

    public bool Execute(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      // TODO: [COMPLETE] (DirectoryDelete) Complete step

      return true;
    }
  }
}
