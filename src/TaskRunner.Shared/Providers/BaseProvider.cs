using System;
using System.Collections.Generic;
using TaskRunner.Shared.Logging;

namespace TaskRunner.Shared.Providers
{
  public abstract class BaseProvider
  {
    public string Name { get; }
    public IAppLogger Logger { get; set; }

    private List<ProviderInput> _inputs;

    protected BaseProvider(IAppLogger logger, string name)
    {
      Logger = logger;
      Name = name;

      _inputs = new List<ProviderInput>();
    }

    public void Execute()
    {

    }

    // Methods used by providers
    public virtual void Run()
    {
      throw new NotImplementedException();
    }

    protected void RegisterInput(string name, bool required = true, string defaultValue = "")
    {

    }
  }
}
