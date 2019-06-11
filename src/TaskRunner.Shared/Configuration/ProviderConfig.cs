using System.Collections.Generic;

namespace TaskRunner.Shared.Configuration
{
  public class ProviderConfig
  {
    // TODO: [DOCS] (ProviderConfig) Document this

    public bool Enabled { get; set; }
    public string Provider { get; set; }
    public Dictionary<string, string> Inputs { get; set; }

    public ProviderConfig()
    {
      Enabled = true;
      Inputs = new Dictionary<string, string>();
    }
  }
}
