using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TaskRunner.App.Abstractions;
using TaskRunner.App.Builders;
using TaskRunner.App.Logging;
using TaskRunner.App.Services;
using TaskRunner.Providers.Core;
using TaskRunner.Shared.Abstractions;
using TaskRunner.Shared.Builders;
using TaskRunner.Shared.Configuration;
using TaskRunner.Shared.Enums;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Providers;
using TaskRunner.Shared.Services;
using TaskRunner.Shared.Steps;
using TaskRunner.Shared.Validators;
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

      RunTaskRunner();

      DisposeServices();
      Console.ReadLine();
    }

    private static void RunTaskRunner()
    {
      // Hack:  for now - need "IConfigService" to configure secrets and tasks
      //        This will be resolved when we make use of the TaskService to resolve and run tasks
      var configService = _serviceProvider.GetService<IConfigService>();
      Console.WriteLine($"Using configuration file: {configService.ConfigFilePath}");

      // Compile and run the development test task
      var taskRunner = _serviceProvider.GetService<ITaskRunnerService>();

      // Continuously run registered tasks
      for (; ; )
      {
        taskRunner.Run();
        Thread.Sleep(1000);
      }
    }


    // Initial development support methods
    private static void BuildDnsUpdateTask()
    {
      var updateTask = new TaskConfig
      {
        Enabled = true,
        Name = "DNS Update Task",
        Frequency = TaskRunInterval.Seconds,
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
            Validators = new List<ValidatorConfig>
            {
              new ValidatorConfig
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

      var taskJson = JsonConvert.SerializeObject(updateTask, Formatting.Indented);
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
        Frequency = TaskRunInterval.Days,
        FrequencyArgs = "1",
        Steps = new[]
        {
          new StepConfig
          {
            Name = "Log Hi",
            Step = "Console.Log",
            Providers = new List<ProviderConfig>
            {
              new ProviderConfig("Date")
              {
                Inputs = new Dictionary<string, string>
                {
                  {"Format", "yyyymmdd_hhmmss"}
                }
              },
              new ProviderConfig("UtcDate")
            },
            Inputs = new Dictionary<string, string>
            {
              {"Severity", "Info"},
              {"Message", "The provided date is: {$Date}.sql ({$UtcDate})"},
            }
          }
        }
      };
    }

    private static void BuildHelloWorldTask()
    {
      var task = new TaskConfig
      {
        Name = "Hello World",
        Enabled = true,
        Frequency = TaskRunInterval.Seconds,
        FrequencyArgs = "30",
        RunAtStartup = true,
        Steps = new StepConfig[]
        {
          new StepConfig
          {
            Enabled = true,
            Step = "Console.Log",
            Name = "step_1",
            Inputs = new Dictionary<string, string>
            {
              {"Message", "Hello World!"},
              {"Severity", "Info" }
            }
          }
        }
      };

      var taskJson = JsonConvert.SerializeObject(task, Formatting.Indented);
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
        .AddSingleton<IConsole, RunnerConsole>()
        .AddSingleton<IEnvironment, RunnerEnvironment>()
        .AddSingleton<IDateTime, RunnerDateTime>()

        // Services
        .AddSingleton<IConfigService, ConfigService>()
        .AddSingleton<ISecretsService, SecretsService>()
        .AddSingleton<ITasksService, TasksService>()
        .AddSingleton<ITaskRunnerService, TaskRunnerService>()
        .AddSingleton<ISchedulerService, SchedulerService>()

        // Misc
        .AddSingleton<IAppLogger, AppLogger>()

        // Steps
        .AddSingleton<IStep, ConsoleLogger>()
        .AddSingleton<IStep, HttpGet>()

        // Providers
        .AddSingleton<IProvider, UtcDateProvider>()
        .AddSingleton<IProvider, DateProvider>()

        // Validators
        .AddSingleton<IValidator, PropertyContains>();


      // Register platform specific services and objects
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        RegisterLinuxServices(collection);
      }
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        RegisterWindowsServices(collection);
      }
      else
      {
        // TODO: [EX] (Program) Make use of a better exception type here
        throw new Exception("Unsupported OS - for now...");
      }

      _serviceProvider = collection.BuildServiceProvider();
    }

    private static void RegisterLinuxServices(IServiceCollection collection)
    {
      collection
        .AddSingleton<IPathBuilder, LinuxPathBuilder>();
    }

    private static void RegisterWindowsServices(IServiceCollection collection)
    {
      collection
        .AddSingleton<IPathBuilder, WinPathBuilder>();
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
