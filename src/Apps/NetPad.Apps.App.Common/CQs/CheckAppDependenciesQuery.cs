﻿using MediatR;
using Microsoft.Extensions.Logging;
using NetPad.Application;
using NetPad.DotNet;

namespace NetPad.Apps.App.Common.CQs;

public class CheckAppDependenciesQuery : Query<AppDependencyCheckResult>
{
    public class Handler : IRequestHandler<CheckAppDependenciesQuery, AppDependencyCheckResult>
    {
        private readonly IDotNetInfo _dotNetInfo;
        private readonly ILogger<Handler> _logger;

        public Handler(IDotNetInfo dotNetInfo, ILogger<Handler> logger)
        {
            _dotNetInfo = dotNetInfo;
            _logger = logger;
        }

        public Task<AppDependencyCheckResult> Handle(CheckAppDependenciesQuery request,
            CancellationToken cancellationToken)
        {
            DotNetSdkVersion[]? dotNetSdkVersions = null;
            SemanticVersion? dotNetEfToolVersion = null;

            try
            {
                dotNetSdkVersions = _dotNetInfo.GetDotNetSdkVersions();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting .NET SDK versions");
            }

            try
            {
                var dotNetEfToolExePath = _dotNetInfo.LocateDotNetEfToolExecutable();

                dotNetEfToolVersion = dotNetEfToolExePath == null
                    ? null
                    : _dotNetInfo.GetDotNetEfToolVersion(dotNetEfToolExePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting .NET Entity Framework Tool version");
            }

            var result = new AppDependencyCheckResult(
                _dotNetInfo.GetCurrentDotNetRuntimeVersion().ToString(),
                dotNetSdkVersions?.Select(v => v.Version).ToArray() ?? Array.Empty<SemanticVersion>(),
                dotNetEfToolVersion
            );

            return Task.FromResult(result);
        }
    }
}
