﻿using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Zip
{
  public class ZipAddFiles : ITaskBuilderStep
  {
    // TODO: [DOCS] (ZipAddFiles) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public ZipAddFiles()
    {
      Name = "Zip.AddFiles";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (ZipAddFiles) Complete step

      return true;
    }
  }
}
