using System.Collections.Generic;
using TaskRunner.Core.Extensions;

namespace TaskRunner.Core.Steps
{
  public class StepContext
  {
    // TODO: [DOCS] (StepContext) Document this

    public int StepId { get; set; }

    public string StepName { get; set; }

    public Dictionary<string, string> Arguments { get; set; }

    public Dictionary<string, Dictionary<string, string>> TaskData { get; set; }

    // Constructor
    public StepContext()
    {
      TaskData = new Dictionary<string, Dictionary<string, string>>();
    }


    // Public methods
    public string GetArgument(string argument, string fallback = null)
    {
      // TODO: [TESTS] (StepContextExtensions) Add tests

      // Ensure that we have something to work with
      if (Arguments == null || Arguments.Count == 0)
        return fallback;

      // Check to see if the requested argument exists
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (!Arguments.ContainsKey(argument))
        return fallback;

      // Argument exists, return it
      return Arguments[argument];
    }

    public void Publish(string key, string value)
    {
      // TODO: [TESTS] (StepContextExtensions) Add tests

      // Ensure we have a valid key to work with
      if (string.IsNullOrWhiteSpace(key))
        return;

      // Ensure we have a dictionary to store data in
      if (!TaskData.ContainsKey(StepName))
        TaskData[StepName] = new Dictionary<string, string>();

      // Publish the given data
      TaskData[StepName][key.TrimAndLower()] = value;
    }

    public void Publish(string key, int value)
      => Publish(key, value.ToString("D"));

    public void Publish(string key, double value)
      => Publish(key, value.ToString("N"));
  }
}
