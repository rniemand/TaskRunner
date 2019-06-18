using TaskRunner.Shared.Enums;

namespace TaskRunner.Shared.Abstractions
{
  public interface IEnvironment
  {
    HostEnvironment HostEnvironment { get; }
  }
}
