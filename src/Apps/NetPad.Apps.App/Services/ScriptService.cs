using MediatR;
using NetPad.Apps.CQs;
using NetPad.Apps.UiInterop;
using NetPad.Exceptions;
using NetPad.Sessions;

namespace NetPad.Services;

public class ScriptService
{
    private readonly ISession _session;
    private readonly IUiDialogService _uiDialogService;
    private readonly IMediator _mediator;

    public ScriptService(ISession session, IUiDialogService uiDialogService, IMediator mediator)
    {
        _session = session;
        _uiDialogService = uiDialogService;
        _mediator = mediator;
    }

    public async Task CloseScriptAsync(Guid scriptId)
    {
        var scriptEnvironment = _session.Get(scriptId) ?? throw new ScriptNotFoundException(scriptId);
        var script = scriptEnvironment.Script;

        bool shouldAskUserToSave = script.IsDirty;
        if (script.IsNew && string.IsNullOrEmpty(script.Code))
        {
            shouldAskUserToSave = false;
        }

        if (shouldAskUserToSave)
        {
            var response = await _uiDialogService.AskUserIfTheyWantToSave(script);
            if (response == YesNoCancel.Cancel)
            {
                return;
            }

            if (response == YesNoCancel.Yes)
            {
                bool saved = await SaveScriptAsync(scriptId);
                if (!saved)
                {
                    return;
                }
            }
        }

        await _mediator.Send(new CloseScriptCommand(scriptId));
    }

    public async Task<bool> SaveScriptAsync(Guid scriptId)
    {
        var scriptEnvironment = _session.Get(scriptId) ?? throw new ScriptNotFoundException(scriptId);
        var script = scriptEnvironment.Script;

        if (script.IsNew)
        {
            var path = await _uiDialogService.AskUserForSaveLocation(script);

            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            script.SetPath(path);
        }

        await _mediator.Send(new SaveScriptCommand(script));

        return true;
    }
}
