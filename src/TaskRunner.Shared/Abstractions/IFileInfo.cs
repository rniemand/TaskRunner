using System;

namespace TaskRunner.Shared.Abstractions
{
  public interface IFileInfo
  {
    string FullName { get; }
    DateTime CreationTime { get; }
    DateTime LastWriteTime { get; }
    DateTime LastWriteTimeUtc { get; }
    DateTime CreationTimeUtc { get; }
  }
}