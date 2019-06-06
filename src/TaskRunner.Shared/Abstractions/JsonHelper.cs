using Newtonsoft.Json;
using TaskBuilder.Common.Abstractions.Interfaces;

namespace TaskBuilder.Common.Abstractions
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
