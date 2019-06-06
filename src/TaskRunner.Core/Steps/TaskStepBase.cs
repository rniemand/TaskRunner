﻿using System;
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
    public List<StepParameter> Parameters { get; set; }

    // Constructor
    public TaskStepBase(IAppLogger logger, string name)
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

    public string GetInput(StepContext context, string name)
    {
      // TODO: [TESTS] (TaskStepBase) Add tests

      // TODO: [COMPLETE] (TaskStepBase) Handle this better - should we LOG or THROW?
      if (Parameters.All(x => x.Name != name))
        throw new Exception($"Requested parameter {name} was not provided");

      // Grab the parameter - we may need the default value
      var param = Parameters.FirstOrDefault(x => x.Name == name);

      var argument = context.GetArgument(param.Name, param.DefaultValue);

      // TODO: [COMPLETE] (TaskStepBase) Validate the parameter based on "param.Validator"

      return argument;
    }

    public void RegisterInput(string input, InputValidator validator, bool required = true, string defaultValue = null)
    {
      // TODO: [TESTS] (TaskStepBase) Add tests

      Parameters.Add(new StepParameter
      {
        Name = input,
        Required = required,
        Validator = validator,
        DefaultValue = defaultValue
      });
    }
  }
}
