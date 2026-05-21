using CareWorkOps.Integration.Tests.Infrastructure;
using CareWorkOps.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CareWorkOps.Integration.Tests.Tenants
{
    [Collection(nameof(IntegrationTestCollection))]
    public sealed class CreateTenantEndpointTests
    {
        private readonly CareWorkOpsApiFactory _factory;
        private readonly HttpClient _client;

        public CreateTenantEndpointTests(CareWorkOpsApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTenant_Should_Return_Created_When_Request_Is_Valid()
        {
            var request = new
            {
                tenantName = "Alpha Care Ltd",
                tenantSlug = "alpha-care",
                adminFirstName = "John",
                adminLastName = "Smith",
                adminEmail = "admin@alphacare.com",
                adminPassword = "Password123!",
                isolationMode = "SharedDatabase",
                connectionString = (string?)null
            };

            var response = await _client.PostAsJsonAsync("/api/v1/tenants", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var json = await response.Content.ReadAsStringAsync();

            json.Should().Contain("Tenant created successfully");
            json.Should().Contain("alpha-care");

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var tenant = dbContext.Tenants.SingleOrDefault(x => x.Slug == "alpha-care");

            tenant.Should().NotBeNull();
            tenant!.Name.Should().Be("Alpha Care Ltd");
        }

        [Fact]
        public async Task CreateTenant_Should_Return_BadRequest_When_Request_Is_Invalid()
        {
            var request = new
            {
                tenantName = "",
                tenantSlug = "Invalid Slug",
                adminFirstName = "",
                adminLastName = "",
                adminEmail = "not-an-email",
                adminPassword = "weak",
                isolationMode = "SharedDatabase",
                connectionString = (string?)null
            };

            var response = await _client.PostAsJsonAsync("/api/v1/tenants", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync();

            json.Should().Contain("Validation");
            json.Should().Contain("correlationId");
        }

        [Fact]
        public async Task CreateTenant_Should_Return_Conflict_When_TenantSlug_Already_Exists()
        {
            var request = new
            {
                tenantName = "Alpha Care Ltd",
                tenantSlug = "duplicate-care",
                adminFirstName = "John",
                adminLastName = "Smith",
                adminEmail = "admin1@duplicatecare.com",
                adminPassword = "Password123!",
                isolationMode = "SharedDatabase",
                connectionString = (string?)null
            };

            var firstResponse = await _client.PostAsJsonAsync("/api/v1/tenants", request);

            firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var duplicateRequest = new
            {
                tenantName = "Duplicate Care Ltd",
                tenantSlug = "duplicate-care",
                adminFirstName = "Mary",
                adminLastName = "Jones",
                adminEmail = "admin2@duplicatecare.com",
                adminPassword = "Password123!",
                isolationMode = "SharedDatabase",
                connectionString = (string?)null
            };

            var secondResponse = await _client.PostAsJsonAsync("/api/v1/tenants", duplicateRequest);

            secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var json = await secondResponse.Content.ReadAsStringAsync();

            json.Should().Contain("already exists");
        }

        [Fact]
        public async Task CreateTenant_Should_Create_Tenant_Admin_User()
        {
            var request = new
            {
                tenantName = "Beta Care Ltd",
                tenantSlug = "beta-care",
                adminFirstName = "Sarah",
                adminLastName = "Adams",
                adminEmail = "admin@betacare.com",
                adminPassword = "Password123!",
                isolationMode = "SharedDatabase",
                connectionString = (string?)null
            };

            var response = await _client.PostAsJsonAsync("/api/v1/tenants", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var user = dbContext.Users.SingleOrDefault(x => x.Email == "admin@betacare.com");

            user.Should().NotBeNull();
            user!.TenantId.Should().NotBeEmpty();
            user.FirstName.Should().Be("Sarah");
            user.LastName.Should().Be("Adams");
        }
    }
}
