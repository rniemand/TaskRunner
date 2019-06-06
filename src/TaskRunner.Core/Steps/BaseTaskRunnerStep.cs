using System.Collections.Generic;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Core.Steps
{
  public class BaseTaskRunnerStep : IRunnerStep
  {
    public string Name { get; }
    public IAppLogger Logger { get; set; }

    public BaseTaskRunnerStep(IAppLogger logger, string name)
    {
      Logger = logger;
      Name = name;
    }

    public virtual bool Execute(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      throw new System.NotImplementedException();
    }

    public bool RunTaskValidators(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      if (validators == null)
        return true;

      // Execute each validation function looking for failures
      foreach (var validator in validators)
      {
        if (validator.Validate(context))
          continue;

        // Validation failed, log and return false
        Logger.Error("Step validation failed for '{step}' in task '{task}'",
          context.StepName, context.TaskName);

        return false;
      }

      return true;
    }
  }
}
