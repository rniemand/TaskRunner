using System.Collections.Generic;
using TaskRunner.Shared.Interfaces.Steps;
using TaskRunner.Shared.Tasks;

namespace TaskRunner.Core.Validators.Core
{
  public class PocSuccessValidator : IStepSuccessValidator
  {
    // TODO: [MOVE] (PocSuccessValidator) Move this into config (not required for the actual validator)
    public bool Enabled { get; set; }
    // TODO: [MOVE] (PocSuccessValidator) Move this into config (not required for the actual validator)
    public string Name { get; set; }
    public Dictionary<string, string> Arguments { get; set; }

    public PocSuccessValidator()
    {
      Name = "PocValidator";
      Arguments = new Dictionary<string, string>();
    }

    public bool Validate(StepContext context)
    {
      // TODO: [REFACTOR] (PocSuccessValidator) Create a helper method for getting arguments
      var property = Arguments["Property"];
      var contains = Arguments["Contains"];

      var propertyValue = context.GetPublishedData(context.StepName, property);

      // Run a simple response validation on the step
      if (string.IsNullOrWhiteSpace(propertyValue))
        return false;

      return propertyValue.Contains(contains);
    }
  }
}
