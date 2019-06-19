using TaskRunner.Shared.Configuration;

namespace TaskRunner.Shared.Services
{
  public interface ISchedulerService
  {
    void ScheduleNextRun(TaskConfig task);
  }
}
