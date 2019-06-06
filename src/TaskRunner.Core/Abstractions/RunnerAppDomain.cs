using System;
using TaskRunner.Core.Abstractions.Interfaces;

namespace TaskRunner.Core.Abstractions
{
  public class RunnerAppDomain : IAppDomain
  {
    public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
