using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Json
{
  public class JsonSaveToFile : ITaskBuilderStep
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
