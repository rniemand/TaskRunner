using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Services
{
  public interface ITaskRunnerService
  {
    // TODO: [REMOVE] (ITaskRunnerService) Remove this once initial dev testing has been completed
    void RunTask(TaskConfig task);
  }
}
