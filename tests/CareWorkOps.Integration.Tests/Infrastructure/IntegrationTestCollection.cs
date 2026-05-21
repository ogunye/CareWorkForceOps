using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Integration.Tests.Infrastructure
{
    [CollectionDefinition(nameof(IntegrationTestCollection))]
    public sealed class IntegrationTestCollection
    : ICollectionFixture<CareWorkOpsApiFactory>
    {
    }
}
