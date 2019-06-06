using TaskRunner.Core.Configuration;

namespace TaskRunner.Core.Services.Interfaces
{
  public interface ITasksService
  {
    void Reconfigure(TaskBuilderConfig baseConfig);
  }
}
