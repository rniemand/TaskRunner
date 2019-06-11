using System;
using System.Collections.Generic;
using System.Linq;
using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Interfaces.Logging;
using TaskRunner.Shared.Interfaces.Steps;
using TaskRunner.Shared.Steps;

namespace TaskRunner.Shared.Validators
{
  public abstract class BaseValidator : IStepValidator
  {
    // TODO: [DOCS] (BaseValidator) Document this

    public string Name { get; }
    public IAppLogger Logger { get; }

    private readonly List<ValidatorInput> _inputs;


    // Constructor
    protected BaseValidator(IAppLogger logger, string name)
    {
      Logger = logger;
      Name = name;
      _inputs = new List<ValidatorInput>();
    }


    // Public methods | called by "implemented class"
    public virtual bool Validate(StepContext context, StepValidatorConfig config)
    {
      // TODO: [TESTS] (BaseValidator) Add tests

      throw new NotImplementedException();
    }

    public string GetInput(StepValidatorConfig config, string inputName)
    {
      // TODO: [TESTS] (BaseValidator) Add tests

      if (string.IsNullOrWhiteSpace(inputName))
        return string.Empty;

      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (!config.Arguments.ContainsKey(inputName))
        return string.Empty;

      return config.Arguments[inputName];
    }


    // Public methods | called from "BaseStep"
    public bool Run(StepContext context, StepValidatorConfig config)
    {
      // TODO: [TESTS] (BaseValidator) Add tests

      return Validate(context, config);
    }


    // Public methods | called from "TaskRunner.Validators"
    public void RegisterInput(string inputName, bool required = true)
    {
      _inputs.Add(new ValidatorInput
      {
        Name = inputName,
        Required = required
      });
    }


    // Public methods | called from "TaskRunnerService"
    public bool HasRequiredInputs(StepValidatorConfig config)
    {
      // TODO: [TESTS] (BaseValidator) Add tests

      var requiredInputs = _inputs.Where(x => x.Required).ToList();

      foreach (var input in requiredInputs)
      {
        if (config.Arguments.ContainsKey(input.Name))
          continue;

        // Required input is missing
        return false;
      }

      // All required inputs are present
      return true;
    }
  }
}
