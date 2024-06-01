using MediatR;
using NetPad.Events;
using NetPad.Scripts;
using NetPad.Scripts.Events;

namespace NetPad.Apps.App.Common.CQs;

/// <summary>
/// Duplicates a script.
/// </summary>
public class DuplicateScriptCommand : Command<Script>
{
    public Script Script { get; }

    public DuplicateScriptCommand(Script script)
    {
        Script = script;
    }

    public class Handler : IRequestHandler<DuplicateScriptCommand, Script>
    {
        private readonly IScriptRepository _scriptRepository;
        private readonly IScriptNameGenerator _scriptNameGenerator;
        private readonly IEventBus _eventBus;

        public Handler(IScriptRepository scriptRepository, IScriptNameGenerator scriptNameGenerator, IEventBus eventBus)
        {
            _scriptRepository = scriptRepository;
            _scriptNameGenerator = scriptNameGenerator;
            _eventBus = eventBus;
        }

        public async Task<Script> Handle(DuplicateScriptCommand request, CancellationToken cancellationToken)
        {
            var name = _scriptNameGenerator.Generate(request.Script.Name);
            var script = await _scriptRepository.CreateAsync(name);
            await _eventBus.PublishAsync(new ScriptCreatedEvent(script));

            script.SetDataConnection(request.Script.DataConnection);
            script.UpdateConfig(request.Script.Config);
            script.UpdateCode(request.Script.Code);

            return script;
        }
    }
}
