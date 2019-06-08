using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Interfaces.Logging;
using TaskRunner.Shared.Tasks;
using TaskRunner.Shared.Validators;

namespace TaskRunner.Validators.Core
{
  public class PropertyContainsValidator : BaseStepValidator
  {
    public PropertyContainsValidator(IAppLogger logger)
    : base(logger, "Property.Contains")
    {
      RegisterInput("Property");
      RegisterInput("Contains");
    }

    public override bool Validate(StepContext context, StepValidatorConfig config)
    {
      // TODO: [TESTS] (PropertyContainsValidator) Add tests
      // TODO: [REFACTOR] (PropertyContainsValidator) Create a helper method for getting arguments

      var property = GetInput(config, "Property");
      var contains = GetInput(config, "Contains");

      var propertyValue = context.GetPublishedData(context.StepName, property);

      // Run a simple response validation on the step
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (string.IsNullOrWhiteSpace(propertyValue))
        return false;

      return propertyValue.Contains(contains);
    }
  }
}
