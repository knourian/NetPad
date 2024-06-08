using MediatR;
using NetPad.Events;
using NetPad.Scripts;
using NetPad.Scripts.Events;

namespace NetPad.Apps.CQs;

/// <summary>
/// Saves a script. Returns true if script was saved, false otherwise.
/// </summary>
public class SaveScriptCommand : Command
{
    public Script Script { get; }

    public SaveScriptCommand(Script script)
    {
        Script = script;
    }

    public class Handler : IRequestHandler<SaveScriptCommand>
    {
        private readonly IScriptRepository _scriptRepository;
        private readonly IAutoSaveScriptRepository _autoSaveScriptRepository;
        private readonly IEventBus _eventBus;

        public Handler(
            IScriptRepository scriptRepository,
            IAutoSaveScriptRepository autoSaveScriptRepository,
            IEventBus eventBus
        )
        {
            _scriptRepository = scriptRepository;
            _autoSaveScriptRepository = autoSaveScriptRepository;
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(SaveScriptCommand request, CancellationToken cancellationToken)
        {
            var script = request.Script;

            await _scriptRepository.SaveAsync(script);

            await _autoSaveScriptRepository.DeleteAsync(script);

            await _eventBus.PublishAsync(new ScriptSavedEvent(script));

            return Unit.Value;
        }
    }
}
