using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Steps;
using TaskRunner.Shared.Validators;

namespace TaskRunner.Validators.Core
{
  public class PropertyContains : BaseValidator
  {
    public PropertyContains(IAppLogger logger)
    : base(logger, "Property.Contains")
    {
      RegisterInput("Property");
      RegisterInput("Contains");
    }

    public override bool Validate(StepContext context, ValidatorConfig config)
    {
      // TODO: [TESTS] (PropertyContains) Add tests
      // TODO: [REFACTOR] (PropertyContains) Create a helper method for getting arguments

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
