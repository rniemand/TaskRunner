using System;
using TaskBuilder.Common.Abstractions.Interfaces;

namespace TaskBuilder.Common.Abstractions
{
  public class AppDomainAbstraction : IAppDomain
  {
    public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
