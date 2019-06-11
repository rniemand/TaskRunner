using System.Collections.Generic;

namespace TaskRunner.Shared.Configuration
{
  public class ValidatorConfig
  {
    // TODO: [DOCS] (ValidatorConfig) Document this

    public bool Enabled { get; set; }

    /// <summary>
    /// The name of the registered validator to run
    /// </summary>
    public string Validator { get; set; }

    /// <summary>
    /// Arguments to pass to the validator
    /// </summary>
    public Dictionary<string, string> Arguments { get; set; }

    public ValidatorConfig()
    {
      Enabled = true;
      Arguments = new Dictionary<string, string>();
    }
  }
}
