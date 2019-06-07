using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Interfaces.Services
{
  public interface ITasksService
  {
    void Reconfigure(TaskRunnerConfig baseConfig);
  }
}
