using System.Collections.Generic;
using TaskRunner.Core.Steps;
using TaskRunner.Core.Steps.Interfaces;

namespace TaskRunner.Steps.Http
{
  public class HttpPost : IRunnerStep
  {
    // TODO: [DOCS] (HttpPost) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public HttpPost()
    {
      Name = "Http.Post";
    }

    public bool Execute(StepContext context, List<IStepSuccessValidator> validators = null)
    {
      // TODO: [COMPLETE] (HttpPost) Complete this step

      return true;
    }
  }
}
