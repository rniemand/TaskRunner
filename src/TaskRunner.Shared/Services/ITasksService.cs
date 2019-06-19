using System.Collections.Generic;
using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Services
{
  public interface ITasksService
  {
    void Reconfigure(TaskRunnerConfig baseConfig);
    List<TaskConfig> GetRunnableTasks();
    void TaskRanSuccessfully(TaskConfig task);
  }
}
