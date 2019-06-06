using System.Collections.Generic;
using System.Text.RegularExpressions;
using TaskBuilder.Common.Tasks;

namespace TaskBuilder.Common.Extensions
{
  public static class StepContextExtensions
  {
    private const string TaskDataRx = @"({@([^\.]+)\.([^}]+)})";

    public static string GetArgument(this StepContext stepContext, string argument, string fallback = null)
    {
      // TODO: [TESTS] (StepContextExtensions) Add tests

      // Ensure that we have something to work with
      if (stepContext?.Arguments == null || stepContext.Arguments.Count == 0)
        return fallback;

      // Check to see if the requested argument exists
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (!stepContext.Arguments.ContainsKey(argument))
        return fallback;

      // Argument exists, return it
      return stepContext.Arguments[argument];
    }

    // TODO: [RENAME] (StepContextExtensions) Come up with a better name for this
    public static void AppendOutputValue(this StepContext stepContext, string key, string value)
    {
      // TODO: [TESTS] (StepContextExtensions) Add tests

      if (stepContext == null || string.IsNullOrWhiteSpace(key))
        return;

      stepContext.EnsureOutputStepExists();

      stepContext.TaskData[stepContext.StepName][key.TrimAndLower()] = value;
    }

    // TODO: [RENAME] (StepContextExtensions) Come up with a better name for this
    public static void AppendOutputValue(this StepContext stepContext, string key, int value)
    {
      stepContext.AppendOutputValue(key, value.ToString("D"));
    }

    // TODO: [RENAME] (StepContextExtensions) Come up with a better name for this
    public static void AppendOutputValue(this StepContext stepContext, string key, double value)
    {
      stepContext.AppendOutputValue(key, value.ToString("N"));
    }

    // TODO: [RENAME] (StepContextExtensions) Come up with a better name for this
    public static void EnsureOutputStepExists(this StepContext stepContext)
    {
      // TODO: [TESTS] (StepContextExtensions) Add tests

      if (stepContext == null)
        return;

      if (stepContext.TaskData.ContainsKey(stepContext.StepName))
        return;

      stepContext.TaskData[stepContext.StepName] = new Dictionary<string, string>();
    }

    public static bool HasStepData(this StepContext stepContext, string stepName)
    {
      // TODO: [TESTS] (StepContextExtensions) Add tests

      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (stepContext?.TaskData == null || !stepContext.TaskData.ContainsKey(stepName))
        return false;

      return true;
    }

    public static string GetStepData(this StepContext stepContext, string stepName, string key)
    {
      // TODO: [TESTS] (StepContextExtensions) Add tests

      if (string.IsNullOrWhiteSpace(key))
        return string.Empty;

      if (!stepContext.HasStepData(stepName))
        return string.Empty;

      if (!stepContext.TaskData[stepName].ContainsKey(key))
        return string.Empty;

      return stepContext.TaskData[stepName][key];
    }

    public static string ReplacePlaceholders(this StepContext stepContext, string input)
    {
      // TODO: [TESTS] (TaskRunnerService) Add tests

      if (string.IsNullOrWhiteSpace(input))
        return input;

      if (!input.MatchesRxPattern(TaskDataRx))
        return input;

      var matches = input.GetRxMatches(TaskDataRx);

      foreach (Match match in matches)
      {
        var placeholder = match.Groups[1].Value;
        var stepName = match.Groups[2].Value;
        var dataKey = match.Groups[3].Value;
        var replacement = stepContext.GetStepData(stepName, dataKey);

        input = input.Replace(placeholder, replacement);
      }

      return input;
    }
  }
}
