using System.Collections.Generic;
using System.IO;

namespace TaskRunner.Shared.Abstractions
{
  public interface IDirectoryInfo
  {
    List<IFileInfo> GetFiles(string searchPattern, SearchOption searchOption);
  }
}
