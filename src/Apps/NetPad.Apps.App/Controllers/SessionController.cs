using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetPad.Apps.CQs;
using NetPad.Exceptions;
using NetPad.Scripts;
using NetPad.Services;

namespace NetPad.Controllers;

[ApiController]
[Route("session")]
public class SessionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("environments/{scriptId:guid}")]
    public async Task<ScriptEnvironment> GetEnvironment(Guid scriptId)
    {
        return await _mediator.Send(new GetOpenedScriptEnvironmentQuery(scriptId))
               ?? throw new EnvironmentNotFoundException(scriptId);
    }

    [HttpGet("environments")]
    public async Task<IEnumerable<ScriptEnvironment>> GetEnvironments()
    {
        return await _mediator.Send(new GetOpenedScriptEnvironmentsQuery());
    }

    [HttpPatch("open/path")]
    public async Task OpenByPath([FromBody] string scriptPath)
    {
        await _mediator.Send(new OpenScriptCommand(scriptPath));
    }

    [HttpPatch("{scriptId:guid}/close")]
    public async Task Close(Guid scriptId, [FromServices] ScriptService scriptService)
    {
        await scriptService.CloseScriptAsync(scriptId);
    }

    [HttpGet("active")]
    public async Task<Guid?> GetActive()
    {
        var active = await _mediator.Send(new GetActiveScriptEnvironmentQuery());
        return active?.Script.Id;
    }

    [HttpPatch("{scriptId:guid}/activate")]
    public async Task Activate(Guid scriptId)
    {
        await _mediator.Send(new ActivateScriptCommand(scriptId));
    }

    [HttpPatch("activate-last-active")]
    public async Task ActivateLastActive()
    {
        await _mediator.Send(new ActivateLastActiveScriptCommand());
    }
}
