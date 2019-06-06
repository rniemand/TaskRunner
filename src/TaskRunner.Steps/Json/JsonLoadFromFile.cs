using System.Collections.Generic;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Json
{
  public class JsonLoadFromFile : IRunnerStep
  {
    // TODO: [DOCS] (JsonLoadFromFile) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public JsonLoadFromFile()
    {
      Name = "Json.LoadFromFile";
    }

    public bool Execute(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      // TODO: [COMPLETE] (JsonLoadFromFile) Complete step

      return true;
    }
  }
}
