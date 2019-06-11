using TaskRunner.Shared.Interfaces.Logging;

namespace TaskRunner.Shared.Providers
{
  public abstract class BaseProvider
  {
    public string Name { get; }
    public IAppLogger Logger { get; set; }

    protected BaseProvider(IAppLogger logger, string name)
    {
      Logger = logger;
      Name = name;
    }
  }
}
