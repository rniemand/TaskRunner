using System;

namespace TaskRunner.Shared.Interfaces.Abstractions
{
  public interface IConsole
  {
    ConsoleColor ForegroundColor { get; set; }
    ConsoleColor BackgroundColor { get; set; }

    void WriteLine(string value);
    void ResetColor();
  }
}
