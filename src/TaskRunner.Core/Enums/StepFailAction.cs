namespace TaskRunner.Core.Enums
{
  public enum StepFailAction
  {
    // TODO: [DOCS] (StepFailAction) Document step fail actions

    Stop,
    Continue,
    Retry,
    Delay // maybe rename this to reschedule
  }
}
