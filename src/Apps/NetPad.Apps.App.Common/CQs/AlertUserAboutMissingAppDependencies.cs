using NetPad.Application;

namespace NetPad.Apps.App.Common.CQs;

public class AlertUserAboutMissingAppDependencies : Command
{
    public AlertUserAboutMissingAppDependencies(AppDependencyCheckResult dependencyCheckResult)
    {
        DependencyCheckResult = dependencyCheckResult;
    }

    public AppDependencyCheckResult DependencyCheckResult { get; }
}
