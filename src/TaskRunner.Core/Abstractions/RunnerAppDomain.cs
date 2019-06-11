using System;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.Core.Abstractions
{
  public class RunnerAppDomain : IAppDomain
  {
    public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
