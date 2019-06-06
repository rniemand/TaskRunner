using System.Text.RegularExpressions;
using TaskRunner.Core.Steps;

namespace TaskRunner.Core.Extensions
{
  public static class StepContextExtensions
  {
    private const string TaskDataRx = @"({@([^\.]+)\.([^}]+)})";

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
