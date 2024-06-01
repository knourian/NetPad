using MediatR;
using NetPad.Events;
using NetPad.Exceptions;
using NetPad.Scripts;
using NetPad.Scripts.Events;
using NetPad.Sessions;

namespace NetPad.Apps.App.Common.CQs;

public class CloseScriptCommand : Command
{
    public CloseScriptCommand(Guid scriptId)
    {
        ScriptId = scriptId;
    }

    public Guid ScriptId { get; }

    public class Handler : IRequestHandler<CloseScriptCommand>
    {
        private readonly ISession _session;
        private readonly IAutoSaveScriptRepository _autoSaveScriptRepository;
        private readonly IMediator _mediator;
        private readonly IEventBus _eventBus;

        public Handler(
            ISession session,
            IAutoSaveScriptRepository autoSaveScriptRepository,
            IMediator mediator,
            IEventBus eventBus)
        {
            _session = session;
            _autoSaveScriptRepository = autoSaveScriptRepository;
            _mediator = mediator;
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(CloseScriptCommand request, CancellationToken cancellationToken)
        {
            var scriptId = request.ScriptId;

            var script = _session.Get(scriptId)?.Script ?? throw new ScriptNotFoundException(scriptId);

            await _session.CloseAsync(scriptId);

            await _autoSaveScriptRepository.DeleteAsync(script);

            await _eventBus.PublishAsync(new ScriptClosedEvent(script));

            return Unit.Value;
        }
    }
}
