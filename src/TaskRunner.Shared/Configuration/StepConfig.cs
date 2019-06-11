using System.Collections.Generic;
using System.Diagnostics;
using TaskRunner.Shared.Enums;

namespace TaskRunner.Shared.Configuration
{
  [DebuggerDisplay("{StepId}: {Name} ({Step})")]
  public class StepConfig
  {
    // TODO: [DOCS] (StepConfig) Document this

    /// <summary>
    /// Indicates the enabled state of the current step.
    /// Defaults to TRUE
    /// </summary>
    public bool Enabled { get; set; }

    // TODO: [DOCS] (StepConfig) Decide if {@xx:xx} works for this
    /// <summary>
    /// The name of the step - this value is used to publish data against.
    /// In following steps you can access it's data by {@Name:PropertyName}
    /// </summary>
    public string Name { get; set; }

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

    /// <summary>
    /// List of validators to run after the step has executed. These will determine the overall
    /// success of the step execution. If there are no validators we will fall back to using the
    /// determined outcome from the step itself. This is mainly used to allow the user to add
    /// additional validation to a step. rough doc - will update
    /// </summary>
    public List<ValidatorConfig> Validators { get; set; }

    /// <summary>
    /// Collection of providers to run before the current step is executed, providers are used
    /// to inject additional values into the step pipeline.
    /// </summary>
    public List<ProviderConfig> Providers { get; set; }

    public StepConfig()
    {
      Enabled = true;
      FailAction = StepFailAction.Stop;
      Inputs = new Dictionary<string, string>();
      Validators = new List<ValidatorConfig>();
      Providers = new List<ProviderConfig>();
    }
  }
}
