using System;
using System.Collections.Generic;
using System.Linq;
using TaskRunner.Core.Enums;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Core.Steps
{
  public class TaskStepBase : IRunnerStep
  {
    public string Name { get; }
    public IAppLogger Logger { get; set; }
    public List<StepInpt> Inputs { get; set; }


    // Constructor
    public TaskStepBase(IAppLogger logger, string name)
    {
      Logger = logger;
      Name = name;

      Inputs = new List<StepInpt>();
    }


    // Public methods
    public virtual bool Execute(StepContext context)
    {
      throw new NotImplementedException();
    }

    public bool RunTaskValidators(StepContext context)
    {
      // TODO: [TESTS] (TaskStepBase) Add tests

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

    public bool RequiredInputsSet(Dictionary<string, string> stepInputs, string stepName, string taskName)
    {
      // TODO: [TESTS] (TaskStepBase) Add tests

      // Ensure that all required parameters are present
      var requiredParams = Inputs.Where(p => p.Required).ToList();

      foreach (var param in requiredParams)
      {
        if (stepInputs.ContainsKey(param.Name))
          continue;

        Logger.Error("Required argument '{arg}' is missing from step '{step}' for task '{task}'",
          param.Name, stepName, taskName);

        return false;
      }

      // All required parameters are set - validation passed
      return true;
    }

    public string GetInput(StepContext context, string name)
    {
      // TODO: [TESTS] (TaskStepBase) Add tests

      // TODO: [COMPLETE] (TaskStepBase) Handle this better - should we LOG or THROW?
      if (Inputs.All(x => x.Name != name))
        throw new Exception($"Requested parameter {name} was not provided");

      // Grab the parameter - we may need the default value
      var param = Inputs.FirstOrDefault(x => x.Name == name);

      var argument = context.GetArgument(param.Name, param.DefaultValue);

      // TODO: [COMPLETE] (TaskStepBase) Validate the parameter based on "param.Validator"

      return argument;
    }

    public T GetInput<T>(StepContext context, string name, Func<string, T> convertFunc)
    {
      // TODO: [DOCS] (TaskStepBase) Document this function
      // TODO: [TESTS] (TaskStepBase) Add tests

      var value = string.Empty;

      if (Inputs.Any(x => x.Name == name))
      {
        var defaultValue = Inputs.First(x => x.Name == name).DefaultValue;
        value = context.GetArgument(name, defaultValue);
      }

      return convertFunc.Invoke(value);
    }

    public void RegisterInput(string inputName, InputValidator validator, bool required = true, string defaultValue = null)
    {
      // TODO: [TESTS] (TaskStepBase) Add tests

      Inputs.Add(new StepInpt
      {
        Name = inputName,
        Required = required,
        Validator = validator,
        DefaultValue = defaultValue
      });
    }
  }
}
