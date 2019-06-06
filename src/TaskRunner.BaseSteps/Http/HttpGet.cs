using System.Diagnostics;
using System.Net.Http;
using TaskBuilder.Common.Extensions;
using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Http
{
  public class HttpGet : ITaskBuilderStep
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

    public string Name { get; }

    public HttpGet()
    {
      Name = "Http.Get";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [TESTS] (HttpGet) Add tests

      var url = context.GetArgument("Url");

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

      context.AppendOutputValue("response.status", httpResponse.StatusCode.ToString("G"));
      context.AppendOutputValue("response.status_code", (int)httpResponse.StatusCode);
      context.AppendOutputValue("response.content", responseBody.Trim());
      context.AppendOutputValue("response.content_length", responseBody.Length);
      context.AppendOutputValue("response.execution_time", requestTimeMs);

      foreach (var (header, value) in httpResponse.GetHeadersDictionary())
      {
        context.AppendOutputValue($"response.headers.{header}", value);
      }

      return true;
    }
  }
}
