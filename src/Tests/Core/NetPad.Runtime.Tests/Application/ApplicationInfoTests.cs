using NetPad.Application;
using Xunit;

namespace NetPad.Runtime.Tests.Application;

public class ApplicationInfoTests
{
    [Theory]
    [InlineData(ApplicationType.Electron)]
    public void SetsApplicationTypeCorrectly(ApplicationType applicationType)
    {
        var appInfo = new ApplicationInfo(applicationType);

        Assert.Equal(applicationType, appInfo.Type);
    }
}
