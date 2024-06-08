using System;
using Microsoft.Extensions.DependencyInjection;
using NetPad.Data.EntityFrameworkCore.DataConnections;

namespace NetPad.Data.EntityFrameworkCore;

internal class EntityFrameworkConnectionResourcesGeneratorFactory : IDataConnectionResourcesGeneratorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkConnectionResourcesGeneratorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDataConnectionResourcesGenerator Create(DataConnection dataConnection)
    {
        if (dataConnection is EntityFrameworkDatabaseConnection)
        {
            return _serviceProvider.GetRequiredService<EntityFrameworkResourcesGenerator>();
        }

        throw new NotImplementedException("Only EntityFramework data connections are supported.");
    }
}
