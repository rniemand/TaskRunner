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
    public bool DataPublished { get; private set; }
    public string TaskName { get; set; }

    private const string TaskDataRx = @"({@([^\.]+)\.([^}]+)})";

    private Dictionary<string, string> _arguments;
    private readonly Dictionary<string, Dictionary<string, string>> _publishedData;


    // Constructor
    public StepContext()
    {
      // TODO: [TESTS] (StepContext) Add tests

      _arguments = new Dictionary<string, string>();
      _publishedData = new Dictionary<string, Dictionary<string, string>>();
      DataPublished = false;
    }


    // Public methods
    public void SetArguments(Dictionary<string, string> arguments)
    {
      // TODO: [TESTS] (StepContext) Add tests

      _arguments.Clear();
      DataPublished = false;

      if (arguments.Count == 0)
        return;

      _arguments = arguments;
    }

    public string GetArgument(string argument, string fallback = null)
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Ensure that we have something to work with
      if (_arguments == null || _arguments.Count == 0)
        return fallback;

      // Check to see if the requested argument exists
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (!_arguments.ContainsKey(argument))
        return fallback;

      // Argument exists, return it
      return _arguments[argument];
    }

    public Dictionary<string, string> GetCurrentStepsPublishedData()
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Check to see if there is any published data to return
      if (_publishedData.Count == 0 || !_publishedData.ContainsKey(StepName))
        return new Dictionary<string, string>();

      return _publishedData[StepName];
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
      if (!_publishedData.ContainsKey(StepName))
        _publishedData[StepName] = new Dictionary<string, string>();

      // Publish the given data
      _publishedData[StepName][key.TrimAndLower()] = value;
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

    public string GetPublishedData(string key)
    {
      // TODO: [TESTS] (StepContext) Add tests

      if (string.IsNullOrWhiteSpace(key) || !_publishedData.ContainsKey(StepName))
        return string.Empty;

      if (!_publishedData[StepName].ContainsKey(key))
        return string.Empty;

      return _publishedData[StepName][key];
    }


    // Internal methods
    public string GetPublishedValue(string stepName, string key)
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Ensure that we have a valid stepName and key
      if (string.IsNullOrWhiteSpace(stepName) || string.IsNullOrWhiteSpace(key))
        return string.Empty;

      // Check if the step published any data
      if (!_publishedData.ContainsKey(stepName))
        return string.Empty;

      // Check if the requested key exists
      return !_publishedData[stepName].ContainsKey(key)
        ? string.Empty
        : _publishedData[stepName][key];
    }
  }
}
