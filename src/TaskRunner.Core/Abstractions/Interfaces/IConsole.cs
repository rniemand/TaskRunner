using System;

namespace TaskRunner.Core.Abstractions.Interfaces
{
  public interface IConsole
  {
    ConsoleColor ForegroundColor { get; set; }
    ConsoleColor BackgroundColor { get; set; }

    void WriteLine(string value);
    void ResetColor();
  }
}
