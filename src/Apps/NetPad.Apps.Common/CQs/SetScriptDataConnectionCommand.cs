using MediatR;
using NetPad.Data;
using NetPad.Events;
using NetPad.Scripts;
using NetPad.Scripts.Events;

namespace NetPad.Apps.CQs;

public class SetScriptDataConnectionCommand : Command
{
    public SetScriptDataConnectionCommand(Script script, DataConnection? connection)
    {
        Script = script;
        Connection = connection;
    }

    public Script Script { get; }
    public DataConnection? Connection { get; }

    public class Handler : IRequestHandler<SetScriptDataConnectionCommand, Unit>
    {
        private readonly IEventBus _eventBus;

        public Handler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(SetScriptDataConnectionCommand request, CancellationToken cancellationToken)
        {
            var script = request.Script;
            var connection = request.Connection;

            if (script.DataConnection?.Id == connection?.Id)
            {
                return Unit.Value;
            }

            script.SetDataConnection(connection);

            await _eventBus.PublishAsync(new ScriptDataConnectionChangedEvent(script, connection));

            return Unit.Value;
        }
    }
}
