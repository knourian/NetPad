using MediatR;
using NetPad.Events;
using NetPad.Scripts;

namespace NetPad.Apps.CQs;

/// <summary>
/// Renames a script.
/// </summary>
public class RenameScriptCommand : Command
{
    public Script Script { get; }
    public string NewName { get; }

    public RenameScriptCommand(Script script, string newName)
    {
        Script = script;
        NewName = newName;
    }

    public class Handler : IRequestHandler<RenameScriptCommand>
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

        public async Task<Unit> Handle(RenameScriptCommand request, CancellationToken cancellationToken)
        {
            var script = request.Script;
            var newName = request.NewName;

            _scriptRepository.Rename(script, newName);

            if (script.IsDirty)
            {
                await _autoSaveScriptRepository.DeleteAsync(script);
                await _autoSaveScriptRepository.SaveAsync(script);
            }

            return Unit.Value;
        }
    }
}
