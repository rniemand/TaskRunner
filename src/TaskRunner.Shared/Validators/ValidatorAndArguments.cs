using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Validators
{
  public class ValidatorAndArguments
  {
    // TODO: [REVISE] (ValidatorAndArguments) Find a better way to do this

    public BaseValidator Validator { get; set; }

    public ValidatorConfig Config { get; set; }
  }
}
