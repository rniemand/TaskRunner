using System;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.Core.Abstractions
{
  public class RunnerConsole : IConsole
  {
    public ConsoleColor ForegroundColor
    {
      get => Console.ForegroundColor;
      set => Console.ForegroundColor = value;
    }

    public ConsoleColor BackgroundColor
    {
      get => Console.BackgroundColor;
      set => Console.BackgroundColor = value;
    }

    public void WriteLine(string value)
    {
      Console.WriteLine(value);
    }

    public void ResetColor()
    {
      Console.ResetColor();
    }
  }
}
