using System.Collections.Generic;
using System.Diagnostics;
using TaskRunner.Core.Enums;

namespace TaskRunner.Core.Configuration
{
  [DebuggerDisplay("{StepId}: {StepName} ({Step})")]
  public class RunnerStep
  {
    // TODO: [DOCS] (RunnerStep) Document this

    /// <summary>
    /// Indicates the enabled state of the current step.
    /// Defaults to TRUE
    /// </summary>
    public bool Enabled { get; set; }

    // TODO: [DOCS] (RunnerStep) Decide if {@xx:xx} works for this
    /// <summary>
    /// The name of the step - this value is used to publish data against.
    /// In following steps you can access it's data by {@StepName:PropertyName}
    /// </summary>
    public string StepName { get; set; }

    /// <summary>
    /// The ID for this step in the main Tasks.Steps array.
    /// This value is auto-generated.
    /// </summary>
    public int StepId { get; set; }

    /// <summary>
    /// Inputs to pass to the step - differs based on the selected step.
    /// These arguments will be cast into the expected types by the step
    /// </summary>
    public Dictionary<string, string> Inputs { get; set; }

    /// <summary>
    /// Action to take when the given step fails.
    /// Defaults to STOP
    /// </summary>
    public StepFailAction FailAction { get; set; }

    /// <summary>
    /// Additional (optional) configuration to pass to the fail action
    /// </summary>
    public string FailActionArg { get; set; }

    /// <summary>
    /// The registered name of the step that you want to execute (name defined in Step)
    /// e.g. "Core.ConsoleLog"
    /// </summary>
    public string Step { get; set; }

    public RunnerStep()
    {
      Enabled = true;
      FailAction = StepFailAction.Stop;
      Inputs = new Dictionary<string, string>();
    }
  }
}
