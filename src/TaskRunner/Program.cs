using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TaskBuilder.BaseSteps.Core;
using TaskBuilder.BaseSteps.Directory;
using TaskBuilder.BaseSteps.File;
using TaskBuilder.BaseSteps.Http;
using TaskBuilder.BaseSteps.Json;
using TaskBuilder.BaseSteps.Zip;
using TaskBuilder.Common.Abstractions;
using TaskBuilder.Common.Abstractions.Interfaces;
using TaskBuilder.Common.Builders;
using TaskBuilder.Common.Builders.Interfaces;
using TaskBuilder.Common.Logging;
using TaskBuilder.Common.Logging.Interfaces;
using TaskBuilder.Common.Services;
using TaskBuilder.Common.Services.Interfaces;
using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Enums;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder
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
        .AddSingleton<IAppDomain, AppDomainAbstraction>()
        .AddSingleton<IJsonHelper, JsonHelper>()
        .AddSingleton<IFile, FileAbstraction>()
        .AddSingleton<IDirectory, DirectoryAbstraction>()
        .AddSingleton<IConsole, ConsoleAbstraction>();

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
