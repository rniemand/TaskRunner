using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskRunner.Steps.Http
{
  public class HttpPost : ITaskBuilderStep
  {
    // TODO: [DOCS] (HttpPost) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public HttpPost()
    {
      Name = "Http.Post";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (HttpPost) Complete this step

      return true;
    }
  }
}
