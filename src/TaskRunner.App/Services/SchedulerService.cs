using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Enums;
using TaskRunner.Shared.Extensions;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Services;

namespace TaskRunner.App.Services
{
  public class SchedulerService : ISchedulerService
  {
    private readonly IAppLogger _logger;
    private readonly IDateTime _dateTime;

    public SchedulerService(
      IAppLogger logger,
      IDateTime dateTime)
    {
      _logger = logger;
      _dateTime = dateTime;
    }


    // Public methods
    public void ScheduleNextRun(TaskConfig task)
    {
      // TODO: [TESTS] (SchedulerService) Add tests

      // Check to see if this task needs to run at start-up
      if (task.NextRunTime.HasValue == false && task.RunAtStartup)
      {
        task.NextRunTime = _dateTime.Now;
        return;
      }

      // Ensure we have a last run time to work from, if there is none use NOW
      if (task.LastRunTime.HasValue == false)
        task.LastRunTime = _dateTime.Now;

      // Schedule the next task run based on the "Frequency"

      switch (task.Frequency)
      {
        case TaskRunInterval.Seconds: ScheduleBySeconds(task); break;

        case TaskRunInterval.Minutes: ScheduleByMinuets(task); break;

        case TaskRunInterval.Hours: ScheduleByHours(task); break;

        case TaskRunInterval.Days: ScheduleByDays(task); break;

        default:
          _logger.Error("Unknown frequency type: '{type}'", task.Frequency.ToString("G"));
          return;
      }
    }


    // Scheduling methods
    private void ScheduleBySeconds(TaskConfig task)
    {
      // TODO: [TESTS] (SchedulerService) Add tests

      var interval = task.FrequencyArgs.ToInt(-1);

      // If we did not get a satisfactory value from "FrequencyArgs" we will disable the task
      if (interval <= 0)
      {
        _logger.Error(
          "Unable to parse or an invalid value was used for task '{task}' ({taskFile}) FrequencyArgs (got: {value}) " +
          "task has been disabled for now",
          task.Name, task.TaskFilePath, interval);

        task.Enabled = false;
        return;
      }

      // Schedule the next run time for the task
      task.NextRunTime = _dateTime.Now.AddSeconds(interval);

      _logger.Verbose("Scheduled next run time for '{task}' ({file}) for {time}",
        task.Name, task.TaskFilePath, task.NextRunTime.Value.ToString("u"));
    }

    private void ScheduleByMinuets(TaskConfig task)
    {
      // TODO: [TESTS] (SchedulerService) Add tests

      var interval = task.FrequencyArgs.ToInt(-1);

      // If we did not get a satisfactory value from "FrequencyArgs" we will disable the task
      if (interval <= 0)
      {
        _logger.Error(
          "Unable to parse or an invalid value was used for task '{task}' ({taskFile}) FrequencyArgs (got: {value}) " +
          "task has been disabled for now",
          task.Name, task.TaskFilePath, interval);

        task.Enabled = false;
        return;
      }

      // Schedule the next run time for the task
      task.NextRunTime = _dateTime.Now.AddMinutes(interval);

      _logger.Verbose("Scheduled next run time for '{task}' ({file}) for {time}",
        task.Name, task.TaskFilePath, task.NextRunTime.Value.ToString("u"));
    }

    private void ScheduleByHours(TaskConfig task)
    {
      // TODO: [TESTS] (SchedulerService) Add tests

      var interval = task.FrequencyArgs.ToInt(-1);

      // If we did not get a satisfactory value from "FrequencyArgs" we will disable the task
      if (interval <= 0)
      {
        _logger.Error(
          "Unable to parse or an invalid value was used for task '{task}' ({taskFile}) FrequencyArgs (got: {value}) " +
          "task has been disabled for now",
          task.Name, task.TaskFilePath, interval);

        task.Enabled = false;
        return;
      }

      // Schedule the next run time for the task
      task.NextRunTime = _dateTime.Now.AddMinutes(interval);

      _logger.Verbose("Scheduled next run time for '{task}' ({file}) for {time}",
        task.Name, task.TaskFilePath, task.NextRunTime.Value.ToString("u"));
    }

    private void ScheduleByDays(TaskConfig task)
    {
      // TODO: [TESTS] (SchedulerService) Add tests

      var interval = task.FrequencyArgs.ToInt(-1);

      // If we did not get a satisfactory value from "FrequencyArgs" we will disable the task
      if (interval <= 0)
      {
        _logger.Error(
          "Unable to parse or an invalid value was used for task '{task}' ({taskFile}) FrequencyArgs (got: {value}) " +
          "task has been disabled for now",
          task.Name, task.TaskFilePath, interval);

        task.Enabled = false;
        return;
      }

      // Schedule the next run time for the task
      task.NextRunTime = _dateTime.Now.AddDays(interval);

      _logger.Verbose("Scheduled next run time for '{task}' ({file}) for {time}",
        task.Name, task.TaskFilePath, task.NextRunTime.Value.ToString("u"));
    }
  }
}
