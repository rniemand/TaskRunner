using System;
using TaskRunner.Shared.Enums;

namespace TaskRunner.Shared.Configuration
{
  public class TaskConfig
  {
    // TODO: [DOCS] (TaskConfig) Document this

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// Human friendly name for the given task.
    /// </summary>
    public string Name { get; set; }

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// Indicates if the current task is enabled.
    /// Defaults to TRUE
    /// </summary>
    public bool Enabled { get; set; }

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// The steps that make up the task, steps will be executed in the order they appear
    /// </summary>
    public StepConfig[] Steps { get; set; }

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// The frequency modifier for the task (how often it should run)
    /// </summary>
    public TaskInterval Frequency { get; set; }

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// Additional arguments to configure the frequency, based on the selected frequency these
    /// values will be cast and interpenetrated by the scheduler
    /// </summary>
    public string FrequencyArgs { get; set; }

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// When enabled this task will run at application start regardless of the last run time or schedule
    /// </summary>
    public bool RunAtStartup { get; set; }

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// The last time this task was run
    /// </summary>
    public DateTime? LastRunTime { get; set; }

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// The next run time for this task
    /// </summary>
    public DateTime? NextRunTime { get; set; }

    // TODO: [COMPLETE] (TaskConfig) Ensure that this property is being set / updated
    /// <summary>
    /// Tracks the amount of times the given task has been run
    /// </summary>
    public long RunCount { get; set; }

    /// <summary>
    /// Path to the physical task file
    /// </summary>
    public string TaskFilePath { get; set; }

    public TaskConfig()
    {
      Enabled = true;
      FrequencyArgs = string.Empty;
      RunAtStartup = false;
      RunCount = 0;
    }
  }
}
