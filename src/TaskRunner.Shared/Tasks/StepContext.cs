using System.Collections.Generic;
using System.Text.RegularExpressions;
using TaskRunner.Shared.Extensions;
using TaskRunner.Shared.Interfaces.Steps;

namespace TaskRunner.Shared.Tasks
{
  public class StepContext
  {
    // TODO: [DOCS] (StepContext) Document this

    public int StepId { get; set; }
    public string StepName { get; set; }
    public bool DataPublished { get; private set; }
    public string TaskName { get; set; }
    public List<IStepSuccessValidator> Validators { get; private set; }

    private const string TaskDataRx = @"({@([^\.]+)\.([^}]+)})";

    // TODO: [RENAME] (StepContext) Not happy with the name "_publishedData" - think of something better
    private Dictionary<string, string> _inputs;
    private readonly Dictionary<string, Dictionary<string, string>> _publishedData;


    // Constructor
    public StepContext()
    {
      // TODO: [TESTS] (StepContext) Add tests

      _inputs = new Dictionary<string, string>();
      _publishedData = new Dictionary<string, Dictionary<string, string>>();
      Validators = new List<IStepSuccessValidator>();
      DataPublished = false;
    }



    // Public methods | called from "Steps"
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



    // Public methods | called from "TaskRunnerService"
    public void ResetDataPublished()
    {
      // TODO: [TESTS] (StepContext) Add tests

      DataPublished = false;
    }

    public void ClearStepValidators()
    {
      // TODO: [TESTS] (StepContext) Add tests

      Validators.Clear();
    }

    public void SetCurrentStepInputs(Dictionary<string, string> inputs)
    {
      // TODO: [TESTS] (StepContext) Add tests

      _inputs.Clear();

      if (inputs.Count == 0)
        return;

      _inputs = inputs;
    }

    public void RegisterSuccessValidators(List<IStepSuccessValidator> validators = null)
    {
      // TODO: [TESTS] (StepContext) Add tests

      Validators.Clear();

      if (validators == null || validators.Count == 0)
        return;

      Validators = validators;
    }

    public Dictionary<string, string> GetLastPublishedData()
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
          GetPublishedData(stepName, dataKey)
        );
      }

      // Return the modified input
      return input;
    }



    // Public methods | called from "TaskStepBase"
    public string GetInput(string input, string fallback = null)
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Ensure that we have something to work with
      if (_inputs == null || _inputs.Count == 0)
        return fallback;

      // Check to see if the requested input exists
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (!_inputs.ContainsKey(input))
        return fallback;

      // Argument exists, return it
      return _inputs[input];
    }

    public Dictionary<string, string> GetResolvedInputs()
    {
      // TODO: [TESTS] (StepContext) Add tests

      return _inputs;
    }



    // Public methods | called from "IStepSuccessValidator"
    public string GetPublishedData(string stepName, string key)
    {
      // TODO: [TESTS] (StepContext) Add tests
      // TODO: [RENAME] (StepContext) I am not happy with this name

      if (string.IsNullOrWhiteSpace(key) || !_publishedData.ContainsKey(stepName))
        return string.Empty;

      if (!_publishedData[stepName].ContainsKey(key))
        return string.Empty;

      return _publishedData[stepName][key];
    }
  }
}
