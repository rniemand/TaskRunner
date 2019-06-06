using System.Collections.Generic;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.File
{
  public class FileExists : IRunnerStep
  {
    // TODO: [DOCS] (FileExists) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public FileExists()
    {
      Name = "File.Exists";
    }

    public bool Execute(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      // TODO: [COMPLETE] (FileExists) Complete this step

      return true;
    }
  }
}
