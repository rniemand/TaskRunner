using TaskRunner.Shared.Logging;
using TaskRunner.Shared.Providers;

namespace TaskRunner.Providers.Core
{
  public class DateProvider : BaseProvider
  {
    public DateProvider(IAppLogger logger)
      : base(logger, "DateProvider")
    {

    }

    public override void Run()
    {
      
    }
  }
}
