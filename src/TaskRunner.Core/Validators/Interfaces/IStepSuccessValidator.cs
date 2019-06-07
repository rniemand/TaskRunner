using System.Collections.Generic;
using TaskRunner.Core.Steps;

namespace TaskRunner.Core.Validators.Interfaces
{
  public interface IStepSuccessValidator
  {
    bool Enabled { get; set; }
    string Name { get; set; }
    Dictionary<string, string> Arguments { get; set; }

    bool Validate(StepContext context);
  }
}
