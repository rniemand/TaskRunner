using TaskRunner.Core.Tasks;

namespace TaskRunner.Core.Services.Interfaces
{
  public interface ITaskRunnerService
  {
    // TODO: [REMOVE] (ITaskRunnerService) Remove this once initial dev testing has been completed
    void RunTask(TaskBuilderTask task);
  }
}
