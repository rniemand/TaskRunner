using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TaskRunner.Core.Enums;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Steps.Interfaces;
using TaskRunner.Shared.Tasks;

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
    public bool RunMe(StepContext context)
    {
      // TODO: [TESTS] (TaskStepBase) Add tests
      // TODO: [RENAME] (TaskStepBase) Rename this to something more meaningful

      // Time and run the step
      var executeStopwatch = Stopwatch.StartNew();
      var success = Execute(context);
      executeStopwatch.Stop();

      // TODO: [COMPLETE] (TaskStepBase) Publish total elapsed MS for step execution

      Logger.Info(
        "Step {name} took {ms} ms to run",
        context.StepName,
        executeStopwatch.ElapsedMilliseconds);


      // TODO: [COMPLETE] (TaskStepBase) Publish resolved arguments used for the task
      LogResolvedInputs(context.GetResolvedInputs(), context.StepName);

      // ReSharper disable once InvertIf
      if (context.Validators.Count > 0 && RunStepValidators(context) == false)
      {
        Logger.Error("Step '{step}' for task '{task}' failed validation - stopping task",
          context.StepName, context.TaskName);

        return false;
      }

      // Step execution was a success
      return success;
    }

    public virtual bool Execute(StepContext context)
    {
      throw new NotImplementedException();
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

      var argument = context.GetInput(param.Name, param.DefaultValue);

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
        value = context.GetInput(name, defaultValue);
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



    // Internal methods
    private bool RunStepValidators(StepContext context)
    {
      // TODO: [TESTS] (TaskStepBase) Add tests

      Logger.Debug("Running {count} validator(s) against step '{name}' from task '{task}'",
        context.Validators.Count, context.StepName, context.TaskName);

      foreach (var validator in context.Validators)
      {
        // PASS - continue
        if (validator.Validate(context))
          continue;

        // FAILURE - log and stop running validation
        Logger.Error("Step validation failed for '{step}' in task '{task}'",
          context.StepName, context.TaskName);

        // Validation FAILED
        return false;
      }

      // Validation PASSED
      Logger.Debug("Validation for step '{step}' passed", context.StepName);

      return true;
    }

    private void LogResolvedInputs(Dictionary<string, string> inputs, string stepName)
    {
      if (inputs == null || inputs.Count == 0)
        return;

      var longestKey = inputs.Select(x => x.Key.Length).Max() + 1;
      var sb = new StringBuilder()
        .Append($"Step '{stepName}' was called with the following inputs: ")
        .Append(Environment.NewLine);

      foreach (var (key, value) in inputs)
      {
        sb.Append($"    {key.PadRight(longestKey, ' ')}: {value}");
        sb.Append(Environment.NewLine);
      }

      Logger.Debug(sb.ToString());
    }
  }
}
