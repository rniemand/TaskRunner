﻿using TaskBuilder.Common.Tasks;
using TaskBuilder.Common.Tasks.Interfaces;

namespace TaskBuilder.BaseSteps.File
{
  public class FileCopy : ITaskBuilderStep
  {
    // TODO: [DOCS] (FileCopy) Document this step
    // TODO: [COMPLETE] (ConsoleLog) Add logic to validate required arguments

    public string Name { get; }

    public FileCopy()
    {
      Name = "File.Copy";
    }

    public bool Execute(StepContext context)
    {
      // TODO: [COMPLETE] (FileCopy) Complete this

      return true;
    }
  }
}
