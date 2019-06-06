using System.Collections.Generic;
using System.Text.RegularExpressions;
using TaskRunner.Core.Extensions;

namespace TaskRunner.Core.Steps
{
  public class StepContext
  {
    // TODO: [DOCS] (StepContext) Document this

    public int StepId { get; set; }
    public string StepName { get; set; }
    public bool DataPublished { get; set; }


    private const string TaskDataRx = @"({@([^\.]+)\.([^}]+)})";
    // TODO: [REFACTOR] (StepContext) Convert to private
    public Dictionary<string, string> Arguments { get; set; }
    // TODO: [REFACTOR] (StepContext) Convert to private
    public Dictionary<string, Dictionary<string, string>> PublishedData { get; set; }


    // Constructor
    public StepContext()
    {
      // TODO: [TESTS] (StepContext) Add tests

      PublishedData = new Dictionary<string, Dictionary<string, string>>();
      DataPublished = false;
    }


    // Public methods
    public void SetArguments(Dictionary<string, string> arguments)
    {
      // TODO: [TESTS] (StepContext) Add tests

      Arguments.Clear();

      if(arguments.Count == 0)
        return;

      Arguments = arguments;
    }

    public string GetArgument(string argument, string fallback = null)
    {
      // TODO: [TESTS] (StepContext) Add tests

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

    public string ReplaceTags(string input)
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Ensure we have something to work with
      if (string.IsNullOrWhiteSpace(input))
        return input;

      // Look for any placeholders
      if (!input.MatchesRxPattern(TaskDataRx))
        return input;

      // Find and replace all placeholders
      var matches = input.GetRxMatches(TaskDataRx);
      foreach (Match match in matches)
      {
        var stepName = match.Groups[2].Value;
        var dataKey = match.Groups[3].Value;

        input = input.Replace(
          match.Groups[1].Value,
          GetPublishedValue(stepName, dataKey)
        );
      }

      // Return the modified input
      return input;
    }

    public void Publish(string key, string value)
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Ensure we have a valid key to work with
      if (string.IsNullOrWhiteSpace(key))
        return;

      // Ensure we have a dictionary to store data in
      if (!PublishedData.ContainsKey(StepName))
        PublishedData[StepName] = new Dictionary<string, string>();

      // Publish the given data
      PublishedData[StepName][key.TrimAndLower()] = value;
      DataPublished = true;
    }

    public void Publish(string key, int value)
    {
      // TODO: [TESTS] (StepContext) Add tests

      Publish(key, value.ToString("D"));
    }

    public void Publish(string key, double value)
    {
      // TODO: [TESTS] (StepContext) Add tests

      Publish(key, value.ToString("N"));
    }


    // Internal methods
    public string GetPublishedValue(string stepName, string key)
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Ensure that we have a valid stepName and key
      if (string.IsNullOrWhiteSpace(stepName) || string.IsNullOrWhiteSpace(key))
        return string.Empty;

      // Check if the step published any data
      if (!PublishedData.ContainsKey(stepName))
        return string.Empty;

      // Check if the requested key exists
      return !PublishedData[stepName].ContainsKey(key)
        ? string.Empty
        : PublishedData[stepName][key];
    }
  }
}
