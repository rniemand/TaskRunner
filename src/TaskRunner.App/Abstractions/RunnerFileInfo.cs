using System;
using System.IO;
using TaskRunner.Shared.Abstractions;

namespace TaskRunner.App.Abstractions
{
  public class RunnerFileInfo : IFileInfo
  {
    public string FullName => _fileInfo.FullName;
    public DateTime CreationTime => _fileInfo.CreationTime;
    public DateTime LastWriteTime => _fileInfo.LastWriteTime;
    public DateTime LastWriteTimeUtc => _fileInfo.LastWriteTimeUtc;
    public DateTime CreationTimeUtc => _fileInfo.CreationTimeUtc;

    private readonly FileInfo _fileInfo;

    public RunnerFileInfo(string path)
    {
      _fileInfo = new FileInfo(path);
    }
  }
}
