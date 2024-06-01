using Microsoft.AspNetCore.Mvc;
using NetPad.Apps.App.Common.UiInterop;

namespace NetPad.Controllers;

[ApiController]
[Route("window")]
public class WindowController : ControllerBase
{
    private readonly IUiWindowService _uiWindowService;

    public WindowController(IUiWindowService uiWindowService)
    {
        _uiWindowService = uiWindowService;
    }

    [HttpPatch("open-output-window")]
    public async Task OpenOutputWindow()
    {
        await _uiWindowService.OpenOutputWindowAsync();
    }

    [HttpPatch("open-code-window")]
    public async Task OpenCodeWindow()
    {
        await _uiWindowService.OpenCodeWindowAsync();
    }
}
