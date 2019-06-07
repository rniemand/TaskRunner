using System.Collections.Generic;
using TaskRunner.Shared.Tasks;

namespace TaskRunner.Shared.Interfaces.Steps
{
  public interface IStepSuccessValidator
  {
    bool Enabled { get; set; }
    string Name { get; set; }
    Dictionary<string, string> Arguments { get; set; }

    bool Validate(StepContext context);
  }
}
