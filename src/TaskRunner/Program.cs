using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TaskRunner.Core.Abstractions;
using TaskRunner.Core.Abstractions.Interfaces;
using TaskRunner.Core.Builders;
using TaskRunner.Core.Builders.Interfaces;
using TaskRunner.Core.Configuration;
using TaskRunner.Core.Enums;
using TaskRunner.Core.Logging;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Services;
using TaskRunner.Core.Services.Interfaces;
using TaskRunner.Core.Steps.Interfaces;
using TaskRunner.Steps.Console;
using TaskRunner.Steps.Http;

namespace TaskRunner
{
  class Program
  {
    private static IServiceProvider _serviceProvider;

    static void Main(string[] args)
    {
      RegisterServices();


      // Hack:  for now - need "IConfigService" to configure secrets and tasks
      //        This will be resolved when we make use of the TaskService to resolve and run tasks
      var configService = _serviceProvider.GetService<IConfigService>();
      Console.WriteLine($"Using config file: {configService.ConfigFilePath}");

      // Compile and run the development test task
      var taskRunner = _serviceProvider.GetService<ITaskRunnerService>();
      var testTask = GetDemoTask();
      taskRunner.RunTask(testTask);


      DisposeServices();
      Console.ReadLine();
    }



    // Initial development support methods
    private static RunnerTask GetDemoTask()
    {
      return new RunnerTask
      {
        Enabled = true,
        Name = "Development super task 1",
        Frequency = TaskInterval.Seconds,
        FrequencyArgs = "5",
        Steps = new[]
        {
          new RunnerStep
          {
            Enabled = true,
            Arguments = new Dictionary<string, string>
            {
              {"Severity", "warn"},
              {"Message", "Attempting to do something different!"}
            },
            FailAction = StepFailAction.Continue,
            Step = "Console.Log",
            StepName = "console1"
          },
          new RunnerStep
          {
            Enabled = true,
            Arguments = new Dictionary<string, string>
            {
              {"Url", "{!FreeDns.NAS}"}
            },
            FailAction = StepFailAction.Continue,
            Step = "Http.Get",
            StepName = "update_nas"
          },
          new RunnerStep
          {
            Enabled = true,
            Arguments = new Dictionary<string, string>
            {
              {"Message", "Http.Get completed with a '{@update_nas.response.status_code}' response code!"}
            },
            FailAction = StepFailAction.Continue,
            Step = "Console.Log",
            StepName = "log_result"
          }
        }
      };
    }


    // Service management
    private static void RegisterServices()
    {
      var collection = new ServiceCollection();

      // Abstractions
      collection
        .AddSingleton<IAppDomain, RunnerAppDomain>()
        .AddSingleton<IJsonHelper, JsonHelper>()
        .AddSingleton<IFile, RunnerFile>()
        .AddSingleton<IDirectory, RunnerDirectory>()
        .AddSingleton<IConsole, RunnerConsole>();

      // Services
      collection
        .AddSingleton<IConfigService, ConfigService>()
        .AddSingleton<ISecretsService, SecretsService>()
        .AddSingleton<ITasksService, TasksService>()
        .AddSingleton<ITaskRunnerService, TaskRunnerService>()
        .AddSingleton<ISchedulerService, SchedulerService>();

      // Misc
      collection
        .AddScoped<IAppLogger, AppLogger>()
        .AddSingleton<IPathBuilder, PathBuilder>();

      // Steps
      collection
        .AddSingleton<IRunnerStep, ConsoleLogger>()
        .AddSingleton<IRunnerStep, HttpGet>();

      _serviceProvider = collection.BuildServiceProvider();
    }

    private static void DisposeServices()
    {
      // ReSharper disable once ConvertIfStatementToSwitchStatement
      if (_serviceProvider == null)
        return;

      // ReSharper disable once UseNullPropagation
      // ReSharper disable once MergeCastWithTypeCheck
      if (_serviceProvider is IDisposable)
        ((IDisposable)_serviceProvider).Dispose();
    }
  }
}
