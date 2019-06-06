using System.Collections.Generic;

namespace TaskBuilder.Common.Tasks
{
  public class StepContext
  {
    // TODO: [DOCS] (StepContext) Document this

    public int StepId { get; set; }

    public string StepName { get; set; }

    public string Step { get; set; }

    public Dictionary<string, string> Arguments { get; set; }

    // Indexed initially on step name
    public Dictionary<string, Dictionary<string, string>> TaskData { get; set; }
    
    public StepContext()
    {
      TaskData = new Dictionary<string, Dictionary<string, string>>();
    }
  }
}
