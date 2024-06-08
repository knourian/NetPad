using MediatR;
using NetPad.Data;
using NetPad.DotNet;
using NetPad.Sessions;

namespace NetPad.Apps.CQs;

public class RefreshDataConnectionCommand : Command
{
    public RefreshDataConnectionCommand(Guid id)
    {
        ConnectionId = id;
    }

    public Guid ConnectionId { get; }

    public class Handler : IRequestHandler<RefreshDataConnectionCommand, Unit>
    {
        private readonly IDataConnectionResourcesCache _dataConnectionResourcesCache;
        private readonly IDataConnectionRepository _dataConnectionRepository;
        private readonly ISession _session;
        private readonly IDotNetInfo _dotNetInfo;

        public Handler(
            IDataConnectionResourcesCache dataConnectionResourcesCache,
            IDataConnectionRepository dataConnectionRepository,
            ISession session,
            IDotNetInfo dotNetInfo)
        {
            _dataConnectionResourcesCache = dataConnectionResourcesCache;
            _dataConnectionRepository = dataConnectionRepository;
            _session = session;
            _dotNetInfo = dotNetInfo;
        }

        public async Task<Unit> Handle(RefreshDataConnectionCommand request, CancellationToken cancellationToken)
        {
            var currentActiveScriptTargetFrameworkVersion = _session.Active?.Script.Config.TargetFrameworkVersion;

            var connection = await _dataConnectionRepository.GetAsync(request.ConnectionId);

            if (connection == null)
            {
                return Unit.Value;
            }

            await _dataConnectionResourcesCache.RemoveCachedResourcesAsync(request.ConnectionId);

            var targetFramework = currentActiveScriptTargetFrameworkVersion
                                  ?? _dotNetInfo.GetLatestSupportedDotNetSdkVersionOrThrow().FrameworkVersion();

            await _dataConnectionResourcesCache.GetAssemblyAsync(connection, targetFramework);

            return Unit.Value;
        }
    }
}
