using System.Diagnostics;
using System.Net.Http;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Steps;
using TaskRunner.Shared.Enums;
using TaskRunner.Shared.Extensions;
using TaskRunner.Shared.Tasks;

namespace TaskRunner.Steps.Http
{
  public class HttpGet : TaskStepBase
  {
    // TODO: [DOCS] (HttpGet) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments
    // TODO: [DOCS] (HttpGet) Document argument: Url (required)
    // TODO: [DOCS] (HttpGet) Document output: response.status [string] (always)
    // TODO: [DOCS] (HttpGet) Document output: response.status_code [number] (always)
    // TODO: [DOCS] (HttpGet) Document output: response.headers.<x> [string] (varies)
    // TODO: [DOCS] (HttpGet) Document output: response.content [string] (always)
    // TODO: [DOCS] (HttpGet) Document output: response.content_length [number] (always)
    // TODO: [DOCS] (HttpGet) Document output: response.execution_time [double] (always) - maybe rename this?

    public HttpGet(IAppLogger logger)
      : base(logger, "Http.Get")
    {
      // TODO: [TESTS] (HttpGet) Add tests

      RegisterInput("Url", InputValidator.Url);
    }

    public override bool Execute(StepContext context)
    {
      // TODO: [TESTS] (HttpGet) Add tests

      var url = GetInput(context, "Url");

      // TODO: [REFACTOR] (HttpGet) Break out into a service (perhaps HttpService)
      // TODO: [REFACTOR] (HttpGet) Wrap in try \ catch block?

      var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
      var httpClient = new HttpClient();
      var requestTimer = Stopwatch.StartNew();

      var httpResponse = httpClient
        .SendAsync(httpRequest)
        .ConfigureAwait(false)
        .GetAwaiter()
        .GetResult();

      var requestTimeMs = requestTimer.Elapsed.TotalMinutes;

      // Publish useful information from the request
      var responseBody = httpResponse.GetBodyAsString();

      context.Publish("response.status", httpResponse.StatusCode.ToString("G"));
      context.Publish("response.status_code", (int)httpResponse.StatusCode);
      context.Publish("response.content", responseBody.Trim());
      context.Publish("response.content_length", responseBody.Length);
      context.Publish("response.execution_time", requestTimeMs);

      foreach (var (header, value) in httpResponse.GetHeaders())
      {
        context.Publish($"response.headers.{header}", value);
      }

      return true;
    }
  }
}
