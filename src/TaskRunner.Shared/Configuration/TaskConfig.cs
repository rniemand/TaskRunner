using System;
using TaskRunner.Shared.Enums;

namespace TaskRunner.Shared.Configuration
{
  public class TaskConfig
  {
    // TODO: [DOCS] (TaskConfig) Document this

    public string Name { get; set; }

    public bool Enabled { get; set; }

    public StepConfig[] Steps { get; set; }

    public TaskRunInterval Frequency { get; set; }

    public string FrequencyArgs { get; set; }

    public bool RunAtStartup { get; set; }

    public DateTime? LastRunTime { get; set; }

    public DateTime? NextRunTime { get; set; }

    public long RunCount { get; set; }

    public string TaskFilePath { get; set; }


    // Constructor and sensible defaults
    public TaskConfig()
    {
      Enabled = true;
      FrequencyArgs = string.Empty;
      RunAtStartup = false;
      RunCount = 0;
    }
  }
}
