using TaskRunner.Core.Enums;

namespace TaskRunner.Core.Steps
{
  public struct StepParameter
  {
    public string Name { get; set; }
    public bool Required { get; set; }
    public InputValidator Validator { get; set; }
    public string DefaultValue { get; set; }
  }
}
