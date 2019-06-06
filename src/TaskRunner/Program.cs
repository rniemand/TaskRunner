using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TaskRunner.Core.Abstractions;
using TaskRunner.Core.Abstractions.Interfaces;
using TaskRunner.Core.Builders;
using TaskRunner.Core.Builders.Interfaces;
using TaskRunner.Core.Logging;
using TaskRunner.Core.Logging.Interfaces;
using TaskRunner.Core.Services;
using TaskRunner.Core.Services.Interfaces;
using TaskRunner.Core.Tasks;
using TaskRunner.Core.Tasks.Enums;
using TaskRunner.Core.Tasks.Interfaces;
using TaskRunner.Steps.Console;
using TaskRunner.Steps.Directory;
using TaskRunner.Steps.File;
using TaskRunner.Steps.Http;
using TaskRunner.Steps.Json;
using TaskRunner.Steps.Win;
using TaskRunner.Steps.Zip;

namespace TaskRunner
{
  class Program
  {
    private static IServiceProvider _serviceProvider;

    static void Main(string[] args)
    {
      RegisterServices();

      var taskRunner = _serviceProvider.GetService<ITaskRunnerService>();


      taskRunner.RunTask(GetDemoTask());


      DisposeServices();
      Console.ReadLine();
    }

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
        .AddSingleton<ISchedulerService, SchedulerService>()
        .AddSingleton<IStepContextService, StepContextService>();

      // Misc
      collection
        .AddScoped<IAppLogger, AppLogger>()
        .AddSingleton<IPathBuilder, PathBuilder>();

      // Steps
      collection
        .AddSingleton<ITaskBuilderStep, ConsoleLogger>()
        .AddSingleton<ITaskBuilderStep, FileLogger>()
        .AddSingleton<ITaskBuilderStep, FileCopy>()
        .AddSingleton<ITaskBuilderStep, FileDelete>()
        .AddSingleton<ITaskBuilderStep, FileExists>()
        .AddSingleton<ITaskBuilderStep, HttpPost>()
        .AddSingleton<ITaskBuilderStep, HttpGet>()
        .AddSingleton<ITaskBuilderStep, JsonSaveToFile>()
        .AddSingleton<ITaskBuilderStep, JsonLoadFromFile>()
        .AddSingleton<ITaskBuilderStep, ZipCreate>()
        .AddSingleton<ITaskBuilderStep, ZipAddFile>()
        .AddSingleton<ITaskBuilderStep, ZipAddFiles>()
        .AddSingleton<ITaskBuilderStep, ZipRemoveFile>()
        .AddSingleton<ITaskBuilderStep, ZipRemoveFiles>()
        .AddSingleton<ITaskBuilderStep, DirectoryExists>()
        .AddSingleton<ITaskBuilderStep, DirectoryCreate>()
        .AddSingleton<ITaskBuilderStep, DirectoryDelete>()
        .AddSingleton<ITaskBuilderStep, WinShell>();

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

    private static TaskBuilderTask GetDemoTask()
    {
      return new TaskBuilderTask
      {
        Enabled = true,
        Name = "Console Logger Test Task",
        Frequency = TaskInterval.Seconds,
        FrequencyArgs = "5",
        Steps = new[]
        {
          new TaskBuilderStep
          {
            Enabled = true,
            Arguments = new Dictionary<string, string>
            {
              {"Severity", "Debug"},
              {"Message", "Attempting to do something different!"}
            },
            FailAction = StepFailAction.Continue,
            Step = "Console.Log",
            StepName = "console1"
          },
          new TaskBuilderStep
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
          new TaskBuilderStep
          {
            Enabled = true,
            Arguments = new Dictionary<string, string>
            {
              {"Message", "Http.Get completed with a '{@update_nas.response.status_code}' response code!"}
            },
            FailAction = StepFailAction.Continue,
            Step = "Console.Log",
            StepName = "log_output"
          }
        }
      };
    }
  }
}
