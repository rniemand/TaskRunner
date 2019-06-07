using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TaskRunner.Core.Abstractions;
using TaskRunner.Core.Builders;
using TaskRunner.Core.Logging;
using TaskRunner.Core.Services;
using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Enums;
using TaskRunner.Shared.Interfaces.Abstractions;
using TaskRunner.Shared.Interfaces.Builders;
using TaskRunner.Shared.Interfaces.Logging;
using TaskRunner.Shared.Interfaces.Services;
using TaskRunner.Shared.Interfaces.Steps;
using TaskRunner.Steps.Console;
using TaskRunner.Steps.Http;

namespace TaskRunner
{
  class Program
  {
    private static IServiceProvider _serviceProvider;

    // ReSharper disable once ArrangeTypeMemberModifiers
    // ReSharper disable once UnusedParameter.Local
    static void Main(string[] args)
    {
      RegisterServices();


      // Hack:  for now - need "IConfigService" to configure secrets and tasks
      //        This will be resolved when we make use of the TaskService to resolve and run tasks
      var configService = _serviceProvider.GetService<IConfigService>();
      Console.WriteLine($"Using config file: {configService.ConfigFilePath}");

      // Compile and run the development test task
      var taskRunner = _serviceProvider.GetService<ITaskRunnerService>();
      var testTask = GetDnsUpdateTask();
      taskRunner.RunTask(testTask);


      DisposeServices();
      Console.ReadLine();
    }



    // Initial development support methods
    private static RunnerTask GetDnsUpdateTask()
    {
      return new RunnerTask
      {
        Enabled = true,
        Name = "DNS Update Task",
        Frequency = TaskInterval.Seconds,
        FrequencyArgs = "5",
        Steps = new[]
        {
          new RunnerStep
          {
            StepName = "log_start",
            Step = "Console.Log",
            Inputs = new Dictionary<string, string>
            {
              {"Severity", "warn"},
              {"Message", "Attempting to do something different!"}
            }
          },
          new RunnerStep
          {
            StepName = "update_dns",
            Step = "Http.Get",
            Inputs = new Dictionary<string, string>
            {
              {"Url", "{!FreeDns.NAS}"}
            }
          },
          new RunnerStep
          {
            StepName = "log_result",
            Step = "Console.Log",
            Inputs = new Dictionary<string, string>
            {
              {"Message", "Http.Get completed with a '{@update_dns.response.status_code}' response code!"}
            }
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
