using System.Collections.Generic;

namespace TaskRunner.Core.Steps.Interfaces
{
  public interface IStepSuccessValidator
  {
    bool Enabled { get; set; }
    string Name { get; set; }
    Dictionary<string, string> Arguments { get; set; }

    bool Validate(StepContext context);
  }
}
