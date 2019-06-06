using Newtonsoft.Json;

namespace TaskRunner.Core.Abstractions.Interfaces
{
  public interface IJsonHelper
  {
    string SerializeObject(object value);
    string SerializeObject(object value, Formatting formatting);
    T DeserializeObject<T>(string value);
  }
}
