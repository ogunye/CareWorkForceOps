using CareWorkOps.Domain.Common;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NetArchTest.Rules;
using System.Reflection;

namespace CareWorkOps.Architecture.Tests
{
    public sealed class ArchitectureRulesTests
    {
        private static readonly Assembly DomainAssembly = typeof(Entity<>).Assembly;
        private static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;
        private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;
        private static readonly Assembly PersistenceAssembly = typeof(Persistence.DependencyInjection).Assembly;
        private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

        [Fact]
        public void Domain_Should_Not_Depend_On_Other_Projects()
        {
            var forbiddenDependencies = new[]
            {
            "CareWorkOps.Application",
            "CareWorkOps.Infrastructure",
            "CareWorkOps.Persistence",
            "CareWorkOps.Api",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.AspNetCore"
        };

            var result = Types
                .InAssembly(DomainAssembly)
                .ShouldNot()
                .HaveDependencyOnAny(forbiddenDependencies)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Infrastructure_Persistence_Or_Api()
        {
            var forbiddenDependencies = new[]
            {
            "CareWorkOps.Infrastructure",
            "CareWorkOps.Persistence",
            "CareWorkOps.Api",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.AspNetCore"
        };

            var result = Types
                .InAssembly(ApplicationAssembly)
                .ShouldNot()
                .HaveDependencyOnAny(forbiddenDependencies)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Api()
        {
            var result = Types
                .InAssembly(InfrastructureAssembly)
                .ShouldNot()
                .HaveDependencyOn("CareWorkOps.Api")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Persistence_Should_Not_Depend_On_Api()
        {
            var result = Types
                .InAssembly(PersistenceAssembly)
                .ShouldNot()
                .HaveDependencyOn("CareWorkOps.Api")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Controllers_Should_Not_Depend_On_DbContext()
        {
            var result = Types
                .InAssembly(ApiAssembly)
                .That()
                .HaveNameEndingWith("Controller")
                .ShouldNot()
                .HaveDependencyOn("CareWorkOps.Persistence.Context.ApplicationDbContext")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Controllers_Should_Not_Depend_On_Repositories()
        {
            var result = Types
                .InAssembly(ApiAssembly)
                .That()
                .HaveNameEndingWith("Controller")
                .ShouldNot()
                .HaveDependencyOn("CareWorkOps.Application.Abstractions.Persistence")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Handlers_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types
                .InAssembly(ApplicationAssembly)
                .That()
                .HaveNameEndingWith("Handler")
                .ShouldNot()
                .HaveDependencyOn("CareWorkOps.Infrastructure")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Handlers_Should_Not_Depend_On_Persistence()
        {
            var result = Types
                .InAssembly(ApplicationAssembly)
                .That()
                .HaveNameEndingWith("Handler")
                .ShouldNot()
                .HaveDependencyOn("CareWorkOps.Persistence")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Entities_Should_Not_Have_Public_Setters()
        {
            var entityTypes = Types
                .InAssembly(DomainAssembly)
                .That()
                .Inherit(typeof(Entity<>))
                .GetTypes();

            var invalidProperties = entityTypes
                .SelectMany(type => type.GetProperties()
                    .Where(property =>
                        property.SetMethod is not null &&
                        property.SetMethod.IsPublic))
                .ToList();

            invalidProperties.Should().BeEmpty();
        }

        [Fact]
        public void Application_Commands_Should_End_With_Command()
        {
            var commandTypes = Types
                .InAssembly(ApplicationAssembly)
                .That()
                .ResideInNamespace("CareWorkOps.Application")
                .And()
                .HaveNameEndingWith("Command")
                .GetTypes();

            commandTypes.Should().OnlyContain(type => type.Name.EndsWith("Command"));
        }

        [Fact]
        public void Application_Handlers_Should_End_With_Handler()
        {
            var handlerTypes = Types
                .InAssembly(ApplicationAssembly)
                .That()
                .HaveDependencyOn("MediatR")
                .And()
                .HaveNameEndingWith("Handler")
                .GetTypes();

            handlerTypes.Should().OnlyContain(type => type.Name.EndsWith("Handler"));
        }

        [Fact]
        public void Validators_Should_End_With_Validator()
        {
            var validatorTypes = Types
                .InAssembly(ApplicationAssembly)
                .That()
                .HaveDependencyOn("FluentValidation")
                .And()
                .HaveNameEndingWith("Validator")
                .GetTypes();

            validatorTypes.Should().OnlyContain(type => type.Name.EndsWith("Validator"));
        }
    }
}
