using System;
using Microsoft.Extensions.DependencyInjection;
using NetPad.Data.EntityFrameworkCore.DataConnections;

namespace NetPad.Data.EntityFrameworkCore;

internal class EntityFrameworkConnectionMetadataProviderFactory : IDatabaseConnectionMetadataProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkConnectionMetadataProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDatabaseConnectionMetadataProvider Create(DatabaseConnection databaseConnection)
    {
        if (databaseConnection is EntityFrameworkDatabaseConnection)
        {
            return _serviceProvider.GetRequiredService<EntityFrameworkDatabaseConnectionMetadataProvider>();
        }

        throw new NotImplementedException("Only EntityFramework database connections are supported.");
    }
}
