using System;
using TaskBuilder.Common.Tasks.Enums;

namespace TaskBuilder.Common.Tasks
{
  public class TaskBuilderTask
  {
    // TODO: [DOCS] (TaskBuilderTask) Document this

    /// <summary>
    /// Human friendly name for the given task.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indicates if the current task is enabled.
    /// Defaults to TRUE
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The steps that make up the task, steps will be executed in the order they appear
    /// </summary>
    public TaskBuilderStep[] Steps { get; set; }

    /// <summary>
    /// The frequency modifier for the task (how often it should run)
    /// </summary>
    public TaskInterval Frequency { get; set; }

    /// <summary>
    /// Additional arguments to configure the frequency, based on the selected frequency these
    /// values will be cast and interpenetrated by the scheduler
    /// </summary>
    public string FrequencyArgs { get; set; }

    /// <summary>
    /// When enabled this task will run at application start regardless of the last run time or schedule
    /// </summary>
    public bool RunAtStartup { get; set; }

    /// <summary>
    /// The last time this task was run
    /// </summary>
    public DateTime? LastRunTime { get; set; }

    /// <summary>
    /// The next run time for this task
    /// </summary>
    public DateTime? NextRunTime { get; set; }

    public TaskBuilderTask()
    {
      Enabled = true;
    }
  }
}
