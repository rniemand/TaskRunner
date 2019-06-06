using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Services.Interfaces
{
  public interface ITasksService
  {
    void Reconfigure(TaskBuilderConfig baseConfig);
  }
}
