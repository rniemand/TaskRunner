using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Json
{
  public class JsonSaveToFile : IRunnerStep
  {
    // TODO: [DOCS] (JsonSaveToFile) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public JsonSaveToFile()
    {
      Name = "Json.SaveToFile";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (JsonSaveToFile) Complete this step

      return true;
    }
  }
}
