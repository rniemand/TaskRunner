﻿using System;
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
        Name = "Console Logger Test Task",
        Frequency = TaskInterval.Seconds,
        FrequencyArgs = "5",
        Steps = new[]
        {
          new RunnerStep
          {
            Enabled = true,
            Arguments = new Dictionary<string, string>
            {
              {"RunnerSeverity", "Debug"},
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
            StepName = "log_output"
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
        .AddSingleton<ISchedulerService, SchedulerService>()
        .AddSingleton<IStepContextService, StepContextService>();

      // Misc
      collection
        .AddScoped<IAppLogger, AppLogger>()
        .AddSingleton<IPathBuilder, PathBuilder>();

      // Steps
      collection
        .AddSingleton<IRunnerStep, ConsoleLogger>()
        .AddSingleton<IRunnerStep, FileLogger>()
        .AddSingleton<IRunnerStep, FileCopy>()
        .AddSingleton<IRunnerStep, FileDelete>()
        .AddSingleton<IRunnerStep, FileExists>()
        .AddSingleton<IRunnerStep, HttpPost>()
        .AddSingleton<IRunnerStep, HttpGet>()
        .AddSingleton<IRunnerStep, JsonSaveToFile>()
        .AddSingleton<IRunnerStep, JsonLoadFromFile>()
        .AddSingleton<IRunnerStep, ZipCreate>()
        .AddSingleton<IRunnerStep, ZipAddFile>()
        .AddSingleton<IRunnerStep, ZipAddFiles>()
        .AddSingleton<IRunnerStep, ZipRemoveFile>()
        .AddSingleton<IRunnerStep, ZipRemoveFiles>()
        .AddSingleton<IRunnerStep, DirectoryExists>()
        .AddSingleton<IRunnerStep, DirectoryCreate>()
        .AddSingleton<IRunnerStep, DirectoryDelete>()
        .AddSingleton<IRunnerStep, WinShell>();

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
