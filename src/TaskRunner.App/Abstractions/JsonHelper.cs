using Newtonsoft.Json;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.App.Abstractions
{
  public class JsonHelper : IJsonHelper
  {
    public string SerializeObject(object value)
    {
      return SerializeObject(value, Formatting.None);
    }

    public string SerializeObject(object value, Formatting formatting)
    {
      return JsonConvert.SerializeObject(value, formatting);
    }

    public T DeserializeObject<T>(string value)
    {
      return JsonConvert.DeserializeObject<T>(value);
    }
  }
}
