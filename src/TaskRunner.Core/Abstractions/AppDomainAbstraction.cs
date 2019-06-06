using System;
using TaskRunner.Core.Abstractions.Interfaces;

namespace TaskRunner.Core.Abstractions
{
  public class AppDomainAbstraction : IAppDomain
  {
    public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
