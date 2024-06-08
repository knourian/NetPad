using MediatR;
using NetPad.Events;
using NetPad.Scripts;
using NetPad.Scripts.Events;

namespace NetPad.Apps.App.Common.CQs;

public class UpdateScriptCodeCommand : Command
{
    public UpdateScriptCodeCommand(Script script, string? code)
    {
        Script = script;
        Code = code;
    }

    public Script Script { get; }
    public string? Code { get; }

    public class Handler : IRequestHandler<UpdateScriptCodeCommand>
    {
        private readonly IEventBus _eventBus;

        public Handler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(UpdateScriptCodeCommand request, CancellationToken cancellationToken)
        {
            var script = request.Script;

            var oldCode = script.Code;

            if (oldCode == request.Code)
            {
                return Unit.Value;
            }

            request.Script.UpdateCode(request.Code);

            await _eventBus.PublishAsync(new ScriptCodeUpdatedEvent(script, script.Code, oldCode));

            return Unit.Value;
        }
    }
}
