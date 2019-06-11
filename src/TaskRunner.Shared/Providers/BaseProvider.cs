using System;
using System.Collections.Generic;
using System.Linq;
using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Steps;

namespace TaskRunner.Shared.Providers
{
  public abstract class BaseProvider : IProvider
  {
    public string Name { get; }
    public IAppLogger Logger { get; set; }

    private readonly List<ProviderInput> _inputs;

    protected BaseProvider(IAppLogger logger, string name)
    {
      Logger = logger;
      Name = name;

      _inputs = new List<ProviderInput>
      {
        // TODO: [DOCS] (BaseProvider) Document default value

        new ProviderInput
        {
          Name = "ProvideAs",
          Required = false,
          DefaultValue = name.Trim()
        }
      };

    }


    // TaskRunnerService methods
    public void Execute(StepContext context, Dictionary<string, string> inputs)
    {
      Run(context, GetInput(inputs, "ProvideAs"), inputs);
    }


    // Implementing class methods
    public virtual void Run(StepContext context, string provideAs, Dictionary<string, string> inputs)
    {
      throw new NotImplementedException();
    }

    protected void RegisterInput(string name, bool required = true, string defaultValue = "")
    {
      // TODO: [TESTS] (BaseProvider) Add tests

      _inputs.Add(new ProviderInput
      {
        Name = name,
        Required = required,
        DefaultValue = defaultValue
      });
    }

    protected string GetInput(Dictionary<string, string> inputs, string name)
    {
      // TODO: [TESTS] (BaseProvider) Add tests

      var input = _inputs.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

      if (!inputs.ContainsKey(input.Name))
        return input.DefaultValue;

      return inputs[input.Name];
    }
  }
}
