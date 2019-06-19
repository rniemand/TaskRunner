using System;

namespace TaskRunner.Shared.Abstractions
{
  public interface IDateTime
  {
    DateTime Now { get; }
  }
}
