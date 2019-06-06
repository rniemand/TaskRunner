using System.Collections.Generic;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Zip
{
  public class ZipRemoveFiles : IRunnerStep
  {
    // TODO: [DOCS] (ZipRemoveFiles) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public ZipRemoveFiles()
    {
      Name = "Zip.RemoveFiles";
    }

    public bool Execute(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      // TODO: [COMPLETE] (ZipRemoveFiles) Complete step

      return true;
    }
  }
}
