using System.Collections.Generic;

namespace TaskRunner.Core.Steps.Interfaces
{
  public interface IRunnerStep
  {
    string Name { get; }

    bool Execute(StepContext context, List<IStepSuccessValidator> validators = null);
  }
}
