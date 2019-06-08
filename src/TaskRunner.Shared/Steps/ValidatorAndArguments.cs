using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Validators;

namespace TaskRunner.Shared.Steps
{
  public class ValidatorAndArguments
  {
    // TODO: [REVISE] (ValidatorAndArguments) Find a better way to do this

    public BaseStepValidator Validator { get; set; }

    public StepValidatorConfig Config { get; set; }
  }
}
