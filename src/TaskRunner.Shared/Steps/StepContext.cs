using System.Collections.Generic;
using System.Text.RegularExpressions;
using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Extensions;
using TaskRunner.Shared.Validators;

namespace TaskRunner.Shared.Steps
{
  public class StepContext
  {
    // TODO: [DOCS] (StepContext) Document this

    public int StepId { get; private set; }
    public string StepName { get; private set; }
    public bool DataPublished { get; private set; }
    public Dictionary<string, string> Inputs { get; private set; }
    public List<ValidatorAndArguments> Validators { get; private set; }
    public Dictionary<int, string> StepNameLookup { get; set; }

    public string TaskName { get; set; }

    private const string TaskDataRx = @"({@([^\.]+)\.([^}]+)})";
    private readonly Dictionary<string, Dictionary<string, string>> _publishedData;

    // Constructor
    public StepContext()
    {
      // TODO: [TESTS] (StepContext) Add tests

      Inputs = new Dictionary<string, string>();
      _publishedData = new Dictionary<string, Dictionary<string, string>>();
      StepNameLookup = new Dictionary<int, string>();
      Validators = new List<ValidatorAndArguments>();
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
    public void SetCurrentStep(
      StepConfig currentStep,
      Dictionary<string, string> inputs,
      List<ValidatorAndArguments> validators)
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Set the current step ID and Name
      StepId = currentStep.StepId;
      StepName = currentStep.Name;

      // Reset core properties
      DataPublished = false;
      Inputs.Clear();
      Validators.Clear();

      // Wire up all required collections
      Inputs = inputs;
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

        if (stepName.IsNumeric())
        {
          var stepId = stepName.ToInt(-1);

          // TODO: [REVISE] (StepContext) Handle this better
          if (stepId == -1)
            return input;

          if (!StepNameLookup.ContainsKey(stepId))
            return input;

          stepName = StepNameLookup[stepId];
        }

        input = input.Replace(
          match.Groups[1].Value,
          GetPublishedData(stepName, dataKey)
        );
      }

      // Return the modified input
      return input;
    }



    // Public methods | called from "BaseStep"
    public string GetInput(string input, string fallback = null)
    {
      // TODO: [TESTS] (StepContext) Add tests

      // Ensure that we have something to work with
      if (Inputs == null || Inputs.Count == 0)
        return fallback;

      // Check to see if the requested input exists
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (!Inputs.ContainsKey(input))
        return fallback;

      // Argument exists, return it
      return Inputs[input];
    }

    public Dictionary<string, string> GetResolvedInputs()
    {
      // TODO: [TESTS] (StepContext) Add tests

      return Inputs;
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
