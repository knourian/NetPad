using ElectronNET.API;
using ElectronNET.API.Entities;
using NetPad.Application;
using NetPad.Apps.CQs;
using NetPad.Apps.UiInterop;
using NetPad.Configuration;
using NetPad.Scripts;

namespace NetPad.Apps.Shells.Electron.UiInterop;

public class ElectronDialogService(IIpcService ipcService, Settings settings) : IUiDialogService
{
    public async Task<YesNoCancel> AskUserIfTheyWantToSave(Script script)
    {
        var result = await ElectronNET.API.Electron.Dialog.ShowMessageBoxAsync(ElectronUtil.MainWindow,
            new MessageBoxOptions($"You have unsaved changes. Do you want to save '{script.Name}'?")
            {
                Title = "Save?",
                Buttons = new[] { "Yes", "No", "Cancel" },
                Type = MessageBoxType.question
            });

        return (YesNoCancel)result.Response;
    }

    public async Task<string?> AskUserForSaveLocation(Script script)
    {
        var path = await ElectronNET.API.Electron.Dialog.ShowSaveDialogAsync(ElectronUtil.MainWindow, new SaveDialogOptions
        {
            Title = "Save Script",
            Message = "Where do you want to save this script?",
            NameFieldLabel = script.Name,
            Filters = new[] { new FileFilter { Name = "NetPad Script", Extensions = new[] { Script.STANDARD_EXTENSION_WO_DOT } } },
            DefaultPath = Path.Combine(settings.ScriptsDirectoryPath, script.Name + Script.STANDARD_EXTENSION)
        });

        if (path == null || string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(path)))
            return null;

        path = path.TrimEnd(Path.PathSeparator);

        if (!path.EndsWith(Script.STANDARD_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
            path += Script.STANDARD_EXTENSION;

        return path;
    }

    public async Task AlertUserAboutMissingDependencies(AppDependencyCheckResult dependencyCheckResult)
    {
        await ipcService.SendAsync(new AlertUserAboutMissingAppDependencies(dependencyCheckResult));
    }
}
