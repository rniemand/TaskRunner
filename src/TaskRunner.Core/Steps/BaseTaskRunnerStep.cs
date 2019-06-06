using System.Collections.Generic;
using TaskRunner.Core.Enums;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Core.Steps
{
  public class BaseTaskRunnerStep : IRunnerStep
  {
    public string Name { get; }
    public IAppLogger Logger { get; set; }
    public List<StepParameter> Parameters { get; set; }

    // Constructor
    public BaseTaskRunnerStep(IAppLogger logger, string name)
    {
      Logger = logger;
      Name = name;

      Parameters = new List<StepParameter>();
    }


    // Public methods
    public virtual bool Execute(StepContext context)
    {
      throw new System.NotImplementedException();
    }

    public bool RunTaskValidators(StepContext context)
    {
      if (context.Validators == null)
        return true;

      // Execute each validation function looking for failures
      foreach (var validator in context.Validators)
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

    public void RegisterInput(string input, InputValidator validator, bool required = true)
    {
      // TODO: [TESTS] (BaseTaskRunnerStep) Add tests

      Parameters.Add(new StepParameter
      {
        Name = input,
        Required = required,
        Validator = validator
      });
    }
  }
}
