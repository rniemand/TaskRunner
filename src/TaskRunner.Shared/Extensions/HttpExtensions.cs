using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace TaskBuilder.Common.Extensions
{
  public static class HttpExtensions
  {
    public static Dictionary<string, string> GetHeadersDictionary(this HttpResponseMessage response)
    {
      // TODO: [TESTS] (HttpExtensions) Add tests

      var headers = new Dictionary<string, string>();

      if (response == null)
        return headers;

      foreach (var header in response.Headers)
      {
        var value = header.Value.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(value))
        {
          headers[header.Key] = value;
        }
      }

      return headers;
    }

    public static string GetBodyAsString(this HttpResponseMessage response)
    {
      // TODO: [TESTS] (HttpExtensions) Add tests

      if (response?.Content == null)
        return string.Empty;

      var responseBody = response.Content.ReadAsStringAsync()
        .ConfigureAwait(false)
        .GetAwaiter()
        .GetResult();

      return responseBody;
    }
  }
}
