using System.Collections.Generic;

namespace TaskRunner.Core.Steps
{
  public class StepContext
  {
    // TODO: [DOCS] (StepContext) Document this

    public int StepId { get; set; }

    public string StepName { get; set; }

    public Dictionary<string, string> Arguments { get; set; }

    public Dictionary<string, Dictionary<string, string>> TaskData { get; set; }
    
    public StepContext()
    {
      TaskData = new Dictionary<string, Dictionary<string, string>>();
    }
  }
}
