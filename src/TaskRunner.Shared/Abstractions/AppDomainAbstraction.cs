using System;
using TaskRunner.Shared.Abstractions.Interfaces;

namespace TaskRunner.Shared.Abstractions
{
  public class AppDomainAbstraction : IAppDomain
  {
    public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
