using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Services
{
  public interface ITasksService
  {
    void Reconfigure(TaskRunnerConfig baseConfig);
  }
}
