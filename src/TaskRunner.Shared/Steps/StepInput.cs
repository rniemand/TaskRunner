using TaskRunner.Shared.Enums;

namespace TaskRunner.Shared.Steps
{
  public struct StepInput
  {
    public string Name { get; set; }
    public bool Required { get; set; }
    public InputValidator Validator { get; set; }
    public string DefaultValue { get; set; }
  }
}
