using Xunit;

namespace CareWorkOps.Api.IntegrationTests.Infrastructure;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection
    : ICollectionFixture<CareWorkOpsApiTestFactory>
{
}