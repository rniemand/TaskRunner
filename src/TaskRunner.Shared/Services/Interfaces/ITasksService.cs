using TaskBuilder.Common.Configuration;

namespace TaskBuilder.Common.Services.Interfaces
{
  public interface ITasksService
  {
    void Reconfigure(TaskBuilderConfig baseConfig);
  }
}
