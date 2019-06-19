using System;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.App.Abstractions
{
  public class RunnerDateTime : IDateTime
  {
    public DateTime Now => DateTime.Now;
  }
}
