using System;
using TaskRunner.Shared.Interfaces.Abstractions;

namespace TaskRunner.Core.Abstractions
{
  public class RunnerAppDomain : IAppDomain
  {
    public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
