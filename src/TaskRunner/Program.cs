﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Abstractions.Interfaces;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Builders.Interfaces;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Logging.Interfaces;
using TaskRunner.Shared.Services;
using TaskRunner.Shared.Services.Interfaces;
using TaskRunner.Shared.Tasks;
using TaskRunner.Shared.Tasks.Enums;
using TaskRunner.Shared.Tasks.Interfaces;
using TaskRunner.Steps.Core;
using TaskRunner.Steps.Directory;
using TaskRunner.Steps.File;
using TaskRunner.Steps.Http;
using TaskRunner.Steps.Json;
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
