﻿using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.Directory
{
  public class DirectoryCreate : ITaskBuilderStep
  {
    // TODO: [DOCS] (DirectoryCreate) Document step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public DirectoryCreate()
    {
      Name = "Directory.Create";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (DirectoryCreate) Complete step

      return true;
    }
  }
}
