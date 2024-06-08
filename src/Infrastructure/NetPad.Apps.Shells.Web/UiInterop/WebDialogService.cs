using NetPad.Application;
using NetPad.Apps.CQs;
using NetPad.Apps.UiInterop;
using NetPad.Scripts;

namespace NetPad.Apps.Shells.Web.UiInterop;

public class WebDialogService : IUiDialogService
{
    private readonly IIpcService _ipcService;

    public WebDialogService(IIpcService ipcService)
    {
        _ipcService = ipcService;
    }

    public async Task<YesNoCancel> AskUserIfTheyWantToSave(Script script)
    {
        return await _ipcService.SendAndReceiveAsync(new ConfirmSaveCommand(script));
    }

    public async Task<string?> AskUserForSaveLocation(Script script)
    {
        var newName = await _ipcService.SendAndReceiveAsync(new RequestNewScriptNameCommand(script.Name));

        if (newName == null)
            return null;

        return $"/{newName}{Script.STANDARD_EXTENSION}";
    }

    public async Task AlertUserAboutMissingDependencies(AppDependencyCheckResult dependencyCheckResult)
    {
        await _ipcService.SendAsync(new AlertUserAboutMissingAppDependencies(dependencyCheckResult));
    }
}
