using TaskRunner.Shared.Tasks;
using TaskRunner.Shared.Tasks.Interfaces;

namespace TaskRunner.Steps.Json
{
  public class JsonLoadFromFile : ITaskBuilderStep
  {
    // TODO: [DOCS] (JsonLoadFromFile) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public JsonLoadFromFile()
    {
      Name = "Json.LoadFromFile";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (JsonLoadFromFile) Complete step

      return true;
    }
  }
}
