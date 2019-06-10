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
using TaskRunner.Validators.Core;

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
      taskRunner.RunTask(GetMysqlDumpTask());


      DisposeServices();
      Console.ReadLine();
    }



    // Initial development support methods
    private static TaskConfig GetDnsUpdateTask()
    {
      return new TaskConfig
      {
        Enabled = true,
        Name = "DNS Update Task",
        Frequency = TaskInterval.Seconds,
        FrequencyArgs = "5",
        Steps = new[]
        {
          // Id = 0 (Test log)
          new StepConfig
          {
            Name = "log_start",
            Step = "Console.Log",
            Inputs = new Dictionary<string, string>
            {
              {"Severity", "warn"},
              {"Message", "Attempting to do something different!"}
            }
          },
          // Id = 1 (Update DNS entry)
          new StepConfig
          {
            Name = "update_dns",
            Step = "Http.Get",
            Inputs = new Dictionary<string, string>
            {
              {"Url", "{!FreeDns.NAS}"}
            },
            Validators = new List<StepValidatorConfig>
            {
              new StepValidatorConfig
              {
                Enabled = true,
                Validator = "Property.Contains",
                Arguments = new Dictionary<string, string>
                {
                  {"Property", "response.content"},
                  {"Contains", "has not changed"}
                }
              }
            }
          },
          // Id = 2 (Log response code)
          new StepConfig
          {
            Name = "log_result",
            Step = "Console.Log",
            Inputs = new Dictionary<string, string>
            {
              {"Message", "Http.Get completed with a '{@update_dns.response.status_code}' response code!"}
            }
          },
          // Id = 3 (Test @id accessing)
          new StepConfig
          {
            Name = "log_result",
            Step = "Console.Log",
            Inputs = new Dictionary<string, string>
            {
              {"Message", "Response was: '{@1.response.content}'"}
            }
          }
        }
      };
    }

    private static TaskConfig GetMysqlDumpTask()
    {
      // mysqldump --databases Confluence -u richardn -p > dump.sql
      //   https://dev.mysql.com/downloads/mysql/
      //    -> Windows (x86, 64-bit), ZIP Archive
      //    -> Open the ZIP archive and go to "bin" folder
      //    -> extract MYSQLDUMP.EXE where you want
      //    -> https://dev.mysql.com/doc/refman/5.5/en/mysqldump.html
      //    -> VIP: https://unix.stackexchange.com/questions/227648/mysqldump-via-crontab-pass-password-hashed-password-file-so-i-can-use-via-c

      return new TaskConfig
      {
        Name = "Backup Clean-Home DB",
        Enabled = true,
        Frequency = TaskInterval.Days,
        FrequencyArgs = "1",
        Steps = new[]
        {
          new StepConfig
          {
            Name = "Log Hi",
            Step = "Console.Log",
            Inputs = new Dictionary<string, string>
            {
              {"Severity", "Info"},
              {"Message", "Starting backup of CleanHome DB"}
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
        .AddSingleton<ITaskStep, ConsoleLogger>()
        .AddSingleton<ITaskStep, HttpGet>();

      // Validators
      collection
        .AddSingleton<IStepValidator, PropertyContains>();

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
