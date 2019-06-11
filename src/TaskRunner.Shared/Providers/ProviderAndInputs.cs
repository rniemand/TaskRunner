using System.Collections.Generic;

namespace TaskRunner.Shared.Providers
{
  public class ProviderAndInputs
  {
    public BaseProvider Provider { get; set; }
    public Dictionary<string, string> Inputs { get; set; }

    public ProviderAndInputs()
    {
      Inputs = new Dictionary<string, string>();
    }
  }
}
